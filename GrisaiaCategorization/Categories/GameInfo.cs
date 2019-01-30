using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
		///  Gets the located installation directory of the game.
		/// </summary>
		[JsonIgnore]
		public string InstallDir { get; private set; }
		/// <summary>
		///  Gets the V_CODE2 resource used for KIFINT archive extraction.
		/// </summary>
		[JsonIgnore]
		public string VCode2 { get; private set; }

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
		public uint VNDb { get; private set; }

		/// <summary>
		///  Gets the name of the Frontwing installation registry key.
		/// </summary>
		[JsonProperty("registry")]
		public string FrontwingRegistryValue { get; private set; }

		/// <summary>
		///  Gets the name of the game executable.
		/// </summary>
		[JsonProperty("exe")]
		public string Executable { get; private set; }

		#endregion

		#region Properties

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

		#region GetLookup


		#endregion

		#region LoadGame

		/// <summary>
		///  Loads the dummy game install directory and V_CODE2.
		/// </summary>
		/// <returns>Always true.</returns>
		public bool LoadDummyGame() {
			InstallDir = @"C:\";
			VCode2 = "FC-ASKJDHAS"; // Just a random value
			return true;
		}
		/// <summary>
		///  Loads the game install directory and V_CODE2 if they are found.
		/// </summary>
		/// <returns>True if the game was successfully loaded.</returns>
		public bool LoadGame() {
			SteamLibraryFolders steamLibraries = null;
			try {
				steamLibraries = SteamLocator.FindLibrariesFromRegistry();
			} catch (SteamException) { }
			return LoadGame(steamLibraries);
		}
		/// <summary>
		///  Loads the game install directory and V_CODE2 if they are found.
		/// </summary>
		/// <param name="steamLibraries">The Steam library folders to look for the game install in.</param>
		/// <returns>True if the game was successfully loaded.</returns>
		public bool LoadGame(SteamLibraryFolders steamLibraries) {
			VCode2 = null;

			// We'll want to reload these if something has changed.
			lookups.Clear();
			//ImageLookup = null;
			//UpdateLookup = null;

			InstallDir = LocateGame(steamLibraries);
			if (InstallDir != null) {
				VCode2 = FindVCode2(InstallDir);
			}
			return VCode2 != null;
		}

		/// <summary>
		///  Looks for the game's install directory.
		/// </summary>
		/// <returns>The game's install path if found, otherwise null.</returns>
		public string LocateGame() {
			SteamLibraryFolders steamLibraries = null;
			try {
				steamLibraries = SteamLocator.FindLibrariesFromRegistry();
			} catch (SteamException) { }
			return LocateGame(steamLibraries);
		}
		/// <summary>
		///  Looks for the game's install directory.
		/// </summary>
		/// <param name="steamLibraries">The Steam library folders to look for the game install in.</param>
		/// <returns>The game's install path if found, otherwise null.</returns>
		public string LocateGame(SteamLibraryFolders steamLibraries) {
			string installDir = null;
			if (installDir == null && SteamId.HasValue && steamLibraries != null) {
				installDir = SteamLocator.LocateGame(steamLibraries, SteamId.Value);
			}
			if (installDir == null && FrontwingRegistryValue != null) {
				installDir = FrontwingLocator.LocateGame(FrontwingRegistryValue);
			}
			// We should still set this to null if we lost the install path
			//InstallDir = installDir;
			//ImageLookup = null; // We'll want to reload this if something has changed.

			return installDir;
		}

		/// <summary>
		///  Looks for the game's V_CODE2 resource in <see cref="InstallDir"/>.
		/// </summary>
		/// <returns>The V_CODE2 string if found, otherwise null.</returns>
		/// 
		/// <exception cref="InvalidOperationException">
		///  <see cref="InstallDir"/> is not located.
		/// </exception>
		public string FindVCode2() {
			if (InstallDir == null)
				throw new InvalidOperationException($"No installation directory found for {this}!");
			return FindVCode2(InstallDir);
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
		public string FindVCode2(string installDir) {
			if (installDir == null)
				throw new ArgumentNullException(nameof(installDir));
			string exePath = Path.Combine(installDir, Executable);
			try {
				return Asmodean.VCode2.Find(exePath);
			} catch { }
			string binPath = Path.ChangeExtension(exePath, ".bin");
			if (File.Exists(binPath)) {
				try {
					return Asmodean.VCode2.Find(binPath);
				} catch { }
			}
			return null;
		}

		public void ClearLookups() {
			lookups.Clear();
		}
		public KifintLookup LoadLookup(KifintType type) {
			Trace.WriteLine($"Loading {type} Cache: {Id}");
			string lookupName = Path.ChangeExtension($"{Id}-{type.ToString().ToLower()}", ".intlookup");
			string lookupFile = Path.Combine(Database.GrisaiaDatabase.CachePath, lookupName);
			KifintLookup lookup = null;
			if (File.Exists(lookupFile)) {
				try {
					lookup = KifintLookup.Load(lookupFile, InstallDir);
				} catch { }
			}
			if (lookup == null) {
				Trace.WriteLine($"Building {type} Cache: {Id}");
				lookup = Kifint.Decrypt(type, InstallDir, VCode2);
				lookup.Save(lookupFile);
			}
			lookups[type] = lookup;
			return lookup;
		}

		#endregion
	}
}
