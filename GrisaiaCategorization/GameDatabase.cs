using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Grisaia.Asmodean;
using Newtonsoft.Json;

namespace Grisaia {
	/// <summary>
	///  A game database storing all Grisaia games  along with their information.
	/// </summary>
	public sealed class GameDatabase {
		#region Fields

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
		///  The path leading to where cached .intlookup files are stored.
		/// </summary>
		private string cachePath = Path.Combine(AppContext.BaseDirectory, "cache");

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
		///  Gets or sets the path leading to where cached .intlookup files are stored.
		/// </summary>
		public string CachePath {
			get => cachePath;
			set {
				if (value == null)
					throw new ArgumentNullException(nameof(CachePath));
				if (string.IsNullOrWhiteSpace(value) || !PathHelper.IsValidPath(value))
					throw new ArgumentException($"{nameof(CachePath)} is invalid!");
				cachePath = value;
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
			get => gameList;
			private set {
				gameList.Clear();
				gameMap.Clear();
				gameList.AddRange(value);
				foreach (GameInfo game in gameList)
					gameMap.Add(game.Id, game);
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
		///  Looks for all known Grisaia games' installation directories.
		/// </summary>
		public void LocateGames() {
			locatedGameList.Clear();
			SteamLibraryFolders steamLibraries = null;
			try {
				steamLibraries = SteamLocator.FindLibrariesFromRegistry();
			} catch (SteamException) { }
			foreach (GameInfo game in gameList) {
				string installDir = null;
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
				}
			}
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
		/// <exception cref="KeyNotFoundException">
		///  No game with the key '<paramref name="id"/>' was found.
		/// </exception>
		public GameInfo Get(string id) {
			return gameMap[id];
		}
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
			string cachePath = this.cachePath;
			if (Directory.Exists(cachePath)) {
				foreach (var game in gameList) {
					string lookupFile = GetLookupFile(cachePath, game);
					if (File.Exists(lookupFile)) {
						File.Delete(lookupFile);
						Trace.WriteLine($"Deleted: {Path.GetFileName(lookupFile)}");
					}
				}
			}
		}

		public void LoadCache() {
			string cachePath = this.cachePath;
			if (!Directory.Exists(cachePath))
				Directory.CreateDirectory(cachePath);
			
			for (int i = 0; i < LocatedCount; i++) {
				var game = LocatedGames[i];
				Trace.WriteLine($"Loading Cache: {game.Id}");
				string lookupFile = GetLookupFile(cachePath, game);
				if (File.Exists(lookupFile)) {
					try {
						game.ImageLookup = KifintLookup.Load(lookupFile, game.InstallDir);
						Trace.WriteLine($"Loaded: {Path.GetFileName(lookupFile)}");
					} catch (Exception) {
						Trace.WriteLine($"Building Cache: {game.Id}");
						game.ImageLookup = Kifint.DecryptImages(game.InstallDir, game.Executable);
						game.ImageLookup.Save(lookupFile);
						Trace.WriteLine($"Saved: {Path.GetFileName(lookupFile)}");
					}
				}
				else {
					Trace.WriteLine($"Building Cache: {game.Id}");
					game.ImageLookup = Kifint.DecryptImages(game.InstallDir, game.Executable);
					game.ImageLookup.Save(lookupFile);
					Trace.WriteLine($"Saved: {Path.GetFileName(lookupFile)}");
				}
			}
		}

		private string GetLookupFile(string cachePath, GameInfo game) {
			return Path.Combine(cachePath, Path.ChangeExtension(game.Id, KifintLookup.Extension));
		}

		#endregion
	}
}
