using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Grisaia.Asmodean;
using Grisaia.Categories.Sprites;
using Grisaia.Geometry;
using Grisaia.Rules.Sprites;
using Grisaia.Utils;
using Newtonsoft.Json;

namespace Grisaia.Categories {
	/// <summary>
	///  The event args used with <see cref="LoadSpritesProgressCallback"/>.
	/// </summary>
	public struct LoadSpritesProgressArgs {
		/// <summary>
		///  The current game whose sprites are being parsed.
		/// </summary>
		public GameInfo CurrentGame { get; internal set; }
		/// <summary>
		///  The index of the current located game being parsed.
		/// </summary>
		public int GameIndex { get; internal set; }
		/// <summary>
		///  The total number of located games to parse.
		/// </summary>
		public int GameCount { get; internal set; }
		/// <summary>
		///  The total number of sprites that have been parsed.
		/// </summary>
		public int SpriteCount { get; internal set; }

		/// <summary>
		///  Gets the index of the current lookup entry being parsed.
		/// </summary>
		public int EntryIndex { get; internal set; }
		/// <summary>
		///  Gets the total number of lookup entries to parse.
		/// </summary>
		public int EntryCount { get; internal set; }

		/// <summary>
		///  Gets the minor progress being made on a single game.
		/// </summary>
		public double MinorProgress {
			get => (EntryIndex == EntryCount ? 1d : ((double) EntryIndex / EntryCount));
		}
		/// <summary>
		///  Gets the major progress being made on all games.
		/// </summary>
		public double MajorProgress {
			get => (GameIndex == GameCount ? 1d : ((double) GameIndex / GameCount));
		}
		/// <summary>
		///  Gets if the progress is completely finished.
		/// </summary>
		public bool IsDone => GameIndex == GameCount;
	}
	/// <summary>
	///  An callback handler for use during the building of a sprite database.
	/// </summary>
	/// <param name="e">The progress event args.</param>
	public delegate void LoadSpritesProgressCallback(LoadSpritesProgressArgs e);
	/// <summary>
	///  A database for cached and categorized Grisaia character sprites.
	/// </summary>
	public sealed class SpriteDatabase : ObservableObject, ISpriteCategory {
		#region Static Fields

		/// <summary>
		///  The default sprite parsing rules collected from this assembly.
		/// </summary>
		private static readonly ISpriteParsingRule[] defaultRules;

		#endregion

		#region Static Constructors

		static SpriteDatabase() {
			List<ISpriteParsingRule> rules = new List<ISpriteParsingRule>();
			foreach (Type type in typeof(SpriteDatabase).Assembly.GetTypes()) {
				if (!type.IsAbstract && !type.IsGenericTypeDefinition &&
					typeof(ISpriteParsingRule).IsAssignableFrom(type)) {
					rules.Add((ISpriteParsingRule) Activator.CreateInstance(type));
				}
			}
			rules.Sort();
			defaultRules = rules.ToArray();
		}

		#endregion

		#region Fields

		/// <summary>
		///  Gets the grisaia database containing this database.
		/// </summary>
		public GrisaiaDatabase GrisaiaDatabase { get; private set; }
		/// <summary>
		///  Gets the database of games.
		/// </summary>
		public GameDatabase GameDatabase => GrisaiaDatabase.GameDatabase;
		/// <summary>
		///  Gets the database of characters.
		/// </summary>
		public CharacterDatabase CharacterDatabase => GrisaiaDatabase.CharacterDatabase;
		/// <summary>
		///  Gets all of the ordered categories, the first two categories must be characters and games or vice versa.
		/// </summary>
		public IReadOnlyList<SpriteCategoryInfo> Categories { get; private set; }
		/// <summary>
		///  Gets the number of sprites in the database.
		/// </summary>
		public int SpriteCount { get; private set; }
		/// <summary>
		///  The sorted list of elements in the category.
		/// </summary>
		private List<ISpriteCategoryBuilder> list = new List<ISpriteCategoryBuilder>();
		
		#endregion

		#region Properties
		
		/// <summary>
		///  Gets the sorted list of elements in the category.
		/// </summary>
		public IReadOnlyList<ISpriteCategory> List => list;
		IReadOnlyList<ISpriteElement> ISpriteCategory.List => list;

		#endregion

		#region Events
		
		/// <summary>
		///  The event raised after the sprite database is completely built.
		/// </summary>
		public event EventHandler BuildComplete;

		#endregion

		#region Constructors

		public SpriteDatabase(GrisaiaDatabase grisaiaDb) {
			GrisaiaDatabase = grisaiaDb ?? throw new ArgumentNullException(nameof(grisaiaDb));
			GameDatabase.PropertyChanged += OnGameDatabasePropertyChanged;
			CharacterDatabase.PropertyChanged += OnCharacterDatabasePropertyChanged;
		}

