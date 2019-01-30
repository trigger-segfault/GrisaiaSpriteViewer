using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Grisaia.Categories;
using Grisaia.Categories.Sprites;
using Grisaia.SpriteViewer.Model;
using Grisaia.Utils;

namespace Grisaia.SpriteViewer.ViewModel {
	public class SpriteViewModel : ViewModelBase {
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
		
		private ImmutableSpriteSelection selection = new ImmutableSpriteSelection();
		
		//private GameInfo currentGame;
		//private CharacterInfo currentCharacter;
		/// <summary>
		///  Gets the currently selected sprite parts.
		/// </summary>
		private IReadOnlyList<ISpritePart> currentParts = Array.AsReadOnly(new ISpritePart[PartCount]);
		private IReadOnlyList<ISpritePartGroup> groups = Array.AsReadOnly(new ISpritePartGroup[PartCount]);
		//private IReadOnlyList<ISpritePartGroupPart> groupParts = Array.AsReadOnly(new ISpritePartGroupPart[PartCount]);
		//private IReadOnlyList<CharacterSpritePartGroupInfo> currentGroups;

		private bool suppressCollectionEvents;

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

		/// <summary>
		///  Gets the categories used for selecting the next category in the sprite or the sprite parts.
		/// </summary>
		public ObservableArray<ISpriteCategory> Categories { get; }
			= new ObservableArray<ISpriteCategory>(SpriteCategoryPool.Count);
		/// <summary>
		///  Gets the categories used for selecting sprite part group parts.
		/// </summary>
		//public ObservableArray<ISpritePartGroup> Groups { get; }
		//	= new ObservableArray<ISpritePartGroup>(PartCount);
		public IReadOnlyList<ISpritePartGroup> Groups {
			get => groups;
			private set => Set(ref groups, value);
		}
		/// <summary>
		///  Gets the selected sprite part group parts.
		/// </summary>
		/*public IReadOnlyList<ISpritePartGroupPart> GroupParts {
			get => groupParts;
			set {
				if (suppressCollectionEvents)
					groupParts = value;
				else
					Set(ref groupParts, value);
			}
		}*/
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
		/*{
			get => currentGame;
			private set => Set(ref currentGame, value);
		}*/
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
		/*{
			get => currentCharacter;
			private set => Set(ref currentCharacter, value);
		}*/
		/// <summary>
		///  Gets the currently selected sprite parts.
		/// </summary>
		public IReadOnlyList<ISpritePart> CurrentParts {
			get => currentParts;
			private set => Set(ref currentParts, value);
		}

		/// <summary>
		///  Gets the selection for the current character sprite.
		/// </summary>
		public ImmutableSpriteSelection SpriteSelection {
			get => selection;
			set => Set(ref selection, value);
		}

		#endregion

		#region Constructors

		public SpriteViewModel(GrisaiaModel grisaiaDb) {
			GrisaiaDatabase = grisaiaDb;

			Categories.CollectionChanged += OnCategoriesCollectionChanged;
			//Groups.CollectionChanged += OnGroupsCollectionChanged;
			GroupParts.CollectionChanged += OnGroupPartsCollectionChanged;

			GameDatabase.NamingScheme = Settings.GameNamingScheme;
			CharacterDatabase.NamingScheme = Settings.CharacterNamingScheme;
			PropertyChanged += OnPropertyChanged;

			suppressCollectionEvents = true;
			if (!IsInDesignMode) {
				GameDatabase.LocateGames();
				GameDatabase.LoadLookupCache(Settings.LoadUpdateArchives);
				SpriteDatabase.Build(Settings.SpriteCategoryOrder);
			}
			else {
				GameDatabase.LocateDummyGames();
				SpriteDatabase.BuildDummy(Settings.SpriteCategoryOrder);
			}
			SpriteSelection newSelection = selection.ToMutable();
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
			CurrentParts = Array.AsReadOnly(SpriteDatabase.GetSpriteParts(newSelection, out _, out _));
			suppressCollectionEvents = false;
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
			switch (e.PropertyName) {
			case nameof(GroupParts):
				if (!suppressCollectionEvents) return;
				SpriteSelection newSelection = selection.ToMutable();
				
				for (int i = 0; i < PartCount; i++) {
					ISpritePartGroupPart groupPart = (ISpritePartGroupPart) GroupParts[i];
					newSelection.GroupPartIds[i] = groupPart?.Id ?? -1;
				}

				// Trigger update after we've raised the restriction against updates
				SpriteSelection = newSelection.ToImmutable();
				break;
			}
		}

		#endregion

		#region UpdateChanged

		private void UpdateGameChanged() {

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
					SpriteSelection newSelection = selection.ToMutable();

					foreach (ISpriteCategory category in Categories) {
						category.Category.SetId(newSelection, category.Id);
						if (category.IsLastCategory) {
							UpdateGroups(category, newSelection);
						}
					}

					Console.WriteLine($"SpriteViewModel.CurrentParts+SpriteSelection");
					// Trigger update after we've raised the restriction against updates
					SpriteSelection = newSelection.ToImmutable();
					CurrentParts = Array.AsReadOnly(SpriteDatabase.GetSpriteParts(newSelection, out _, out _));

					suppressCollectionEvents = false;
				}
			}
		}
		/*private void OnGroupsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			//if (surpressCollectionEvents) return;
			Console.WriteLine($"SpriteViewModel.OnGroupsCollectionChanged");
			//surpressCollectionEvents = true;

			//surpressCollectionEvents = false;
		}*/
		private void OnGroupPartsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (suppressCollectionEvents) return;
			suppressCollectionEvents = true;
			Console.WriteLine($"SpriteViewModel.OnGroupPartsCollectionChanged");
			SpriteSelection newSelection = selection.ToMutable();

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
			CurrentParts = Array.AsReadOnly(SpriteDatabase.GetSpriteParts(newSelection, out _, out _));

			suppressCollectionEvents = false;
		}

		#endregion

		private void UpdateGroups(ISpriteCategory category, SpriteSelection newSelection) {
			Console.WriteLine($"SpriteViewModel.UpdateGroups");
			//HashSet<ISpritePartGroup> oldGroups = new HashSet<ISpritePartGroup>(Groups);
			List<ISpritePartGroup> oldGroups = new List<ISpritePartGroup>(Groups);
			List<ISpritePartGroupPart> groupParts = new List<ISpritePartGroupPart>(GroupParts);
			ISpritePartGroup[] newGroups = category.CreateGroups(CurrentGame, CurrentCharacter);

			//Groups.Clear();
			//GroupParts.Clear();

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
				//GroupParts.Add(newGroupPart);
				//Groups.Add(newGroup);
				//Groups[i] = newGroup;
				//GroupParts[i] = newGroupPart;
				newSelection.GroupPartIds[i] = newGroupPart.Id;
				i++;
			}
			for (; i < PartCount; i++) {
				//Groups[i] = null;
				//GroupParts[i] = SpriteDatabase.CreateNoneGroupPart();
				newSelection.GroupPartIds[i] = NoPart;
			}
			Array.Resize(ref newGroups, PartCount);
			Groups = Array.AsReadOnly(newGroups);
		}
	}
}
