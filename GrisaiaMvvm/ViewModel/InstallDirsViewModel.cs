using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Categories;
using Grisaia.Mvvm.Model;
using Grisaia.Mvvm.Services;

namespace Grisaia.Mvvm.ViewModel {
	public sealed partial class InstallDirsViewModel : ViewModelWindow {
		#region Fields

		private IReadOnlyList<InstallDirsGameViewModel> games;

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

		private readonly IRelayCommandFactory relayFactory;
		public IGrisaiaDialogService Dialogs { get; }
		public UIService UI { get; }
		/// <summary>
		///  Gets the list of games and their custom install view models.
		/// </summary>
		public IReadOnlyList<InstallDirsGameViewModel> Games {
			get => games;
			set => Set(ref games, value);
		}

		#endregion


		#region Constructors
		
		public InstallDirsViewModel(IRelayCommandFactory relayFactory,
									GrisaiaModel grisaiaDb,
									IGrisaiaDialogService dialogs,
									UIService ui)
			: base(relayFactory)
		{
			Title = "Grisaia Extract Sprite Viewer - Game Locations";
			GrisaiaDatabase = grisaiaDb;
			UI = ui;
			Dialogs = dialogs;
			this.relayFactory = relayFactory;
			var games = GameDatabase.Games.Select(g => new InstallDirsGameViewModel(relayFactory, this, g));
			Games = Array.AsReadOnly(games.ToArray());
		}

		#endregion

		public override void Loaded() {
			var games = GameDatabase.Games.Select(g => new InstallDirsGameViewModel(relayFactory, this, g));
			Games = Array.AsReadOnly(games.ToArray());
		}
	}
}