		private void OnGameDatabasePropertyChanged(object sender, PropertyChangedEventArgs e) {
			switch (e.PropertyName) {
			case nameof(GameDatabase.NamingScheme):
				foreach (ISpriteCategory category1 in list) {
					foreach (ISpriteCategory category2 in category1.List) {
						SpriteGame g = category1 as SpriteGame ?? category2 as SpriteGame;
						g.RaiseDisplayNameChanged();
					}
				}
				break;
			}
		}
		private void OnCharacterDatabasePropertyChanged(object sender, PropertyChangedEventArgs e) {
			switch (e.PropertyName) {
			case nameof(CharacterDatabase.NamingScheme):
				foreach (ISpriteCategory category1 in list) {
					foreach (ISpriteCategory category2 in category1.List) {
						SpriteCharacter c = category1 as SpriteCharacter ?? category2 as SpriteCharacter;
						c.RaiseDisplayNameChanged();
					}
				}
				break;
			}
		}

		#endregion

		#region Event Handlers

		#endregion

		#region First Category Properties

		/// <summary>
		///  Gets the number of elements in the category.
		/// </summary>
		public int Count { get; }

		#endregion

		#region First Category Accessors

		/// <summary>
		///  Gets the element at the specified index in the category.
		/// </summary>
		/// <param name="index">The index of the element to get.</param>
		/// <returns>The element at the specified index.</returns>
		public ISpriteCategory this[int index] => List[index];

		/// <summary>
		///  Gets the element with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the element to get.</param>
		/// <returns>The element with the specified Id.</returns>
		public ISpriteCategory Get(object id) {
			ISpriteCategory element = list.Find(e => e.Id.Equals(id));
			return element ?? throw new KeyNotFoundException($"Could not find the key \"{id}\"!");
		}
		/// <summary>
		///  Tries to get the element with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the element to get.</param>
		/// <param name="value">The output element if one was found, otherwise null.</param>
		/// <returns>True if an element with the Id was found, otherwise null.</returns>
		public bool TryGetValue(object id, out ISpriteCategory value) {
			value = list.Find(e => e.Id.Equals(id));
			return value != null;
		}
		/// <summary>
		///  Gets if the category contains an element with the specified Id.
		/// </summary>
		/// <param name="id">The Id to check for an element with.</param>
		/// <returns>True if an element exists with the specified Id, otherwise null.</returns>
		public bool ContainsKey(object id) => list.Find(e => e.Id.Equals(id)) != null;

		#endregion

		#region BuildDatabase

