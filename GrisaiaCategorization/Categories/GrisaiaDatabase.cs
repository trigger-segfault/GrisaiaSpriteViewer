using System;
using System.IO;
using GalaSoft.MvvmLight;
using Grisaia.Utils;
using Newtonsoft.Json;

namespace Grisaia.Categories {
	public class GrisaiaDatabase : ObservableObject {
		#region Fields

		/// <summary>
		///  Gets the path leading to where cached .intlookup files and images are stored.
		/// </summary>
		public string CachePath { get; }
		/// <summary>
		///  Gets the path leading to where json database files are stored.
		/// </summary>
		public string DataPath { get; }
		/// <summary>
		///  Gets the path leading to where saved sprites are stored.
		/// </summary>
		public string SavedPath { get; }
		
		/// <summary>
		///  Gets the database of games.
		/// </summary>
		public GameDatabase GameDatabase { get; }
		/// <summary>
		///  Gets the database of characters.
		/// </summary>
		public CharacterDatabase CharacterDatabase { get; }
		/// <summary>
		///  Gets the database of loaded sprites.
		/// </summary>
		public SpriteDatabase SpriteDatabase { get; }

		#endregion

		#region Constructors

		public GrisaiaDatabase() {
			string baseDirectory = AppContext.BaseDirectory;
			CachePath = Path.Combine(baseDirectory, "cache");
			DataPath = Path.Combine(baseDirectory, "data");
			SavedPath = Path.Combine(baseDirectory, "saved");

			if (ViewModelBase.IsInDesignModeStatic) {
				string gameJson = Embedded.ReadAllText(Embedded.Combine("Grisaia.data", "games.json"));
				string charactersJson = Embedded.ReadAllText(Embedded.Combine("Grisaia.data", "characters.json"));
				GameDatabase = JsonConvert.DeserializeObject<GameDatabase>(gameJson);
				CharacterDatabase = JsonConvert.DeserializeObject<CharacterDatabase>(charactersJson);
				GameDatabase.GrisaiaDatabase = this;
				CharacterDatabase.GrisaiaDatabase = this;
			}
			else {
				string gamePath = Path.Combine(DataPath, "games.json");
				string charactersPath = Path.Combine(DataPath, "characters.json");

				GameDatabase = GameDatabase.FromJsonFile(gamePath, this);
				CharacterDatabase = CharacterDatabase.FromJsonFile(charactersPath, this);
			}
			SpriteDatabase = new SpriteDatabase(this);
		}

		#endregion
	}
}
