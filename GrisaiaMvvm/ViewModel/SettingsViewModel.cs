using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Categories;
using Grisaia.Categories.Sprites;
using Grisaia.Mvvm.Model;
using Grisaia.Mvvm.Services;
using Grisaia.Utils;

namespace Grisaia.Mvvm.ViewModel {
	public sealed partial class SettingsViewModel : ViewModelWindow {
		#region Fields

		private CharacterNamingScheme characterNamingScheme;

		private GameNamingScheme gameNamingScheme;

		private ObservableArray<SpriteCategoryInfo> spritePrimaryCategories;
		private ObservableArray<SpriteCategoryInfo> spriteSecondaryCategories;

		private int selectedCharacterNameTypeIndex = -1;
		private int selectedSpritePrimaryCategoryIndex = -1;
		private int selectedSpriteSecondaryCategoryIndex = -1;

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

		/// <summary>
		///  Gets if naming games is enabled.
		/// </summary>
		public bool GameNamingEnabled => !gameNamingScheme.IdOnly;
		/// <summary>
		///  Gets if naming characters is enabled.
		/// </summary>
		public bool CharacterNamingEnabled => !characterNamingScheme.IdOnly;

		public int SelectedCharacterNameTypeIndex {
			get => selectedCharacterNameTypeIndex;
			set {
				if (Set(ref selectedCharacterNameTypeIndex, value)) {
					MoveCharacterNameTypeUp.RaiseCanExecuteChanged();
					MoveCharacterNameTypeDown.RaiseCanExecuteChanged();
				}
			}
		}
		public int SelectedSpritePrimaryCategoryIndex {
			get => selectedSpritePrimaryCategoryIndex;
			set {
				if (Set(ref selectedSpritePrimaryCategoryIndex, value)) {
					MoveSpritePrimaryCategoryUp.RaiseCanExecuteChanged();
					MoveSpritePrimaryCategoryDown.RaiseCanExecuteChanged();
				}
			}
		}
		public int SelectedSpriteSecondaryCategoryIndex {
			get => selectedSpriteSecondaryCategoryIndex;
			set {
				if (Set(ref selectedSpriteSecondaryCategoryIndex, value)) {
					MoveSpriteSecondaryCategoryUp.RaiseCanExecuteChanged();
					MoveSpriteSecondaryCategoryDown.RaiseCanExecuteChanged();
				}
			}
		}

		/// <summary>
		///  Gets or sets the naming scheme applied to character info names.
		/// </summary>
		/// 
		/// <exception cref="ArgumentNullException">
		///  value is null.
		/// </exception>
		public CharacterNamingScheme CharacterNamingScheme {
			get => characterNamingScheme;
			set {
				if (characterNamingScheme != null)
					characterNamingScheme.PropertyChanged -= OnCharacterNamingSchemePropertyChanged;
				if (value == null)
					throw new ArgumentNullException(nameof(CharacterNamingScheme));
				value.PropertyChanged += OnCharacterNamingSchemePropertyChanged;
				if (Set(ref characterNamingScheme, value))
					CharacterDatabase.NamingScheme = value;
			}
		}
		/// <summary>
		///  Gets or sets the naming scheme applied to game info names.
		/// </summary>
		/// 
		/// <exception cref="ArgumentNullException">
		///  value is null.
		/// </exception>
		public GameNamingScheme GameNamingScheme {
			get => gameNamingScheme;
			set {
				if (gameNamingScheme != null)
					gameNamingScheme.PropertyChanged -= OnGameNamingSchemePropertyChanged;
				if (value == null)
					throw new ArgumentNullException(nameof(GameNamingScheme));
				value.PropertyChanged += OnGameNamingSchemePropertyChanged;
				Set(ref gameNamingScheme, value);
			}
		}

		/// <summary>
		///  Gets the sprite categories order for use with character sprite selection.
		/// </summary>
		public ObservableArray<SpriteCategoryInfo> SpritePrimaryCategories {
			get => spritePrimaryCategories;
			set {
				if (value == null)
					throw new ArgumentNullException(nameof(SpritePrimaryCategories));
				Set(ref spritePrimaryCategories, value);
			}
		}

		/// <summary>
		///  Gets the sprite categories order for use with character sprite selection.
		/// </summary>
		public ObservableArray<SpriteCategoryInfo> SpriteSecondaryCategories {
			get => spriteSecondaryCategories;
			set {
				if (value == null)
					throw new ArgumentNullException(nameof(SpriteSecondaryCategories));
				Set(ref spriteSecondaryCategories, value);
			}
		}

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
			GameNamingScheme = Settings.GameNamingScheme.Clone();
			CharacterNamingScheme = Settings.CharacterNamingScheme.Clone();
			SpritePrimaryCategories = new ObservableArray<SpriteCategoryInfo>(Settings.SpriteCategoryOrder.Take(2));
			SpriteSecondaryCategories = new ObservableArray<SpriteCategoryInfo>(Settings.SpriteCategoryOrder.Skip(2));
		}

		#endregion

		#region Override Methods

		/// <summary>
		///  Called when the window loads the view model.
		/// </summary>
		public override void Loaded() {
			GameNamingScheme = Settings.GameNamingScheme.Clone();
			CharacterNamingScheme = Settings.CharacterNamingScheme.Clone();
			SpritePrimaryCategories = new ObservableArray<SpriteCategoryInfo>(Settings.SpriteCategoryOrder.Take(2));
			SpriteSecondaryCategories = new ObservableArray<SpriteCategoryInfo>(Settings.SpriteCategoryOrder.Skip(2));
			SelectedCharacterNameTypeIndex = -1;
			SelectedSpritePrimaryCategoryIndex = -1;
			SelectedSpriteSecondaryCategoryIndex = -1;
		}

		#endregion

		#region Event Handlers



		private void OnGameNamingSchemePropertyChanged(object sender, PropertyChangedEventArgs e) {
			switch (e.PropertyName) {
			case nameof(GameNamingScheme.IdOnly):
				RaisePropertyChanged(nameof(GameNamingEnabled));
				break;
			}
		}
		private void OnCharacterNamingSchemePropertyChanged(object sender, PropertyChangedEventArgs e) {
			switch (e.PropertyName) {
			case nameof(CharacterNamingScheme.IdOnly):
				RaisePropertyChanged(nameof(CharacterNamingEnabled));
				break;
			}
		}

		#endregion
	}
}