		/// <summary>
		///  Asyncrhonously builds a dummy sprite database.
		/// </summary>
		/// <param name="categories">The category order to use for the database.</param>
		public Task LoadSpritesDummyAsync(IEnumerable<SpriteCategoryInfo> categoryOrder) {
			return Task.Run(() => LoadSpritesDummy(categoryOrder));
		}
		/// <summary>
		///  Syncrhonously builds the sprite database by looking at every sprite in each game's KIFINT lookup.
		/// </summary>
		/// <param name="categoryOrder">The category order to use for the database.</param>
		public void LoadSpritesDummy(IEnumerable<SpriteCategoryInfo> categoryOrder) {
			LoadSpritesProgressArgs progress = new LoadSpritesProgressArgs {
				CurrentGame = null,
				GameIndex = -1,
				GameCount = GameDatabase.LocatedGames.Count,
				SpriteCount = 0,
			};

			//PrimaryCategories = new List<SpriteCategoryInfo>(categoryOrder.Take(2));
			//SecondaryCategories = new List<SpriteCategoryInfo>(categoryOrder.Skip(2));

			Categories = Array.AsReadOnly(categoryOrder.ToArray());

			foreach (var categoryInfo in Categories.Take(2)) {
				if (categoryInfo.IsSecondary)
					throw new ArgumentException($"Sprite Category {categoryInfo} cannot be put in the secondary list!");
			}
			foreach (var categoryInfo in Categories.Skip(2)) {
				if (categoryInfo.IsPrimary)
					throw new ArgumentException($"Sprite Category {categoryInfo} cannot be put in the primary list!");
			}

			list.Clear();
			SpriteCount = 0;
			const int ProgressThreshold = 5000;

			bool charIsPrimary = Categories[0].Id == SpriteCategoryPool.Character.Id;
			SpriteGame gameCategory = null;
			SpriteCharacter charCategory = null;
			ISpriteCategoryBuilder currentCategory;
			SpriteCategoryInfo currentCategoryInfo = Categories[1];
			foreach (GameInfo game in GameDatabase.LocatedGames) {
				Trace.WriteLine($"Categorizing: {game.Id}");
				progress.GameIndex++;
				progress.CurrentGame = game;
				if (!charIsPrimary) {
					gameCategory = new SpriteGame {
						Id = game.Id,
						GameIndex = GameDatabase.IndexOf(game),
						GameInfo = game,
					};
					list.Add(gameCategory);
				}
				string[] files = { "Tama01_1.hg3", "Tama01_001.hg3" };
				foreach (string fileName in files) {
					if (fileName[0] == 'T' && Path.GetExtension(fileName) == ".hg3") {
						// We have a sprite, (most likely)
						bool parsed = false;
						bool ignored = false;
						foreach (ISpriteParsingRule rule in defaultRules) {
							if (rule.TryParse(fileName, out SpriteInfo sprite)) {
								parsed = true;
								if (rule.Ignore) {
									ignored = true;
									break;
								}

								#region Generate Game <-> Character Category
								if (charIsPrimary) {
									if (!TryGetValue(sprite.CharacterId, out ISpriteCategory icharCategory)) {
										charCategory = new SpriteCharacter {
											Id = sprite.CharacterId,
											CharacterInfo = CharacterDatabase.Get(sprite.CharacterId),
										};
										list.Add(charCategory);
									}
									else {
										charCategory = (SpriteCharacter) icharCategory;
									}
									if (!charCategory.TryGetValue(game.Id, out ISpriteCategory igameCategory)) {
										gameCategory = new SpriteGame {
											Id = game.Id,
											GameIndex = GameDatabase.IndexOf(game),
											GameInfo = game,
										};
										charCategory.Add(gameCategory);
										currentCategory = gameCategory;
									}
									else {
										currentCategory = (ISpriteCategoryBuilder) igameCategory;
									}
								}
								else {
									if (!gameCategory.TryGetValue(sprite.CharacterId, out ISpriteCategory icharCategory)) {
										charCategory = new SpriteCharacter {
											Id = sprite.CharacterId,
											CharacterInfo = CharacterDatabase.Get(sprite.CharacterId),
										};
										gameCategory.Add(charCategory);
										currentCategory = charCategory;
									}
									else {
										currentCategory = (ISpriteCategoryBuilder) icharCategory;
									}
								}
								#endregion

								AddSprite(fileName, sprite, currentCategoryInfo, currentCategory);
								progress.SpriteCount++;
								break;
							}
						}
						if (ignored) {
							//Trace.WriteLine($"Ignored {kif.FileName}!");
						}
						else if (!parsed) {
							Trace.WriteLine($"Failed to parse {fileName}!");
							//Kifint.ExtractHg3AndImages(kif, Path.Combine(AppContext.BaseDirectory, "cache"), false);
						}
						else if (progress.SpriteCount % ProgressThreshold == 0) {
							SpriteCount = progress.SpriteCount;
						}
					}
				}
			}

			// Finalize by sorting all sprites
			list.Sort();
			foreach (ISpriteCategoryBuilder category in list) {
				SortCategory(category);
			}

			SpriteCount = progress.SpriteCount;
			progress.CurrentGame = null;
			progress.GameIndex++;
			RaisePropertyChanged(nameof(Categories));
			RaisePropertyChanged(nameof(List));
			RaisePropertyChanged(nameof(SpriteCount));
			RaisePropertyChanged(nameof(Count));
			BuildComplete?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		///  Asyncrhonously builds the sprite database by looking at every sprite in each game's KIFINT lookup.
		/// </summary>
		/// <param name="categoryOrder">The category order to use for the database.</param>
		public Task LoadSpritesAsync(IEnumerable<SpriteCategoryInfo> categoryOrder,
			LoadSpritesProgressCallback callback = null)
		{
			return Task.Run(() => LoadSprites(categoryOrder));
		}
		/// <summary>
		///  Syncrhonously builds the sprite database by looking at every sprite in each game's KIFINT lookup.
		/// </summary>
		/// <param name="categoryOrder">The category order to use for the database.</param>
		public void LoadSprites(IEnumerable<SpriteCategoryInfo> categoryOrder,
			LoadSpritesProgressCallback callback = null)
		{
			LoadSpritesProgressArgs progress = new LoadSpritesProgressArgs {
				CurrentGame = null,
				GameIndex = 0,
				GameCount = GameDatabase.LocatedGames.Count,
				SpriteCount = 0,
			};

			//PrimaryCategories = new List<SpriteCategoryInfo>(categoryOrder.Take(2));
			//SecondaryCategories = new List<SpriteCategoryInfo>(categoryOrder.Skip(2));

			Categories = Array.AsReadOnly(categoryOrder.ToArray());

			foreach (var categoryInfo in Categories.Take(2)) {
				if (categoryInfo.IsSecondary)
					throw new ArgumentException($"Sprite Category {categoryInfo} cannot be put in the secondary list!");
			}
			foreach (var categoryInfo in Categories.Skip(2)) {
				if (categoryInfo.IsPrimary)
					throw new ArgumentException($"Sprite Category {categoryInfo} cannot be put in the primary list!");
			}
			
			list.Clear();
			SpriteCount = 0;
			const int ProgressThreshold = 5000;

			bool charIsPrimary = Categories[0].Id == SpriteCategoryPool.Character.Id;
			SpriteGame gameCategory = null;
			SpriteCharacter charCategory = null;
			ISpriteCategoryBuilder currentCategory;
			SpriteCategoryInfo currentCategoryInfo = Categories[1];
			foreach (GameInfo game in GameDatabase.LocatedGames) {
				Trace.WriteLine($"Categorizing: {game.Id}");
				progress.CurrentGame = game;
				if (!charIsPrimary) {
					gameCategory = new SpriteGame {
						Id = game.Id,
						GameIndex = GameDatabase.IndexOf(game),
						GameInfo = game,
					};
					list.Add(gameCategory);
				}
				IEnumerable<KifintEntry> imageEntries = game.Lookups.Image;
				progress.EntryIndex = 0;
				progress.EntryCount = game.Lookups.Image.Count;
				if (game.Lookups.Update != null) {
					imageEntries = imageEntries.Concat(game.Lookups.Update);
					progress.EntryCount += game.Lookups.Update.Count;
				}
				callback?.Invoke(progress);
				foreach (KifintEntry kif in imageEntries) {
					if (kif.FileName[0] == 'T' && kif.Extension == ".hg3") {
						// We have a sprite, (most likely)
						bool parsed = false;
						bool ignored = false;
						foreach (ISpriteParsingRule rule in defaultRules) {
							if (rule.TryParse(kif.FileName, out SpriteInfo sprite)) {
								parsed = true;
								if (rule.Ignore) {
									ignored = true;
									break;
								}

								#region Generate Game <-> Character Category
								if (charIsPrimary) {
									if (!TryGetValue(sprite.CharacterId, out ISpriteCategory icharCategory)) {
										charCategory = new SpriteCharacter {
											Id = sprite.CharacterId,
											CharacterInfo = CharacterDatabase.Get(sprite.CharacterId),
										};
										list.Add(charCategory);
									}
									else {
										charCategory = (SpriteCharacter) icharCategory;
									}
									if (!charCategory.TryGetValue(game.Id, out ISpriteCategory igameCategory)) {
										gameCategory = new SpriteGame {
											Id = game.Id,
											GameIndex = GameDatabase.IndexOf(game),
											GameInfo = game,
										};
										charCategory.Add(gameCategory);
										currentCategory = gameCategory;
									}
									else {
										currentCategory = (ISpriteCategoryBuilder) igameCategory;
									}
								}
								else {
									if (!gameCategory.TryGetValue(sprite.CharacterId, out ISpriteCategory icharCategory)) {
										charCategory = new SpriteCharacter {
											Id = sprite.CharacterId,
											CharacterInfo = CharacterDatabase.Get(sprite.CharacterId),
										};
										gameCategory.Add(charCategory);
										currentCategory = charCategory;
									}
									else {
										currentCategory = (ISpriteCategoryBuilder) icharCategory;
									}
								}
								#endregion

								AddSprite(kif, sprite, currentCategoryInfo, currentCategory);
								progress.SpriteCount++;
								break;
							}
						}
						if (ignored) {
							//Trace.WriteLine($"Ignored {kif.FileName}!");
						}
						else if (!parsed) {
							Trace.WriteLine($"Failed to parse {kif.FileName}!");
							Kifint.ExtractHg3AndImages(kif, Path.Combine(AppContext.BaseDirectory, "cache"), false);
						}
						else if (progress.SpriteCount % ProgressThreshold == 0) {
							SpriteCount = progress.SpriteCount;
							callback?.Invoke(progress);
						}
					}
					progress.EntryIndex++;
				}
				progress.GameIndex++;
			}

			// Finalize by sorting all sprites
			list.Sort();
			foreach (ISpriteCategoryBuilder category in list) {
				SortCategory(category);
			}

			SpriteCount = progress.SpriteCount;
			progress.CurrentGame = null;
			callback?.Invoke(progress);
			RaisePropertyChanged(nameof(Categories));
			RaisePropertyChanged(nameof(List));
			RaisePropertyChanged(nameof(SpriteCount));
			RaisePropertyChanged(nameof(Count));
			BuildComplete?.Invoke(this, EventArgs.Empty);
		}

		private void AddSprite(string fileName, SpriteInfo sprite, SpriteCategoryInfo categoryInfo,
			ISpriteCategoryBuilder category) {
			object id;
			foreach (var nextCategoryInfo in Categories.Skip(2)) {
				id = nextCategoryInfo.GetInfoId(sprite);
				if (!category.TryGetValue(id, out ISpriteElement nextCategory)) {
					nextCategory = nextCategoryInfo.Create(sprite, GameDatabase, CharacterDatabase);
					category.Add(nextCategory);
				}
				categoryInfo = nextCategoryInfo;
				category = (ISpriteCategoryBuilder) nextCategory;
			}

			int partType = sprite.PartType;
			int partId = sprite.PartId;
			SpritePartList partList;
			if (!category.TryGetValue(partType, out var partListElement)) {
				partList = new SpritePartList { Id = partType };
				category.Add(partList);
			}
			else {
				partList = (SpritePartList) partListElement;
			}

			if (partList.TryGetValue(partId, out var existingPart)) {
				//partList.List.Remove(existingPart);
				Trace.WriteLine($"WARNING: \"{sprite.FileName}\" was found but \"{existingPart.FileName}\" already exists!");
				return;
			}
			SpritePart part = new SpritePart { Id = sprite.PartId, FileName = fileName };
			partList.List.Add(part);
			SpriteCount++;
		}
		private void AddSprite(KifintEntry kif, SpriteInfo sprite, SpriteCategoryInfo categoryInfo,
			ISpriteCategoryBuilder category) {
			object id;
			foreach (var nextCategoryInfo in Categories.Skip(2)) {
				id = nextCategoryInfo.GetInfoId(sprite);
				if (!category.TryGetValue(id, out ISpriteElement nextCategory)) {
					nextCategory = nextCategoryInfo.Create(sprite, GameDatabase, CharacterDatabase);
					category.Add(nextCategory);
				}
				categoryInfo = nextCategoryInfo;
				category = (ISpriteCategoryBuilder) nextCategory;
			}

			int partType = sprite.PartType;
			int partId = sprite.PartId;
			SpritePartList partList;
			if (!category.TryGetValue(partType, out var partListElement)) {
				partList = new SpritePartList { Id = partType };
				category.Add(partList);
			}
			else {
				partList = (SpritePartList) partListElement;
			}
			
			if (partList.TryGetValue(partId, out var existingPart)) {
				//partList.List.Remove(existingPart);
				Trace.WriteLine($"WARNING: \"{sprite.FileName}\" was found but \"{existingPart.FileName}\" already exists!");
				return;
			}
			SpritePart part = new SpritePart { Id = sprite.PartId, FileName = kif.FileName };
			partList.List.Add(part);
			SpriteCount++;
		}
		private void SortCategory(ISpriteCategoryBuilder category) {
			category.Sort();
			foreach (ISpriteElement element in category.List) {
				if (element is ISpriteCategoryBuilder nextCategory) {
					SortCategory(nextCategory);
					continue;
				}
				else if (element is SpritePartList partList) {
					partList.List.Sort();
				}
				break; // We're at sprite parts
			}
		}

		#endregion

		#region TraceUncategorizedSprites

		public void TraceUncategorizedSprites() {
			foreach (ISpriteCategory category1 in list) {
				foreach (ISpriteCategory category2 in category1.List) {
					SpriteGame g = category1 as SpriteGame ?? category2 as SpriteGame;
					SpriteCharacter c = category1 as SpriteCharacter ?? category2 as SpriteCharacter;

					GameInfo ga = g.GameInfo;
					CharacterInfo ch = c.CharacterInfo;
					CharacterSpritePartGroupInfo[] groups = CharacterDatabase.GetPartGroups(ga, ch);

					HashSet<int> groupTypeIds = new HashSet<int>();
					foreach (int typeId in groups.SelectMany(gr => gr.TypeIds)) {
						if (!groupTypeIds.Add(typeId)) {
							Trace.WriteLine($"WARNING: Sprite part Id {typeId} has already been defined for {g.Id}-{c.Id}!");
						}
					}
					TraceUncategorizedSprites(category2, g, c, groupTypeIds);
				}
			}
		}

		private void TraceUncategorizedSprites(ISpriteCategory category, SpriteGame g, SpriteCharacter c,
			HashSet<int> groupTypeIds)
		{
			if (category.List[0] is ISpriteCategory) {
				// More subcategories
				foreach (ISpriteCategory subCategory in list) {
					TraceUncategorizedSprites(category, g, c, groupTypeIds);
				}
			}
			else {
				// Sprite part lists
				foreach (SpritePartList partList in category.List) {
					foreach (int typeId in partList.List.Select(p => p.Id)) {
						if (groupTypeIds.Add(typeId)) {
							Trace.WriteLine($"WARNING: Sprite part Id {typeId} has not been defined for {g.Id}-{c.Id}!");
						}
					}
				}
			}
		}

		string ISpriteCategory.DisplayName => string.Empty;
		object ISpriteElement.Id => 0;
		SpriteCategoryInfo ISpriteCategory.Category => throw new NotSupportedException();
		bool ISpriteCategory.IsLastCategory => false;

		ISpriteElement ISpriteCategory.this[int index] => this[index];

		ISpriteElement ISpriteCategory.Get(object id) => Get(id);
		bool ISpriteCategory.TryGetValue(object id, out ISpriteElement value) {
			if (TryGetValue(id, out ISpriteCategory category)) {
				value = category;
				return true;
			}
			value = null;
			return false;
		}
		bool ISpriteCategory.TryGetValue(int id, out ISpritePartList value) {
			value = null;
			return false;
		}
		ISpritePartGroup[] ISpriteCategory.CreateGroups(GameInfo game, CharacterInfo character) => throw new NotSupportedException();
		/// <summary>
		///  Creates a group part representing no selection.
		/// </summary>
		/// <returns>The created empty group part.</returns>
		public ISpritePartGroupPart CreateNoneGroupPart() => SpritePartGroupPart.None;

		#endregion

		#region GetSpriteParts

		/// <summary>
		///  Gets the sprite parts for the specified sprite selection.
		/// </summary>
		/// <param name="selection">The sprite selection to get the parts for.</param>
		/// <returns>An array of sprite parts. Each value can be null if no part is selected.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="selection"/> is null.
		/// </exception>
		public ISpritePart[] GetSpriteParts(IReadOnlySpriteSelection selection, out GameInfo game,
			out CharacterInfo character, out int[] frames) {
			if (selection == null)
				throw new ArgumentNullException(nameof(selection));

			game = null;
			character = null;

			ISpriteCategory category = this;
			for (int i = 0; i < SpriteCategoryPool.Count; i++) {
				if (category is ISpriteGame sprGame)
					game = sprGame.GameInfo;
				else if (category is ISpriteCharacter sprCharacter)
					character = sprCharacter.CharacterInfo;

				object id = Categories[i].GetId(selection);
				if (!category.TryGetValue(id, out category))
					break;
			}

			ISpritePart[] parts = new ISpritePart[SpriteSelection.PartCount];
			frames = new int[SpriteSelection.PartCount];

			if (category?.IsLastCategory ?? false) {
				ISpritePartGroup[] groups = category.CreateGroups(game, character);
				for (int groupIndex = 0; groupIndex < groups.Length; groupIndex++) {
					ISpritePartGroup group = groups[groupIndex];
					int partId = selection.GroupPartIds[groupIndex];
					int frame = selection.GroupPartFrames[groupIndex];
					foreach (int typeId in group.TypeIds) {
						if (category.TryGetValue(typeId, out ISpritePartList partList)) {
							if (partList.TryGetValue(partId, out parts[typeId]))
								frames[typeId] = frame;
							else
								frames[typeId] = 0;
						}
						else {
							parts[typeId] = null;
							frames[typeId] = 0;
						}
					}
				}
			}

			foreach (ISpritePart part in parts.Where(p => p != null)) {
				LoadHg3(part, game);
			}

			return parts;
		}

		#endregion

		#region BuildSpite

		/// <summary>
		///  Builds information used to draw a sprite from a sprite selection.
		/// </summary>
		/// <param name="selection">The sprite selection information.</param>
		/// <param name="expand">True if the sprite should be fully expanded.</param>
		/// <returns>The built sprite draw information.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="selection"/> is null.
		/// </exception>
		public SpriteDrawInfo BuildSprite(IReadOnlySpriteSelection selection, bool expand) {
			ISpritePart[] parts = GetSpriteParts(selection, out var game, out var character, out int[] frames);
			SpritePartDrawInfo[] drawParts = new SpritePartDrawInfo[SpriteSelection.PartCount];

			/*if (selection == null)
				throw new ArgumentNullException(nameof(selection));

			GameInfo game = null;
			CharacterInfo character = null;

			ISpriteCategory category = this;
			for (int i = 0; i < SpriteCategoryPool.Count; i++) {
				if (category is ISpriteGame sprGame)
					game = sprGame.GameInfo;
				else if (category is ISpriteCharacter sprCharacter)
					character = sprCharacter.CharacterInfo;

				object id = Categories[i].GetId(selection);
				if (!category.TryGetValue(id, out category))
					break;
			}

			ISpritePart[] parts = new ISpritePart[SpriteSelection.PartCount];
			int[] frames = new int[SpriteSelection.PartCount];
			//Hg3[] hg3s = new Hg3[SpriteSelection.PartCount];
			//Hg3Image[] hg3Images = new Hg3Image[SpriteSelection.PartCount];

			if (category?.IsLastCategory ?? false) {
				ISpritePartGroup[] groups = category.CreateGroups(game, character);
				for (int groupIndex = 0; groupIndex < groups.Length; groupIndex++) {
					ISpritePartGroup group = groups[groupIndex];
					int partId = selection.GroupPartIds[groupIndex];
					int frame = selection.GroupPartFrames[groupIndex];
					foreach (int typeId in group.TypeIds) {
						if (category.TryGetValue(typeId, out ISpritePartList partList)) {
							partList.TryGetValue(partId, out parts[typeId]);
							frames[typeId] = frame;
						}
						else {
							parts[typeId] = null;
							frames[typeId] = 0;
						}
					}
				}
			}

			// Load HG-3's
			for (int typeId = 0; typeId < SpriteSelection.PartCount; typeId++) {
				ISpritePart part = parts[typeId];
				if (part == null)
					continue;
				Hg3 hg3 = LoadHg3(part, game);
				//hg3s[typeId] = hg3;
				//hg3Images[typeId] = hg3.Images[partFrames[typeId]];
			}*/
			
			Thickness2I expandCenter = new Thickness2I();
			var usedHg3s = parts.Select((p, i) => p?.Hg3.Images[frames[i]]).Where(h => h != null);
			//var usedHg3s = hg3Images.Where(h => h != null);
			if (usedHg3s.Any()) {
				if (expand) {
					expandCenter = new Thickness2I(
						usedHg3s.Max(h => h.CenterLeft),
						usedHg3s.Max(h => h.BaselineTop),
						usedHg3s.Max(h => h.CenterRight),
						usedHg3s.Max(h => h.BaselineBottom));
				}
				else {
					expandCenter = new Thickness2I(
						usedHg3s.Max(h => h.CenterLeft - h.MarginLeft),
						usedHg3s.Max(h => h.BaselineTop - h.MarginTop),
						usedHg3s.Max(h => h.CenterRight - h.MarginRight),
						usedHg3s.Max(h => h.BaselineBottom - h.MarginBottom));
				}
			}
			Point2I origin = new Point2I(expandCenter.Left, expandCenter.Top);
			Point2I totalSize = new Point2I(expandCenter.Left + expandCenter.Right,
											expandCenter.Top + expandCenter.Bottom);

			for (int typeId = 0; typeId < SpriteSelection.PartCount; typeId++) {
				ISpritePart part = parts[typeId];
				if (part != null) {
					int frame = frames[typeId];
					Hg3Image h = part.Hg3.Images[frame];
					string imagePath = h.GetFrameFilePath(game.CachePath, 0);
					Thickness2I margin = new Thickness2I(
						expandCenter.Left   - h.CenterLeft     + h.MarginLeft,
						expandCenter.Top    - h.BaselineTop    + h.MarginTop,
						expandCenter.Right  - h.CenterRight    + h.MarginRight,
						expandCenter.Bottom - h.BaselineBottom + h.MarginBottom);
					Point2I size = new Point2I(h.Width, h.Height);
					drawParts[typeId] = new SpritePartDrawInfo(part, typeId, frame, imagePath, margin, size);
				}
				else {
					drawParts[typeId] = SpritePartDrawInfo.None;
				}
			}

			return new SpriteDrawInfo(selection, game, character, drawParts, parts, totalSize, origin, expand);
		}

		private Hg3 LoadHg3(ISpritePart part, GameInfo game) {
			if (part.Hg3 == null) {
				if (ViewModelBase.IsInDesignModeStatic) {
					string json = Embedded.ReadAllText(Embedded.Combine("Grisaia.data.dummy", Hg3.GetJsonFileName(part.FileName)));
					part.Hg3 = JsonConvert.DeserializeObject<Hg3>(json);
				}
				else {
					if (!Directory.Exists(game.CachePath))
						Directory.CreateDirectory(game.CachePath);
					// Extract and save the HG-3 if it's not physically cached
					if (File.Exists(Hg3.GetJsonFilePath(game.CachePath, part.FileName))) {
						part.Hg3 = Hg3.FromJsonDirectory(game.CachePath, part.FileName);
						if (part.Hg3.Version != Hg3.CurrentVersion)
							part.Hg3 = null; // Reload this
					}
					if (part.Hg3 == null) {
						var kifintEntry = game.Lookups.Image[part.FileName];
						part.Hg3 = kifintEntry.ExtractHg3AndImages(game.CachePath, false);
						part.Hg3.SaveJsonToDirectory(game.CachePath);
					}
				}
			}
			return part.Hg3;
		}

		#endregion

		#region DrawSprite

		/// <summary>
		///  Draws the sprite from the specified draw info onto a transparent background.
		/// </summary>
		/// <param name="drawInfo">The sprite draw info to use.</param>
		/// <returns>The newly created bitmap that was drawn to.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="drawInfo"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <see cref="SpriteDrawInfo.IsNone"/> is true.
		/// </exception>
		public Bitmap DrawSprite(SpriteDrawInfo drawInfo) {
			return DrawSprite(drawInfo, Color.Transparent);
		}
		/// <summary>
		///  Draws the sprite from the specified draw info onto the specified background color.
		/// </summary>
		/// <param name="drawInfo">The sprite draw info to use.</param>
		/// <param name="background">The color of the background to draw.</param>
		/// <returns>The newly created bitmap that was drawn to.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="drawInfo"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <see cref="SpriteDrawInfo.IsNone"/> is true.
		/// </exception>
		public Bitmap DrawSprite(SpriteDrawInfo drawInfo, Color background) {
			if (drawInfo == null)
				throw new ArgumentNullException(nameof(drawInfo));
			
			var bitmap = new Bitmap(drawInfo.TotalSize.X, drawInfo.TotalSize.Y, PixelFormat.Format32bppArgb);
			try {
				using (var g = Graphics.FromImage(bitmap)) {
					g.Clear(background);
					foreach (var part in drawInfo.UsedDrawParts) {
						using (var partBitmap = Image.FromFile(part.ImagePath))
							g.DrawImageUnscaled(partBitmap, part.Margin.Left, part.Margin.Top);
					}
				}
				return bitmap;
			}
			catch {
				// Only dispose on exception
				bitmap.Dispose();
				throw;
			}
		}
		/// <summary>
		///  Draws the sprite from the specified draw info onto the specified background colors.
		/// </summary>
		/// <param name="drawInfo">The sprite draw info to use.</param>
		/// <param name="backgrounds">The colors of the backgrounds to draw.</param>
		/// <returns>An array of newly created bitmaps that were drawn to for each background color.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="drawInfo"/> or <paramref name="backgrounds"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <paramref name="backgrounds"/> has a length of zero.-or- <see cref="SpriteDrawInfo.IsNone"/> is true.
		/// </exception>
		public DisposableArray<Bitmap> DrawSprite(SpriteDrawInfo drawInfo, params Color[] backgrounds) {
			if (drawInfo == null)
				throw new ArgumentNullException(nameof(drawInfo));
			if (backgrounds == null)
				throw new ArgumentNullException(nameof(backgrounds));
			if (backgrounds.Length == 0)
				throw new ArgumentException($"{nameof(backgrounds)} must have a length greater than zero!",
					nameof(backgrounds));
			
			var bitmaps = new DisposableArray<Bitmap>(backgrounds.Length);
			var partBitmaps = new DisposableArray<Image>(SpriteSelection.PartCount);
			try {
				// Load the bitmap parts once
				foreach (SpritePartDrawInfo part in drawInfo.UsedDrawParts)
					partBitmaps[part.TypeId] = Image.FromFile(part.ImagePath);

				// Construct and draw to the bitmaps
				for (int i = 0; i < backgrounds.Length; i++) {
					bool opaque = backgrounds[i].A == 255;
					PixelFormat format = (opaque ? PixelFormat.Format24bppRgb : PixelFormat.Format32bppArgb);
					bitmaps[i] = new Bitmap(drawInfo.TotalSize.X, drawInfo.TotalSize.Y, format);
					using (var g = Graphics.FromImage(bitmaps[i])) {
						g.Clear(backgrounds[i]);
						foreach (SpritePartDrawInfo part in drawInfo.UsedDrawParts)
							g.DrawImageUnscaled(partBitmaps[part.TypeId], part.Margin.Left, part.Margin.Top);
					}
				}
				return bitmaps;
			} catch {
				// Only dispose on exception
				bitmaps.Dispose();
				throw;
			} finally {
				// Always dispose of parts
				partBitmaps.Dispose();
			}
		}

		#endregion

		#region DrawSprite+BuildSprite

		/// <summary>
		///  Draws the sprite from the specified sprite selection onto a transparent background.
		/// </summary>
		/// <param name="selection">The sprite selection information.</param>
		/// <param name="expand">True if the sprite should be fully expanded.</param>
		/// <returns>The newly created bitmap that was drawn to.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="selection"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <paramref name="selection"/> has no selected parts.
		/// </exception>
		public Bitmap DrawSprite(ISpriteSelection selection, bool expand) {
			return DrawSprite(BuildSprite(selection, expand), Color.Transparent);
		}
		/// <summary>
		///  Draws the sprite from the specified sprite selection onto the specified background color.
		/// </summary>
		/// <param name="selection">The sprite selection information.</param>
		/// <param name="expand">True if the sprite should be fully expanded.</param>
		/// <param name="background">The color of the background to draw.</param>
		/// <returns>The newly created bitmap that was drawn to.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="drawInfo"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <paramref name="selection"/> has no selected parts.
		/// </exception>
		public Bitmap DrawSprite(ISpriteSelection selection, bool expand, Color background) {
			return DrawSprite(BuildSprite(selection, expand), background);
		}
		/// <summary>
		///  Draws the sprite from the specified sprite selection onto the specified background colors.
		/// </summary>
		/// <param name="selection">The sprite selection information.</param>
		/// <param name="expand">True if the sprite should be fully expanded.</param>
		/// <param name="backgrounds">The colors of the backgrounds to draw.</param>
		/// <returns>An array of newly created bitmaps that were drawn to for each background color.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="selection"/> or <paramref name="backgrounds"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <paramref name="backgrounds"/> has a length of zero.-or- <paramref name="selection"/> has no selected
		///  parts.
		/// </exception>
		public DisposableArray<Bitmap> DrawSprite(ISpriteSelection selection, bool expand, params Color[] backgrounds) {
			return DrawSprite(BuildSprite(selection, expand), backgrounds);
		}

		#endregion

		#region GetUniqueSpriteId

		public string GetUniqueSpriteId(IReadOnlySpriteSelection selection) {
			ISpritePart[] parts = GetSpriteParts(selection, out _, out _, out int[] frames);
			return GetUniqueSpriteId(selection, parts, frames);
		}
		public string GetUniqueSpriteId(SpriteDrawInfo drawInfo) {
			int[] frames = drawInfo.DrawParts.Select(p => p.Frame).ToArray();
			return GetUniqueSpriteId(drawInfo.Selection, drawInfo.SpriteParts, frames);
		}
		private static string GetUniqueSpriteId(IReadOnlySpriteSelection selection, IReadOnlyList<ISpritePart> parts,
			IReadOnlyList<int> frames)
		{
			StringBuilder str = new StringBuilder();

			str.Append(selection.GameId);
			str.Append("-");
			str.Append(selection.CharacterId);

			str.Append("_");

			str.Append($"L{(int) selection.Lighting}");
			str.Append("-");
			str.Append($"D{(int) selection.Distance}");
			str.Append("-");
			str.Append($"P{(int) selection.Pose}");
			str.Append("-");
			str.Append($"B{(int) selection.Pose}");

			str.Append("_");
			
			var partNames = parts.Select((p, i) => (p != null ? $"{i}p{p.Id:D2}+{frames[i]}" : null));
			str.Append(string.Join("-", partNames.Where(p => p != null)));

			return str.ToString();
		}

		#endregion
	}
}
