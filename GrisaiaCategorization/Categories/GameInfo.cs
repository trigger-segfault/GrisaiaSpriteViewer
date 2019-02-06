using System;
using System.Diagnostics;
using System.IO;
using GalaSoft.MvvmLight;
using Grisaia.Asmodean;
using Grisaia.Locators;
using Newtonsoft.Json;

namespace Grisaia.Categories {
	/// <summary>
	///  A class storing static information about a single Grisaia game.
	/// </summary>
	public sealed class GameInfo {
		#region Fields

		/// <summary>
		///  Gets the game database containing this game info.
		/// </summary>
		[JsonIgnore]
		public GameDatabase Database { get; internal set; }
		/// <summary>
		///  The collection of loaded and cached KIFINT lookups.
		/// </summary>
		[JsonIgnore]
		private readonly KifintLookupCollection lookups = new KifintLookupCollection();
		/// <summary>
		///  The programmatically located installation information used for the game.
		/// </summary>
		[JsonIgnore]
		private GameInstallInfo locatedInstall;
		/// <summary>
		///  The optional custom installation information used for the game.
		/// </summary>
		[JsonIgnore]
		private GameInstallInfo customInstall;

		/// <summary>
		///  Gets the unique identifier for the game.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <summary>
		///  Gets the full name of the Japanese version of the game.
		/// </summary>
		[JsonProperty("jp_name")]
		public string JPName { get; private set; }
		/// <summary>
		///  Gets the full name of the Japanese version of the game.
		/// </summary>
		[JsonProperty("jp_short")]
		public string JPShortName { get; private set; }
		/// <summary>
		///  Gets the full name of the English version of the game.
		/// </summary>
		[JsonProperty("en_name")]
		public string ENName { get; private set; }
		/// <summary>
		///  Gets the full name of the English version of the game.
		/// </summary>
		[JsonProperty("en_short")]
		public string ENShortName { get; private set; }
		/// <summary>
		///  Gets the Id of the Steam game. 0 is an invalid Id.
		/// </summary>
		[JsonProperty("steam_id")]
		public uint? SteamId { get; private set; }
		/// <summary>
		///  Gets the Id of the VNDb game. 0 is an invalid Id.
		/// </summary>
		[JsonProperty("vndb_id")]
		public uint VNDbId { get; private set; }
		/// <summary>
		///  Gets the name of the Frontwing installation registry key.
		/// </summary>
		[JsonProperty("registry")]
		public string FrontwingRegistryValue { get; private set; }

		#endregion

		#region Properties

		/// <summary>
		///  Gets the name of the game executable.
		/// </summary>
		[JsonProperty("exe")]
		private string DefaultExecutable {
			get => locatedInstall.Executable;
			set => locatedInstall.Executable = value;
		}

		/// <summary>
		///  Gets if the located install location is validated and usable.
		/// </summary>
		[JsonIgnore]
		public bool HasLocatedValidated => locatedInstall.VCode2 != null;
		/// <summary>
		///  Gets if the custom install location is validated and usable.
		/// </summary>
		[JsonIgnore]
		public bool HasCustomValidated => customInstall.VCode2 != null;
		/// <summary>
		///  Gets if the custom or located install location is validated and usable.
		/// </summary>
		[JsonIgnore]
		public bool IsValidated => (customInstall.VCode2 ?? locatedInstall.VCode2) != null;

		/// <summary>
		///  Gets the programmatically located installation information used for the game.
		/// </summary>
		[JsonIgnore]
		public GameInstallInfo LocatedInstall => locatedInstall;
		/// <summary>
		///  Gets the optional custom installation information used for the game.
		/// </summary>
		[JsonIgnore]
		public GameInstallInfo CustomInstall => customInstall;
		/// <summary>
		///  Gets the installation information for the currently chosen install location (located or custom).
		/// </summary>
		[JsonIgnore]
		public GameInstallInfo CurrentInstall {
			get {
				if (customInstall.VCode2  != null) {
					GameInstallInfo custom = customInstall;
					custom.Executable = locatedInstall.Executable;
					return custom;
				}
				if (locatedInstall.VCode2 != null) {
					return locatedInstall;
				}
				return GameInstallInfo.None;
			}
		}

		/// <summary>
		///  Gets the custom or located installation directory of the game.
		/// </summary>
		[JsonIgnore]
		public string InstallDir {
			get {
				if (customInstall.IsValidated)
					return customInstall.Directory ?? locatedInstall.Directory;
				return locatedInstall.Directory;
			}
		}
		/// <summary>
		///  Gets the custom or default game executable file name.
		/// </summary>
		[JsonIgnore]
		public string Executable {
			get {
				if (customInstall.IsValidated)
					return customInstall.Executable ?? locatedInstall.Executable;
				return locatedInstall.Executable;
			}
		}
		/// <summary>
		///  Gets the located or custom V_CODE2 resource used for KIFINT archive extraction.
		/// </summary>
		[JsonIgnore]
		public string VCode2 => customInstall.VCode2 ?? locatedInstall.VCode2;

