using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Categories;
using Grisaia.Mvvm.Model;
using Grisaia.Mvvm.Services;

namespace Grisaia.Mvvm.ViewModel {
	public class SettingsViewModel : ViewModelWindow {
		#region Fields

		private CharacterNamingScheme characterNamingScheme;

		private GameNamingScheme gameNamingScheme;

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

		public IGrisaiaDialogService Dialogs { get; }
		public UIService UI { get; }

		public bool CharacterNamingEnabled => !characterNamingScheme.IdOnly;
		public bool GameNamingEnabled => !gameNamingScheme.IdOnly;


		#endregion

		#region Constructors

		public SettingsViewModel(IRelayCommandFactory relayFactory,
								 GrisaiaModel grisaiaDb,
								 IGrisaiaDialogService dialogs,
								 UIService ui)
			: base(relayFactory)
		{
			Title = "Grisaia Extract Sprite Viewer - Settings";
			GrisaiaDatabase = grisaiaDb;
			Dialogs = dialogs;
			UI = ui;
		}

		#endregion
	}
}
