using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using Microsoft.Win32;
using TriggersTools.SharpUtils.IO;

namespace Grisaia.Locators {
	/// <summary>
	///  A class for locating an installed Steam game with the specified app Id.
	/// </summary>
	public static class SteamLocator {
		#region Constants

		/// <summary>
		///  The path to the Steam registry key.
		/// </summary>
		private const string SteamRegistryPath = @"HKEY_CURRENT_USER\Software\Valve\Steam";
		/// <summary>
		///  The name of the registry value specifying the Steam installation path.
		/// </summary>
		private const string SteamPathValue = "SteamPath";
		/// <summary>
		///  The steam apps folder leading to the app manifests.
		/// </summary>
		private const string SteamApps = "steamapps";
		/// <summary>
		///  The common folder inside a library folder.
		/// </summary>
		private const string Common = "common";
		/// <summary>
		///  The library folders file name.
		/// </summary>
		private const string LibraryFolders = "libraryfolders.vdf";
		/// <summary>
		///  The generic app manifest file name that is formatted during <see cref="GetAppManifestFileName"/>.
		/// </summary>
		private const string AppManifest = @"appmanifest_{0}.acf";
		/// <summary>
		///  The vdf key for the game intallation directory.
		/// </summary>
		private const string InstallDirVdfKey = "installdir";

		#endregion

		#region LibraryFolders

		/// <summary>
		///  Finds the collection of Steam library folders.
		/// </summary>
		/// <returns>The collection of Steam library folders.</returns>
		/// 
		/// <exception cref="SteamNotInstalledException">
		///  Steam installation could not be located in the Registry.
		/// </exception>
		/// <exception cref="SteamException">
		///  An error occurred while reading the Steam folders.
		/// </exception>
		public static SteamLibraryFolders FindLibrariesFromRegistry() {
			if (Registry.GetValue(SteamRegistryPath, SteamPathValue, null) is string steamPath) {
				return FindLibrariesFromSteamPath(steamPath);
			}
			throw new SteamNotInstalledException();
		}
		/// <summary>
		///  Finds the collection of Steam library folders 
		/// </summary>
		/// <param name="steamPath">The located path to the Steam installation folder.</param>
		/// <returns>The collection of Steam library folders.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="steamPath"/> is null.
		/// </exception>
		/// <exception cref="SteamException">
		///  An error occurred while reading the Steam folders.
		/// </exception>
		public static SteamLibraryFolders FindLibrariesFromSteamPath(string steamPath) {
			if (steamPath == null)
				throw new ArgumentNullException(nameof(steamPath));

			List<string> folders = new List<string>();
			steamPath = PrettifyDir(steamPath);
			string steamApps = Path.Combine(steamPath, SteamApps);
			if (!PathUtils.IsValidDirectory(steamPath) || !Directory.Exists(steamApps))
				throw new SteamException($"Steam installation path does not have a \"{SteamApps}\" folder!");
			folders.Add(steamPath);

			string libraryFolders = Path.Combine(steamApps, LibraryFolders);
			string libraryFoldersRelative = Path.Combine(SteamApps, LibraryFolders); // Relative for exceptions
			if (!File.Exists(libraryFolders))
				throw new SteamException($"Steam installation path does not have a \"{libraryFoldersRelative}\" file!");
			try {
				VProperty vlibsRoot = VdfConvert.Deserialize(File.ReadAllText(libraryFolders));
				VObject vlibs = vlibsRoot.Value as VObject;
				int index = 1;
				while (vlibs.TryGetValue((index++).ToString(), out VToken vlibToken)) {
					string folder = vlibToken.Value<string>();
					if (PathUtils.IsValidDirectory(folder) && Directory.Exists(folder)) {
						folders.Add(PrettifyDir(folder));
					}
				}
			} catch (Exception ex) {
				throw new SteamException($"An error occurred while trying to load the " +
										 $"\"{libraryFoldersRelative}\" file!", ex);
			}

			return new SteamLibraryFolders(steamPath, folders);
		}

		#endregion

		#region LocateGame

