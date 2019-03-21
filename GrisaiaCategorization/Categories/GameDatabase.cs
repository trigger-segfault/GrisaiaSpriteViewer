using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight;
using Grisaia.Locators;
using Newtonsoft.Json;
using TriggersTools.CatSystem2;

namespace Grisaia.Categories {
	/// <summary>
	///  The event args used with <see cref="LoadCacheProgressCallback"/>.
	/// </summary>
	public struct LoadCacheProgressArgs {
		/// <summary>
		///  The current game whose KIFINT archives are being loaded.
		/// </summary>
		public GameInfo CurrentGame { get; internal set; }
		/// <summary>
		///  The index of the current located game being parsed.
		/// </summary>
		public int GameIndex { get; internal set; }
		/// <summary>
		///  The total number of located games to parse.
		/// </summary>
		public int GameCount { get; internal set; }
		/// <summary>
		///  Gets if the KIFINT archives are being built instead of loaded.
		/// </summary>
		public bool IsBuilding { get; internal set; }

		/// <summary>
		///  Gets the progress made on KIFINT archive decryption.
		/// </summary>
		public KifintProgressArgs Kifint { get; internal set; }

		/// <summary>
		///  Gets the progress made on the loading of all game caches.
		/// </summary>
		public double Progress {
			get {
				if (GameIndex == GameCount)
					return 1d;
				return (double) GameIndex / GameCount;
			}
		}
		/// <summary>
		///  Gets the minor progress being made on a single game.
		/// </summary>
		public double MinorProgress => Kifint.Progress;
		/// <summary>
		///  Gets the major progress being made on all games.
		/// </summary>
		public double MajorProgress {
			get => (GameIndex == GameCount ? 1d : ((double) GameIndex / GameCount));
		}
		/// <summary>
		///  Gets if the progress is completely finished.
		/// </summary>
		public bool IsDone => GameIndex == GameCount;
	}
	/// <summary>
	///  An event handler for use during the locating of a Grisia games.
	/// </summary>
	/// <param name="e">The progress event args.</param>
	public delegate void LoadCacheProgressCallback(LoadCacheProgressArgs e);
	/// <summary>
	///  A game database storing all Grisaia games  along with their information.
	/// </summary>
	[JsonObject]
	public sealed class GameDatabase : ObservableObject, IReadOnlyCollection<GameInfo> {
		#region Fields

		/// <summary>
		///  Gets the grisaia database containing this database.
		/// </summary>
		[JsonIgnore]
		public GrisaiaDatabase GrisaiaDatabase { get; internal set; }
		/// <summary>
		///  The collection of Grisaia game infos mapped to an Id.
		/// </summary>
		[JsonIgnore]
		private readonly Dictionary<string, GameInfo> gameMap = new Dictionary<string, GameInfo>();
		/// <summary>
		///  The mutable list of Grisaia game infos.
		/// </summary>
		[JsonIgnore]
		internal readonly List<GameInfo> gameList = new List<GameInfo>();
		/// <summary>
		///  The readonly copy of the Grisaia game list.<para/>
		///  This is required because Newtonsoft.Json would write to the list otherwise.
		/// </summary>
		[JsonIgnore]
		private readonly IReadOnlyList<GameInfo> readonlyList;
		/// <summary>
		///  The mutable list of located Grisaia game infos.
		/// </summary>
		[JsonIgnore]
		internal readonly List<GameInfo> locatedGameList = new List<GameInfo>();
		/// <summary>
		///  The readonly copy of the located Grisaia game list.
		/// </summary>
		[JsonIgnore]
		private readonly IReadOnlyList<GameInfo> locatedReadonlyList;
		/// <summary>
		///  The naming scheme used for games.
		/// </summary>
		[JsonIgnore]
		private GameNamingScheme namingScheme = new GameNamingScheme();

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs the game database and sets up the readonly lists.
		/// </summary>
		public GameDatabase() {
			readonlyList = gameList.AsReadOnly();
			locatedReadonlyList = locatedGameList.AsReadOnly();
		}

		#endregion

		#region Properties

