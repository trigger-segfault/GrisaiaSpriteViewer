using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Grisaia.Categories;
using Grisaia.Categories.Sprites;
using Grisaia.SpriteViewer.Model;

namespace Grisaia.SpriteViewer.ViewModel {
	public class LoadingViewModel : ViewModelBase {
		#region Fields


		#endregion

		#region Properties

		/// <summary>
		///  Gets the database for all Grisaia databases.
		/// </summary>
		public GrisaiaModel GrisaiaDatabase { get; }
		/// <summary>
		///  Gets the database for all Grisaia games.
		/// </summary>
		public GameDatabase GameDatabase => GrisaiaDatabase.GameDatabase;
		/// <summary>
		///  Gets the database for all known Grisaia characters.
		/// </summary>
		public CharacterDatabase CharacterDatabase => GrisaiaDatabase.CharacterDatabase;
		/// <summary>
		///  Gets the database for all located character sprites.
		/// </summary>
		public SpriteDatabase SpriteDatabase => GrisaiaDatabase.SpriteDatabase;
		/// <summary>
		///  Gets the program settings
		/// </summary>
		public SpriteViewerSettings Settings => GrisaiaDatabase.Settings;

		#endregion

		#region Constructors

		public LoadingViewModel(GrisaiaModel grisaiaDb) {
			GrisaiaDatabase = grisaiaDb;
			//GameDatabase.LocateGames();
			//GameDatabase.LoadLookupCache(Settings.IncludeUpdateArchives);
			//spriteDb.Build(Settings.SpriteCategoryOrder);
		}

		#endregion



	}
}