		/// <summary>
		///  Attempts to locate the installed Steam game with the specified Id in any existing library folders.
		/// </summary>
		/// <param name="folders">The loaded library folders for Steam.</param>
		/// <param name="steamId">The Steam game app Id.</param>
		/// <returns>The path to the installed game if found, otherwise null.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="folders"/> is null.
		/// </exception>
		public static string LocateGame(SteamLibraryFolders folders, uint steamId) {
			if (folders == null)
				throw new ArgumentNullException(nameof(folders));

			if (steamId == 0)
				return null;
			foreach (string folder in folders) {
				string appManifest = Path.Combine(folder, SteamApps, GetAppManifestFileName(steamId));
				// Is not present in this library folder
				if (!File.Exists(appManifest))
					continue;

				VProperty vappRoot = VdfConvert.Deserialize(File.ReadAllText(appManifest));
				VObject vapp = vappRoot.Value as VObject;
				if (!vapp.TryGetValue(InstallDirVdfKey, out VToken vappToken))
					continue;
				string installDir = vappToken.Value<string>();
				installDir = Path.Combine(folder, SteamApps, Common, installDir);
				if (!PathUtils.IsValidDirectory(installDir) || !Directory.Exists(installDir))
					continue;

				return PrettifyDir(installDir);
			}
			return null;
		}
		/// <summary>
		///  Gets the filename for the app manifest file for a specific steam game.
		/// </summary>
		/// <param name="steamId">The steam app Id of the game.</param>
		/// <returns>The name of the app manifest file for the game.</returns>
		public static string GetAppManifestFileName(uint steamId) {
			return string.Format(AppManifest, steamId);
		}

		#endregion

		#region Private Helpers

		/// <summary>
		///  Gives the directory a proper case-sensitive name.
		/// </summary>
		/// <param name="dir">The directory to prettify.</param>
		/// <returns>The prettified directory with the correct case-sensitive name.</returns>
		private static string PrettifyDir(string dir) {
			return PathUtils.GetProperDirectoryCapitalization(dir);
		}

		#endregion
	}

	/// <summary>
	///  A collection of folders Steam has marked as library paths.
	/// </summary>
	public sealed class SteamLibraryFolders : IReadOnlyList<string> {
		#region Fields

		/// <summary>
		///  Gets the path to the Steam install directory.
		/// </summary>
		public string SteamPath { get; }
		/// <summary>
		///  Gets the list of folders that function as Steam libraries.
		/// </summary>
		public IReadOnlyList<string> Folders { get; private set; }
		/// <summary>
		///  Gets the folder for the primary Steam path where games are installed by default.
		/// </summary>
		public string PrimaryFolder => Folders.First();

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs the Steam library folders collection using the specified Steam path and list of folder paths.
		/// </summary>
		/// <param name="steamPath">The steam installation path.</param>
		/// <param name="folders">The list of Steam library folders.</param>
		internal SteamLibraryFolders(string steamPath, IEnumerable<string> folders) {
			if (folders == null)
				throw new ArgumentNullException(nameof(folders));
			SteamPath = steamPath ?? throw new ArgumentNullException(nameof(steamPath));
			Folders = Array.AsReadOnly(folders.ToArray());
		}

		#endregion

		/// <summary>
		///  Refreshes the collection of library folders using <see cref="SteamPath"/>.
		/// </summary>
		public void Refresh() {
			Folders = SteamLocator.FindLibrariesFromSteamPath(SteamPath).Folders;
		}

		#region IReadOnlyList Implementation

		/// <summary>
		///  Gets the Steam library folder at the specified index.
		/// </summary>
		/// <param name="index">The index of the library folder.</param>
		/// <returns>The library folder at the specified index.</returns>
		public string this[int index] => Folders[index];
		/// <summary>
		///  Gets the number of Steam library folders.
		/// </summary>
		public int Count => Folders.Count;
		/// <summary>
		///  Gets the enumerator for the Steam library folders.
		/// </summary>
		/// <returns>The library folders enumerator.</returns>
		public IEnumerator<string> GetEnumerator() => Folders.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Folders.GetEnumerator();

		#endregion
	}
}