		/// <summary>
		///  Gets or sets the naming scheme used for games.
		/// </summary>
		[JsonIgnore]
		public GameNamingScheme NamingScheme {
			get => namingScheme;
			set {
				if (value == null)
					throw new ArgumentNullException(nameof(NamingScheme));
				Set(ref namingScheme, value);
			}
		}
		/// <summary>
		///  Gets the number of total Grisaia games in the database.
		/// </summary>
		[JsonIgnore]
		public int Count => gameList.Count;
		/// <summary>
		///  Gets the number of located Grisaia games in the database.
		/// </summary>
		[JsonIgnore]
		public int LocatedCount => locatedGameList.Count;
		/// <summary>
		///  Gets the list of Grisaia games.
		/// </summary>
		[JsonProperty("games")]
		public IReadOnlyList<GameInfo> Games {
			get => readonlyList;
			private set {
				gameList.Clear();
				gameMap.Clear();
				gameList.AddRange(value);
				foreach (GameInfo game in gameList) {
					game.Database = this;
					gameMap.Add(game.Id, game);
				}
			}
		}
		/// <summary>
		///  Gets all located Grisaia games after <see cref="LocateGames"/> has been called.
		/// </summary>
		[JsonIgnore]
		public IReadOnlyList<GameInfo> LocatedGames => locatedReadonlyList;
		/// <summary>
		///  Gets the game info at the specified index in the list.
		/// </summary>
		/// <param name="index">The index of the game info to get.</param>
		/// <returns>The game info at the specified index in the list.</returns>
		/// 
		/// <exception cref="IndexOutOfRangeException">
		///  <paramref name="index"/> the index was outside the bounds of the list.
		/// </exception>
		[JsonIgnore]
		public GameInfo this[int index] => gameList[index];
		/// <summary>
		///  Gets the game info with the specified Id.
		/// </summary>
		/// <param name="id">The Id of the game.</param>
		/// <returns>The game info for the game with the specified Id.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="id"/> is null.
		/// </exception>
		/// <exception cref="KeyNotFoundException">
		///  The game with the <paramref name="id"/> was not found.
		/// </exception>
		[JsonIgnore]
		public GameInfo this[string id] {
			get {
				if (id == null)
					throw new ArgumentNullException(nameof(id));
				GameInfo game = gameList.Find(e => e.Id.Equals(id));
				return game ?? throw new KeyNotFoundException($"Could not find the key \"{id}\"!");
			}
		}

		#endregion

		#region LocateGames

		/// <summary>
		///  Adds dummy known game entries.
		/// </summary>
		/// <returns>Always true.</returns>
		public bool LocateDummyGames() {
			locatedGameList.Clear();
			foreach (GameInfo game in gameList.Take(1)) {
				if (game.LoadDummyGame())
					locatedGameList.Add(game);
			}
			RaisePropertyChanged(nameof(LocatedGames));
			RaisePropertyChanged(nameof(LocatedCount));
			return locatedGameList.Count > 0;
		}
		/// <summary>
		///  Looks for all known Grisaia games' installation directories.
		/// </summary>
		/// <param name="customInstalls">The collection of custom install locations by game Id.</param>
		/// <returns>True if any Grisaia games were found.</returns>
		public bool LocateGames(IReadOnlyDictionary<string, GameInstallInfo> customInstalls) {
			locatedGameList.Clear();
			SteamLibraryFolders steamLibraries = null;
			try {
				steamLibraries = SteamLocator.FindLibrariesFromRegistry();
			} catch (SteamException) { }
			foreach (GameInfo game in gameList) {
				GameInstallInfo customInstall = GameInstallInfo.None;
				customInstalls?.TryGetValue(game.Id, out customInstall);
				if (game.LoadGame(steamLibraries, customInstall))
					locatedGameList.Add(game);
			}
			RaisePropertyChanged(nameof(LocatedGames));
			RaisePropertyChanged(nameof(LocatedCount));
			return locatedGameList.Count > 0;
		}

		public bool RelocateGames(IReadOnlyDictionary<string, GameInstallInfo> customInstalls) {
			var oldLocatedGames = locatedGameList.ToList();
			locatedGameList.Clear();
			SteamLibraryFolders steamLibraries = null;
			bool anyCacheNeedsReload = false;
			try {
				steamLibraries = SteamLocator.FindLibrariesFromRegistry();
			} catch (SteamException) { }
			foreach (GameInfo game in gameList) {
				GameInstallInfo customInstall = GameInstallInfo.None;
				customInstalls?.TryGetValue(game.Id, out customInstall);
				if (game.ReloadGame(steamLibraries, customInstall, out bool cacheNeedsReload)) {
					locatedGameList.Add(game);
					if (cacheNeedsReload && !oldLocatedGames.Contains(game))
						anyCacheNeedsReload = true;
				}
				else if (oldLocatedGames.Contains(game)) {
					anyCacheNeedsReload = true;
				}
			}
			RaisePropertyChanged(nameof(LocatedGames));
			RaisePropertyChanged(nameof(LocatedCount));
			return anyCacheNeedsReload;
		}

		#endregion

		#region Accessors

		/// <summary>
		///  Tries to get the game info with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the game info to get.</param>
		/// <param name="value">The output game info if one was found, otherwise null.</param>
		/// <returns>True if an game info with the Id was found, otherwise null.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="id"/> is null.
		/// </exception>
		public bool TryGetValue(string id, out GameInfo game) => gameMap.TryGetValue(id, out game);
		/// <summary>
		///  Gets if the category contains an game info with the specified Id.
		/// </summary>
		/// <param name="id">The Id to check for an game info with.</param>
		/// <returns>True if an game info exists with the specified Id, otherwise null.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="id"/> is null.
		/// </exception>
		public bool ContainsKey(string id) => gameMap.ContainsKey(id);
		/// <summary>
		///  Searches for the index of the game info in the list.
		/// </summary>
		/// <param name="game">The game to look for.</param>
		/// <returns>The index of the game if it was found, otherwise -1.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="game"/> is null.
		/// </exception>
		public int IndexOf(GameInfo game) {
			if (game == null)
				throw new ArgumentNullException(nameof(game));
			return gameList.IndexOf(game);
		}
		/// <summary>
		///  Searches for the index of the game info with the specified Id in the list.
		/// </summary>
		/// <param name="id">The Id of the game to look for.</param>
		/// <returns>The index of the game if it was found, otherwise -1.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="id"/> is null.
		/// </exception>
		public int IndexOf(string id) {
			if (id == null)
				throw new ArgumentNullException(nameof(id));
			return gameList.FindIndex(g => g.Id == id);
		}

