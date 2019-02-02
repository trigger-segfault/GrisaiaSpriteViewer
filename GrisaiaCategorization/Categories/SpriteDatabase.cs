using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Grisaia.Asmodean;
using Grisaia.Categories.Sprites;
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
	public sealed class SpriteDatabase : ISpriteCategory {
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
		}

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
			out CharacterInfo character)
		{
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

			if (category?.IsLastCategory ?? false) {
				ISpritePartGroup[] groups = category.CreateGroups(game, character);
				for (int i = 0; i < groups.Length; i++) {
					ISpritePartGroup group = groups[i];
					int partId = selection.GroupPartIds[i];
					foreach (int typeId in group.TypeIds) {
						if (category.TryGetValue(typeId, out ISpritePartList partList))
							partList.TryGetValue(partId, out parts[typeId]);
						else
							parts[typeId] = null;
					}
				}
			}

			foreach (ISpritePart part in parts.Where(p => p != null)) {
				if (part.Hg3 == null) {
					if (ViewModelBase.IsInDesignModeStatic) {
						string json = Embedded.ReadAllText(Embedded.Combine("GrisaiaCategorization.data.dummy", Hg3.GetJsonFileName(part.FileName)));
						part.Hg3 = JsonConvert.DeserializeObject<Hg3>(json);
					}
					else {
						if (!Directory.Exists(game.CachePath))
							Directory.CreateDirectory(game.CachePath);
						// Extract and save the HG-3 if it's not physically cached
						if (!File.Exists(Hg3.GetJsonFilePath(game.CachePath, part.FileName))) {
							var kifintEntry = game.Lookups.Image[part.FileName];
							part.Hg3 = kifintEntry.ExtractHg3AndImages(game.CachePath, false);
							part.Hg3.SaveJsonToDirectory(game.CachePath);
						}
						else {
							part.Hg3 = Hg3.FromJsonDirectory(game.CachePath, part.FileName);
						}
					}
				}
			}

			return parts;
		}

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
								if (rule.IgnoreSprite) {
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
								if (rule.IgnoreSprite) {
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
	}
}
