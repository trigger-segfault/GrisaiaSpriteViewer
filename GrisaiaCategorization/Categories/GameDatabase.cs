using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Grisaia.Asmodean;
using Grisaia.Locators;
using Newtonsoft.Json;

namespace Grisaia.Categories {
	/// <summary>
	///  A game database storing all Grisaia games  along with their information.
	/// </summary>
	[JsonObject]
	public sealed class GameDatabase : IReadOnlyCollection<GameInfo> {
		#region Fields

		/// <summary>
		///  Gets the grisaia database containing this database.
		/// </summary>
		[JsonIgnore]
		public GrisaiaDatabase GrisaiaDatabase { get; private set; }
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
		/*/// <summary>
		///  The path leading to where cached .intlookup files are stored.
		/// </summary>
		[JsonIgnore]
		private string cachePath = Path.Combine(AppContext.BaseDirectory, "cache");*/
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
			set => namingScheme = value ?? throw new ArgumentNullException(nameof(NamingScheme));
		}
		/*/// <summary>
		///  Gets or sets the path leading to where cached .intlookup files are stored.
		/// </summary>
		[JsonIgnore]
		public string CachePath {
			get => cachePath;
			set {
				if (value == null)
					throw new ArgumentNullException(nameof(CachePath));
				if (string.IsNullOrWhiteSpace(value) || !PathUtils.IsValidPath(value))
					throw new ArgumentException($"{nameof(CachePath)} is invalid!");
				cachePath = value;
			}
		}*/
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
			return locatedGameList.Count > 0;
		}
		/// <summary>
		///  Looks for all known Grisaia games' installation directories.
		/// </summary>
		/// <returns>True if any Grisaia games were found.</returns>
		public bool LocateGames() {
			locatedGameList.Clear();
			SteamLibraryFolders steamLibraries = null;
			try {
				steamLibraries = SteamLocator.FindLibrariesFromRegistry();
			} catch (SteamException) { }
			foreach (GameInfo game in gameList) {
				if (game.LoadGame(steamLibraries))
					locatedGameList.Add(game);
				/*string installDir = null;
				if (installDir == null && game.SteamId.HasValue && steamLibraries != null) {
					installDir = SteamLocator.LocateGame(steamLibraries, game.SteamId.Value);
				}
				if (installDir == null && game.FrontwingRegistryValue != null) {
					installDir = FrontwingLocator.LocateGame(game.FrontwingRegistryValue);
				}
				// We should still set this to null if we lost the install path
				game.InstallDir = installDir;
				game.ImageLookup = null; // We'll want to reload this if something has changed.
				if (installDir != null) {
					locatedGameList.Add(game);
				}*/
			}
			return locatedGameList.Count > 0;
		}

		#endregion

		#region Accessors

		/*/// <summary>
		///  Gets the game info at the specified index in the list.
		/// </summary>
		/// <param name="index">The index of the game info to get.</param>
		/// <returns>The game info at the specified index in the list.</returns>
		/// 
		/// <exception cref="IndexOutOfRangeException">
		///  <paramref name="index"/> the index was outside the bounds of the list.
		/// </exception>
		public GameInfo At(int index) {
			return gameList[index];
		}*/
		/// <summary>
		///  Gets the game info with the specified Id.
		/// </summary>
		/// <param name="id">The Id of the game.</param>
		/// <returns>The game info for the game with the specified Id.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="id"/> is null.
		/// </exception>
		/// 
		/// <exception cref="KeyNotFoundException">
		///  The game with the <paramref name="id"/> was not found.
		/// </exception>
		public GameInfo Get(string id) => gameMap[id];
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

		public void ClearLookupCache() {
			string cachePath = GrisaiaDatabase.CachePath;
			if (Directory.Exists(cachePath))
				Directory.Delete(cachePath, true);

			foreach (GameInfo game in locatedGameList) {
				game.ClearLookups();
			}
		}

		public void RebuildLookupCache(bool loadUpdateArchives = true) {
			ClearLookupCache();
			LoadLookupCache(loadUpdateArchives);
		}

		public void LoadLookupCache(bool loadUpdateArchives = true) {
			string cachePath = GrisaiaDatabase.CachePath;
			if (!Directory.Exists(cachePath))
				Directory.CreateDirectory(cachePath);
			
			foreach (GameInfo game in locatedGameList) {
				game.ClearLookups();
				if (loadUpdateArchives)
					game.LoadLookup(KifintType.Update);
				game.LoadLookup(KifintType.Image);
				//game.ImageLookup = LoadLookup(KifintType.Image, cachePath, game);
				//game.UpdateLookup = LoadLookup(KifintType.Update, cachePath, game);
				//game.ImageLookup.Update = game.UpdateLookup;
			}
		}

		/*private KifintLookup LoadLookup(KifintType type, string cachePath, GameInfo game) {
			Trace.WriteLine($"Loading {type} Cache: {game.Id}");
			string lookupFile = GetLookupFile(type, cachePath, game);
			KifintLookup lookup = null;
			if (File.Exists(lookupFile)) {
				try {
					lookup = KifintLookup.Load(lookupFile, game.InstallDir);
					//Trace.WriteLine($"Loaded {type} Cache: {Path.GetFileName(lookupFile)}");
				} catch { }
			}
			if (lookup == null) {
				Trace.WriteLine($"Building {type} Cache: {game.Id}");
				lookup = Kifint.Decrypt(type, game.InstallDir, game.VCode2);
				lookup.Save(lookupFile);
				//Trace.WriteLine($"Saved {type} Cache: {Path.GetFileName(lookupFile)}");
			}
			return lookup;
		}

		private string GetLookupFile(KifintType type, string cachePath, GameInfo game) {
			string name = $"{game.Id}-{type.ToString().ToLower()}";
			return Path.Combine(cachePath, Path.ChangeExtension(name, KifintLookup.Extension));
		}*/

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
