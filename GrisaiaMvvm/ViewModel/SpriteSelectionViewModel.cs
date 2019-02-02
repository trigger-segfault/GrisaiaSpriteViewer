using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using Grisaia.Categories;
using Grisaia.Categories.Sprites;
using Grisaia.Mvvm;
using Grisaia.Mvvm.Commands;
using Grisaia.Mvvm.Model;
using Grisaia.Mvvm.Services;
using Grisaia.Utils;

namespace Grisaia.Mvvm.ViewModel {
	public class SpriteSelectionViewModel : ViewModelWindow {
		#region Constants

		/// <summary>
		///  The total number of part group part Ids.
		/// </summary>
		private const int PartCount = Grisaia.Categories.Sprites.SpriteSelection.PartCount;
		/// <summary>
		///  The value used to specify no part is selected.
		/// </summary>
		public const int NoPart = Grisaia.Categories.Sprites.SpriteSelection.NoPart;

		#endregion

		#region Fields
		
		//private ImmutableSpriteSelection selection = new ImmutableSpriteSelection();
		
		/// <summary>
		///  Gets the currently selected sprite parts.
		/// </summary>
		//private IReadOnlyList<ISpritePart> currentParts = Array.AsReadOnly(new ISpritePart[PartCount]);
		private IReadOnlyList<ISpritePartGroup> groups = Array.AsReadOnly(new ISpritePartGroup[PartCount]);

		private bool suppressCollectionEvents = false;

		private bool showGridLines = false;

		private bool centered = true;
		private double scale = 1.0;

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

		public SpriteImageViewModel SpriteImage { get; }

		/// <summary>
		///  Gets the categories used for selecting the next category in the sprite or the sprite parts.
		/// </summary>
		public ObservableArray<ISpriteCategory> Categories { get; }
			= new ObservableArray<ISpriteCategory>(SpriteCategoryPool.Count);
		/// <summary>
		///  Gets the categories used for selecting sprite part group parts.
		/// </summary>
		public IReadOnlyList<ISpritePartGroup> Groups {
			get => groups;
			private set => Set(ref groups, value);
		}
		/// <summary>
		///  Gets the selected sprite part group parts.
		/// </summary>
		public ObservableArray<ISpritePartGroupPart> GroupParts { get; }
			= new ObservableArray<ISpritePartGroupPart>(PartCount);

		/// <summary>
		///  Gets the game info for the current character sprite selection.
		/// </summary>
		public GameInfo CurrentGame {
			get {
				foreach (ISpriteCategory category in Categories) {
					if (category is ISpriteGame game)
						return game.GameInfo;
				}
				return null;
			}
		}
		/// <summary>
		///  Gets the character info for the current character sprite selection.
		/// </summary>
		public CharacterInfo CurrentCharacter {
			get {
				foreach (ISpriteCategory category in Categories) {
					if (category is ISpriteCharacter character)
						return character.CharacterInfo;
				}
				return null;
			}
		}
		/// <summary>
		///  Gets the currently selected sprite parts.
		/// </summary>
		/*public IReadOnlyList<ISpritePart> CurrentParts {
			get => currentParts;
			private set => Set(ref currentParts, value);
		}*/
		public IReadOnlyList<ISpritePart> CurrentParts => SpriteImage.CurrentParts;
		public bool ShowGridLines {
			get => showGridLines;
			set => Set(ref showGridLines, value);
		}
		/// <summary>
		///  Gets or sets if the image should be expanded to the total size of all sprites combined.
		/// </summary>
		public bool Expand {
			get => SpriteImage.Expand;
			set => SpriteImage.Expand = value;
		}
		public bool Centered {
			get => centered;
			set => Set(ref centered, value);
		}
		public double Scale {
			get => scale;
			set => Set(ref scale, value);
		}
		public Point SpriteOrigin => SpriteImage.SpriteOrigin;
		public Size SpriteSize => SpriteImage.SpriteSize;
		/// <summary>
		///  Gets the selection for the current character sprite.
		/// </summary>
		public IReadOnlySpriteSelection SpriteSelection {
			get => SpriteImage.SpriteSelection;
			set => SpriteImage.SpriteSelection = value;
		}

		public IRelayCommand ToggleCenterSprite => GetCommand(OnToggleCenterSprite);

		#endregion

		#region Constructors

		public SpriteSelectionViewModel(GrisaiaModel grisaiaDb,
										IGrisaiaDialogService dialogs,
										UIService ui)
		{
			Title = "Grisaia Extract Sprite Viewer";
			GrisaiaDatabase = grisaiaDb;
			Dialogs = dialogs;
			UI = ui;
			SpriteImage = new SpriteImageViewModel(SpriteDatabase);
			SpriteImage.PropertyChanged += OnSpriteImagePropertyChanged;

			Categories.CollectionChanged += OnCategoriesCollectionChanged;
			GroupParts.CollectionChanged += OnGroupPartsCollectionChanged;

			GameDatabase.NamingScheme = Settings.GameNamingScheme;
			CharacterDatabase.NamingScheme = Settings.CharacterNamingScheme;

			suppressCollectionEvents = true;
			if (IsInDesignMode) {
				GameDatabase.LocateDummyGames();
				SpriteDatabase.LoadSpritesDummy(Settings.SpriteCategoryOrder);
			}
			ISpriteSelection newSelection = SpriteSelection.ToMutable();
			ISpriteCategory category = SpriteDatabase;
			for (int i = 0; i < SpriteCategoryPool.Count; i++) {
				category = Categories[i] = (ISpriteCategory) category.List[0];
				category.Category.SetId(newSelection, category.Id);
				if (category.IsLastCategory) {
					UpdateGroups(category, newSelection);
				}
			}
			Console.WriteLine($"SpriteViewModel.CurrentParts+SpriteSelection");
			SpriteSelection = newSelection.ToImmutable();
			//CurrentParts = Array.AsReadOnly(SpriteDatabase.GetSpriteParts(newSelection, out _, out _));
			suppressCollectionEvents = false;
		}