		/// <summary>
		///  Gets the collection of loaded and cached KIFINT lookups.
		/// </summary>
		[JsonIgnore]
		public IKifintLookupCollection Lookups => lookups;
		/// <summary>
		///  Formats the name of the this game info using the naming scheme settings.
		/// </summary>
		/// <returns>The game's name with the naming scheme applied.</returns>
		[JsonIgnore]
		public string FormattedName => Database.NamingScheme.GetName(this);
		/// <summary>
		///  Formats the full name of the this game info using the naming scheme language settings.
		/// </summary>
		/// <returns>The game's name with the naming scheme applied.</returns>
		[JsonIgnore]
		public string FormattedFullName => Database.NamingScheme.GetFullName(this);
		/// <summary>
		///  Formats the short name of the this game info using the naming scheme language settings.
		/// </summary>
		/// <returns>The game's name with the naming scheme applied.</returns>
		[JsonIgnore]
		public string FormattedShortName => Database.NamingScheme.GetShortName(this);
		/// <summary>
		///  Gets or sets the path leading to where cached files for this game are located.
		/// </summary>
		[JsonIgnore]
		public string CachePath => Path.Combine(Database.GrisaiaDatabase.CachePath, Id);

		#endregion

		#region ToString Override

		/// <summary>
		///  Gets the string representation of the game info.
		/// </summary>
		/// <returns>The string representation of the game info.</returns>
		public override string ToString() => Id;

		#endregion

		#region LoadGame

		/// <summary>
		///  Loads the dummy game install directory and V_CODE2.
		/// </summary>
		/// <returns>Always true.</returns>
		public bool LoadDummyGame() {
			locatedInstall.Directory = $@"C:\{ENShortName}";
			locatedInstall.VCode2 = "FC-ASKJDHAS"; // Just a random value
			customInstall = GameInstallInfo.None;
			return true;
		}
		/*/// <summary>
		///  Loads the game install directory and V_CODE2 if they are found.
		/// </summary>
		/// <returns>True if the game was successfully loaded.</returns>
		public bool LoadGame() {
			SteamLibraryFolders steamLibraries = null;
			try {
				steamLibraries = SteamLocator.FindLibrariesFromRegistry();
			} catch (SteamException) { }
			return LoadGame(steamLibraries);
		}*/
		/// <summary>
		///  Checks if the custom installation information leads to a valid executable with a valid V_CODE2.
		/// </summary>
		/// <param name="customInstall">The reference installation info to get the V_CODE2 assigned to.</param>
		/// <returns>True if a V_CODE2 was located.</returns>
		public bool ValidateCustomInstall(GameInstallInfo customInstall) {
			//if (customInstall.Directory == null)
			//	throw new ArgumentException($"{nameof(customInstall)} does not have a " +
			//								$"{nameof(GameInstallInfo.Directory)}!");

			if (customInstall.Directory != null)
				return FindVCode2(customInstall) != null;
			return false;
		}
		/// <summary>
		///  Reloads the game install directory and V_CODE2 if they are found.
		/// </summary>
		/// <param name="steamLibraries">The Steam library folders to look for the game install in.</param>
		/// <param name="customInstall">The optional custom installation information.</param>
		/// <returns>True if the game was successfully loaded.</returns>
		public bool ReloadGame(SteamLibraryFolders steamLibraries, GameInstallInfo custom, out bool cacheNeedsReload) {
			GameInstallInfo oldInstall = CurrentInstall;

			LoadGame(steamLibraries, custom);

			if (oldInstall != CurrentInstall || oldInstall.VCode2 != CurrentInstall.VCode2) {
				// We'll want to reload these since something has changed.
				lookups.Clear();
				cacheNeedsReload = true;
			}
			else {
				cacheNeedsReload = false;
			}

			return IsValidated;
		}
		/// <summary>
		///  Loads the game install directory and V_CODE2 if they are found.
		/// </summary>
		/// <param name="steamLibraries">The Steam library folders to look for the game install in.</param>
		/// <param name="customInstall">The optional custom installation information.</param>
		/// <returns>True if the game was successfully loaded.</returns>
		public bool LoadGame(SteamLibraryFolders steamLibraries, GameInstallInfo custom) {
			locatedInstall.Directory = LocateGame(steamLibraries);
			if (locatedInstall.Directory != null)
				locatedInstall.VCode2 = FindVCode2(locatedInstall);
			else
				locatedInstall.VCode2 = null;

			customInstall = custom;
			if (customInstall.Directory != null)
				customInstall.VCode2 = FindVCode2(customInstall);
			else
				customInstall.VCode2 = null;

			return IsValidated;
		}
		/*/// <summary>
		///  Looks for the game's install directory.
		/// </summary>
		/// <returns>The game's install path if found, otherwise null.</returns>
		public string LocateGame() {
			SteamLibraryFolders steamLibraries = null;
			try {
				steamLibraries = SteamLocator.FindLibrariesFromRegistry();
			} catch (SteamException) { }
			return LocateGame(steamLibraries);
		}*/
		/// <summary>
		///  Looks for the game's install directory.
		/// </summary>
		/// <param name="steamLibraries">The Steam library folders to look for the game install in.</param>
		/// <returns>The game's install path if found, otherwise null.</returns>
		private string LocateGame(SteamLibraryFolders steamLibraries) {
			string installDir = null;
			if (installDir == null && SteamId.HasValue && steamLibraries != null) {
				installDir = SteamLocator.LocateGame(steamLibraries, SteamId.Value);
			}
			if (installDir == null && FrontwingRegistryValue != null) {
				installDir = FrontwingLocator.LocateGame(FrontwingRegistryValue);
			}
			return installDir;
		}
		/// <summary>
		///  Looks for the game's V_CODE2 resource in the specified installation directory.
		/// </summary>
		/// <param name="installDir">The installation directory to look in.</param>
		/// <returns>The V_CODE2 string if found, otherwise null.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="installDir"/> is null.
		/// </exception>
		private string FindVCode2(GameInstallInfo install) {
			if (install.Directory == null)
				throw new ArgumentException($"{nameof(install)} does not have a " +
											$"{nameof(GameInstallInfo.Directory)}!");

			// Make sure the install has an executable location by defaulting to located install
			install = new GameInstallInfo(install, locatedInstall);
			try {
				return Asmodean.VCode2.Find(install.ExecutablePath);
			} catch { }
			if (File.Exists(install.ExecutableBinPath)) {
				try {
					return Asmodean.VCode2.Find(install.ExecutableBinPath);
				} catch { }
			}
			return null;
		}

