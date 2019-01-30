using System;
using System.IO;
using GalaSoft.MvvmLight;

namespace Grisaia.Categories {
	public class GrisaiaDatabase {
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
		///  The path to the dummy data.
		/// </summary>
		public string DummyPath { get; }
		
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
			if (ViewModelBase.IsInDesignModeStatic)
				baseDirectory = DummyCategorizationContext.BaseDirectory;
			CachePath = Path.Combine(baseDirectory, "cache");
			DataPath = Path.Combine(baseDirectory, "data");
			SavedPath = Path.Combine(baseDirectory, "saved");
			DummyPath = Path.Combine(DummyCategorizationContext.BaseDirectory, "data", "dummy");

			string gamePath = Path.Combine(DataPath, "games.json");
			string charactersPath = Path.Combine(DataPath, "characters.json");

			GameDatabase = GameDatabase.FromJsonFile(gamePath, this);
			CharacterDatabase = CharacterDatabase.FromJsonFile(charactersPath, this);
			SpriteDatabase = new SpriteDatabase(this);
		}

		#endregion
	}
}