		private void OnSpriteImagePropertyChanged(object sender, PropertyChangedEventArgs e) {
			switch (e.PropertyName) {
			case nameof(SpriteImageViewModel.Expand):
				RaisePropertyChanged(nameof(Expand));
				break;
			case nameof(SpriteImageViewModel.SpriteSelection):
				RaisePropertyChanged(nameof(SpriteSelection));
				break;
			case nameof(SpriteImageViewModel.SpriteOrigin):
				RaisePropertyChanged(nameof(SpriteOrigin));
				break;
			case nameof(SpriteImageViewModel.SpriteSize):
				RaisePropertyChanged(nameof(SpriteSize));
				break;
			case nameof(SpriteImageViewModel.CurrentParts):
				RaisePropertyChanged(nameof(CurrentParts));
				break;
			}
		}

		private void OnToggleCenterSprite() {
			Centered = !Centered;
		}

		#endregion

		#region Event Handlers

		private void OnCategoriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (suppressCollectionEvents) return;
			IList newItems = e.NewItems ?? Categories;
			if (e.Action == NotifyCollectionChangedAction.Replace && e.NewItems.Count == 1) {
				ISpriteCategory lastCategory = (ISpriteCategory) e.NewItems[0];
				if (lastCategory.IsLastCategory) {
					Console.WriteLine($"SpriteViewModel.OnCategoriesCollectionChanged");
					suppressCollectionEvents = true;
					ISpriteSelection newSelection = SpriteSelection.ToMutable();

					foreach (ISpriteCategory category in Categories) {
						category.Category.SetId(newSelection, category.Id);
						if (category.IsLastCategory) {
							UpdateGroups(category, newSelection);
						}
					}

					Console.WriteLine($"SpriteViewModel.CurrentParts+SpriteSelection");
					// Trigger update after we've raised the restriction against updates
					SpriteSelection = newSelection.ToImmutable();
					//CurrentParts = Array.AsReadOnly(SpriteDatabase.GetSpriteParts(newSelection, out _, out _));

					suppressCollectionEvents = false;
				}
			}
		}
		private void OnGroupPartsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (suppressCollectionEvents) return;
			suppressCollectionEvents = true;
			Console.WriteLine($"SpriteViewModel.OnGroupPartsCollectionChanged");
			ISpriteSelection newSelection = SpriteSelection.ToMutable();

			int startingIndex = e.NewStartingIndex;
			IList newItems = e.NewItems ?? GroupParts;
			for (int i = 0; i < newItems.Count; i++) {
				int groupIndex = startingIndex + i;
				ISpritePartGroupPart groupPart = (ISpritePartGroupPart) newItems[i];
				newSelection.GroupPartIds[groupIndex] = groupPart?.Id ?? -1;
			}

			Console.WriteLine($"SpriteViewModel.CurrentParts+SpriteSelection");
			// Trigger update after we've raised the restriction against updates
			SpriteSelection = newSelection.ToImmutable();
			//CurrentParts = Array.AsReadOnly(SpriteDatabase.GetSpriteParts(newSelection, out _, out _));

			suppressCollectionEvents = false;
		}

		#endregion

		private void UpdateGroups(ISpriteCategory category, ISpriteSelection newSelection) {
			Console.WriteLine($"SpriteViewModel.UpdateGroups");
			List<ISpritePartGroup> oldGroups = new List<ISpritePartGroup>(Groups);
			List<ISpritePartGroupPart> groupParts = new List<ISpritePartGroupPart>(GroupParts);
			ISpritePartGroup[] newGroups = category.CreateGroups(CurrentGame, CurrentCharacter);
			
			int i = 0;
			foreach (ISpritePartGroup newGroup in newGroups) {
				// Check if an old group with the same name already has a valid selection
				int groupIndex = oldGroups.FindIndex(g => g?.Name == newGroup.Name);
				ISpritePartGroupPart newGroupPart, groupPart = null;
				if (groupIndex != -1) {
					groupPart = groupParts[groupIndex];
					oldGroups.RemoveAt(groupIndex);
					groupParts.RemoveAt(groupIndex);
				}
				if (groupPart != null && newGroup.TryGetValue(groupPart.Id, out newGroupPart)) {
					// Do nothing, we've captured newGroupPart already
				}
				else if (newGroup.IsEnabledByDefault && newGroup.Count > 1) {
					newGroupPart = newGroup.GroupParts[1];
				}
				else {
					newGroupPart = newGroup.GroupParts[0];
				}
				newSelection.GroupPartIds[i] = newGroupPart.Id;
				i++;
			}
			for (; i < PartCount; i++) {
				newSelection.GroupPartIds[i] = NoPart;
			}
			Array.Resize(ref newGroups, PartCount);
			Groups = Array.AsReadOnly(newGroups);
		}
	}
}
