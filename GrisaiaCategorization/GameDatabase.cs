using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Grisaia {
	/// <summary>
	///  A game database storing all Grisaia games  along with their information.
	/// </summary>
	public sealed class GameDatabase {
		#region Fields

		/// <summary>
		///  The collection of Grisaia games mapped to an Id.
		/// </summary>
		[JsonIgnore]
		private readonly Dictionary<string, GameInfo> gameMap = new Dictionary<string, GameInfo>();
		/// <summary>
		///  The list of Grisaia games.
		/// </summary>
		[JsonIgnore]
		private IReadOnlyList<GameInfo> gameList = Array.AsReadOnly(new GameInfo[0]);

		#endregion

		#region Properties

		/// <summary>
		///  Gets the list of Grisaia games.
		/// </summary>
		[JsonProperty("games")]
		public IReadOnlyList<GameInfo> Games {
			get => gameList;
			private set {
				gameList = value;
				gameMap.Clear();
				foreach (GameInfo game in gameList)
					gameMap.Add(game.Id, game);
			}
		}

		/// <summary>
		///  Gets all located Grisaia games after <see cref="LocateGames"/> has been called.
		/// </summary>
		public IEnumerable<GameInfo> LocatedGames => gameList.Where(g => g.InstallDir != null);

		#endregion

		#region LocateGames

		/// <summary>
		///  Looks for all known Grisaia games' installation directories.
		/// </summary>
		public void LocateGames() {
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
				game.InstallDir = installDir;
			}
		}

		#endregion

		#region Accessors

		/// <summary>
		///  Gets the game info at the specified index in the list.
		/// </summary>
		/// <param name="index">The index of the game info to get.</param>
		/// <returns>The game info at the specified index in the list.</returns>
		public GameInfo GetGameAt(int index) {
			return gameList[index];
		}
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
		public GameInfo GetGame(string id) {
			return gameMap[id];
		}
		
		#endregion
	}
}