		#endregion

		#region Cache

		public void ClearCache() {
			string cachePath = GrisaiaDatabase.CachePath;
			if (Directory.Exists(cachePath))
				Directory.Delete(cachePath, true);

			foreach (GameInfo game in locatedGameList) {
				game.ClearLookups();
			}
		}

		public void RebuildCache(bool loadUpdateArchives = true, LoadCacheProgressCallback callback = null) {
			ClearCache();
			LoadCache(loadUpdateArchives, callback);
		}

		public void ReloadCache(bool loadUpdateArchives = true, LoadCacheProgressCallback callback = null) {
			string cachePath = GrisaiaDatabase.CachePath;
			if (!Directory.Exists(cachePath))
				Directory.CreateDirectory(cachePath);

			LoadCacheProgressArgs progress = new LoadCacheProgressArgs {
				GameIndex = 0,
				GameCount = LocatedCount,
			};

			foreach (GameInfo game in locatedGameList) {
				progress.CurrentGame = game;
				if (game.Lookups.Count == 0) {
					if (loadUpdateArchives)
						game.LoadLookup(KifintType.Update, progress, callback);
					game.LoadLookup(KifintType.Image, progress, callback);
				}
				progress.GameIndex++;
			}
			progress.CurrentGame = null;
			progress.Kifint = default;
			callback?.Invoke(progress);
		}
		public void LoadCache(bool loadUpdateArchives = true, LoadCacheProgressCallback callback = null) {
			/*string cachePath = GrisaiaDatabase.CachePath;
			if (!Directory.Exists(cachePath))
				Directory.CreateDirectory(cachePath);

			LoadCacheProgressArgs progress = new LoadCacheProgressArgs {
				GameIndex = 0,
				GameCount = LocatedCount,
			};
			
			foreach (GameInfo game in locatedGameList) {
				progress.CurrentGame = game;
				game.ClearLookups();
				if (loadUpdateArchives)
					game.LoadLookup(KifintType.Update, progress, callback);
				game.LoadLookup(KifintType.Image, progress, callback);
				progress.GameIndex++;
			}
			progress.CurrentGame = null;
			progress.Kifint = default;
			callback?.Invoke(progress);*/
			List<KifintType> types = new List<KifintType> {
				KifintType.Image,
			};
			if (loadUpdateArchives)
				types.Add(KifintType.Update);
			LoadCache(callback, types.ToArray());
		}
		public void LoadCache(LoadCacheProgressCallback callback, params KifintType[] types) {
			string cachePath = GrisaiaDatabase.CachePath;
			if (!Directory.Exists(cachePath))
				Directory.CreateDirectory(cachePath);

			LoadCacheProgressArgs progress = new LoadCacheProgressArgs {
				GameIndex = 0,
				GameCount = LocatedCount,
			};

			foreach (GameInfo game in locatedGameList) {
				progress.CurrentGame = game;
				game.ClearLookups();
				foreach (KifintType type in types) {
					game.LoadLookup(type, progress, callback);
				}
				progress.GameIndex++;
			}
			progress.CurrentGame = null;
			progress.Kifint = default;
			callback?.Invoke(progress);
		}

		#endregion

		#region I/O

		/// <summary>
		///  Deserializes the game database from a json file.
		/// </summary>
		/// <param name="jsonFile">The path to the json file to load and deserialize.</param>
		/// <param name="grisaiaDb">The grisaia database containing all databases.</param>
		/// <returns>The deserialized game database.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="jsonFile"/> is null.
		/// </exception>
		public static GameDatabase FromJsonFile(string jsonFile, GrisaiaDatabase grisaiaDb) {
			if (jsonFile == null)
				throw new ArgumentNullException(nameof(jsonFile));
			var db = JsonConvert.DeserializeObject<GameDatabase>(File.ReadAllText(jsonFile));
			db.GrisaiaDatabase = grisaiaDb ?? throw new ArgumentNullException(nameof(grisaiaDb));
			return db;
		}

		#endregion

		#region IEnumerable Implementation

		/// <summary>
		///  Gets the enumerator for all game infos in the game database.
		/// </summary>
		/// <returns>The enumerator for all game infos.</returns>
		public IEnumerator<GameInfo> GetEnumerator() => gameList.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion
	}
}