		#endregion

		#region Lookup

		/// <summary>
		///  Clears the loading KIFINT archive lookup collection.
		/// </summary>
		internal void ClearLookups() => lookups.Clear();
		/// <summary>
		///  Clears the KIFINT archive lookup for the specified type.
		/// </summary>
		/// <param name="type">The type of the KIFINT archive to look for.</param>
		/// <param name="progress">The arguments to modify as loading progresses.</param>
		/// <param name="callback">The progress callback.</param>
		internal KifintLookup LoadLookup(KifintType type, LoadCacheProgressArgs progress,
			LoadCacheProgressCallback callback)
		{
			Debug.WriteLine($"Loading {type} Cache: {Id}");
			string lookupName = Path.ChangeExtension($"{Id}-{type.ToString().ToLower()}", ".intlookup");
			string lookupFile = Path.Combine(Database.GrisaiaDatabase.CachePath, lookupName);
			KifintLookup lookup = null;
			KifintProgressCallback kifintCallback = null;
			if (callback != null) {
				kifintCallback = (e) => {
					progress.Kifint = e;
					callback(progress);
				};
			}
			if (File.Exists(lookupFile)) {
				try {
					callback?.Invoke(progress);
					lookup = KifintLookup.Load(lookupFile, InstallDir);
				} catch { }
			}
			if (lookup == null) {
				Debug.WriteLine($"Building {type} Cache: {Id}");
				progress.IsBuilding = true;
				lookup = Kifint.Decrypt(type, InstallDir, VCode2, kifintCallback);
				lookup.Save(lookupFile);
				progress.IsBuilding = false;
			}
			lookups[type] = lookup;
			return lookup;
		}
		/// <summary>
		///  Clears the KIFINT archive lookup for the specified unknown type.
		/// </summary>
		/// <param name="unknownName">The name of the unknown type to store the archive as.</param>
		/// <param name="wildcard">The wildcard used to look for the archives.</param>
		/// <param name="progress">The arguments to modify as loading progresses.</param>
		/// <param name="callback">The progress callback.</param>
		internal KifintLookup LoadLookup(string unknownName, string wildcard, LoadCacheProgressArgs progress,
			LoadCacheProgressCallback callback)
		{
			Debug.WriteLine($"Loading \"{unknownName}\" Cache: {Id}");
			string lookupName = Path.ChangeExtension($"{Id}-{unknownName}", ".intlookup");
			string lookupFile = Path.Combine(Database.GrisaiaDatabase.CachePath, lookupName);
			KifintLookup lookup = null;
			KifintProgressCallback kifintCallback = null;
			if (callback != null) {
				kifintCallback = (e) => {
					progress.Kifint = e;
					callback(progress);
				};
			}
			if (File.Exists(lookupFile)) {
				try {
					callback?.Invoke(progress);
					lookup = KifintLookup.Load(lookupFile, InstallDir);
				} catch { }
			}
			if (lookup == null) {
				Debug.WriteLine($"Building \"{unknownName}\" Cache: {Id}");
				progress.IsBuilding = true;
				lookup = Kifint.Decrypt(wildcard, InstallDir, VCode2, kifintCallback);
				lookup.Save(lookupFile);
				progress.IsBuilding = false;
			}
			lookups[unknownName] = lookup;
			return lookup;
		}

		#endregion
	}
}
