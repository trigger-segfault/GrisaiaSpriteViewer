using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Grisaia.Asmodean;
using Grisaia.Rules.Sprites;

namespace Grisaia.Categories.Sprites {
	public delegate void LoadGameCallback(string status, string currentGame, double completion);
	public sealed class SpriteDatabase {
		#region Fields

		/// <summary>
		///  Gets the database of games.
		/// </summary>
		public GameDatabase GameDatabase { get; }
		/// <summary>
		///  Gets the database of characters.
		/// </summary>
		public CharacterDatabase CharacterDatabase { get; }
		/// <summary>
		///  Games and characters can only be swapped in order with each other.<para/>
		///  Mixing them with other entries would be too confusing.
		/// </summary>
		public IReadOnlyList<SpriteCategoryInfo> PrimaryCategories { get; }
		/// <summary>
		///  Gets all other entries that are not games or characters.
		/// </summary>
		public IReadOnlyList<SpriteCategoryInfo> SecondaryCategories { get; }
		/// <summary>
		///  Gets the number of sprites in the database.
		/// </summary>
		public int SpriteCount { get; private set; }
		/// <summary>
		///  The collection of elements in the category mapped to their respective Ids.
		/// </summary>
		private Dictionary<object, ISpriteCategory> map = new Dictionary<object, ISpriteCategory>();
		/// <summary>
		///  The sorted list of elements in the category.
		/// </summary>
		private List<ISpriteCategoryBuilder> list = new List<ISpriteCategoryBuilder>();

		#endregion

		#region Properties

		/// <summary>
		///  Gets the collection of elements in the category mapped to their respective Ids.
		/// </summary>
		public IReadOnlyDictionary<object, ISpriteCategory> Map => map;
		/// <summary>
		///  Gets the sorted list of elements in the category.
		/// </summary>
		public IReadOnlyList<ISpriteCategory> List => list;

		#endregion

		#region Constructors

		public SpriteDatabase(GameDatabase gameDb, CharacterDatabase charDb, SpriteCategoryInfo[] primary,
			SpriteCategoryInfo[] secondary)
		{
			GameDatabase = gameDb;
			CharacterDatabase = charDb;
			PrimaryCategories = new List<SpriteCategoryInfo>(primary);
			SecondaryCategories = new List<SpriteCategoryInfo>(secondary);
			foreach (var categoryInfo in PrimaryCategories) {
				if (categoryInfo.IsSecondary)
					throw new ArgumentException($"Sprite Category {categoryInfo} cannot be put in the secondary list!");
			}
			foreach (var categoryInfo in SecondaryCategories) {
				if (categoryInfo.IsPrimary)
					throw new ArgumentException($"Sprite Category {categoryInfo} cannot be put in the primary list!");
			}
		}

		#endregion

		#region First Category Properties

		/// <summary>
		///  Gets the number of elements in the category.
		/// </summary>
		public int Count { get; }
		/// <summary>
		///  Gets the element at the specified index in the category.
		/// </summary>
		/// <param name="index">The index of the element to get.</param>
		/// <returns>The element at the specified index.</returns>
		public ISpriteCategory this[int index] => List[index];

		#endregion

		#region First Category Accessors

		/// <summary>
		///  Gets the element with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the element to get.</param>
		/// <returns>The element with the specified Id.</returns>
		public ISpriteCategory Get(object id) => Map[id];
		/// <summary>
		///  Tries to get the element with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the element to get.</param>
		/// <param name="value">The output element if one was found, otherwise null.</param>
		/// <returns>True if an element with the Id was found, otherwise null.</returns>
		public bool TryGet(object id, out ISpriteCategory value) => Map.TryGetValue(id, out value);
		/// <summary>
		///  Gets if the category contains an element with the specified Id.
		/// </summary>
		/// <param name="id">The Id to check for an element with.</param>
		/// <returns>True if an element exists with the specified Id, otherwise null.</returns>
		public bool Contains(object id) => Map.ContainsKey(id);

		#endregion

		#region BuildDatabase
		
		/// <summary>
		///  Builds the sprite database by looking at every sprite in each game's KIFINT lookup
		/// </summary>
		public void Build() {
			map.Clear();
			list.Clear();
			SpriteCount = 0;

			ISpriteParsingRule[] rules = new ISpriteParsingRule[] {
				new SpriteParsingRule(),
				new DoublePartSpriteParsingRule(),
				new IdolMahouSpriteParsingRule(),
				new MosiacSpriteParsingRule()
			};

			bool charIsPrimary = PrimaryCategories[0].Id == SpriteCategoryPool.Character.Id;
			SpriteGame gameCategory = null;
			SpriteCharacter charCategory = null;
			ISpriteCategoryBuilder currentCategory;
			SpriteCategoryInfo currentCategoryInfo = PrimaryCategories[1];
			foreach (GameInfo game in GameDatabase.LocatedGames) {
				Trace.WriteLine(game.Id);
				if (!charIsPrimary) {
					gameCategory = new SpriteGame {
						Id = game.Id,
						GameIndex = GameDatabase.IndexOf(game),
						GameInfo = game,
					};
					map.Add(game.Id, gameCategory);
					list.Add(gameCategory);
				}
				foreach (KifintEntry kif in game.ImageLookup) {
					if (kif.FileName[0] == 'T' && kif.Extension == ".hg3") {
						// We have a sprite, (most likely)
						bool parsed = false;
						bool ignored = false;
						foreach (ISpriteParsingRule rule in rules) {
							if (rule.TryParse(kif.FileName, out SpriteInfo sprite)) {
								parsed = true;
								if (rule.IgnoreSprite) {
									ignored = true;
									break;
								}

								#region Generate Game <-> Character Category
								if (charIsPrimary) {
									if (!map.TryGetValue(sprite.CharacterId, out var icharCategory)) {
										charCategory = new SpriteCharacter {
											Id = sprite.CharacterId,
											CharacterInfo = CharacterDatabase.Get(sprite.CharacterId),
										};
										map.Add(sprite.CharacterId, charCategory);
										list.Add(charCategory);
									}
									else {
										charCategory = (SpriteCharacter) icharCategory;
									}
									if (!charCategory.Map.TryGetValue(game.Id, out var igameCategory)) {
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
									if (!gameCategory.Map.TryGetValue(sprite.CharacterId, out var icharCategory)) {
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
								break;
							}
						}
						if (ignored) {
							//Trace.WriteLine($"Ignored {kif.FileName}!");
						}
						else if (!parsed) {
							Trace.WriteLine($"Failed to parse {kif.FileName}!");
							Kifint.ExtractHgx(kif, Path.Combine(AppContext.BaseDirectory, "cache"));
						}
					}
				}
			}

			// Finalize by sorting all sprites
			list.Sort();
			foreach (ISpriteCategoryBuilder category in list) {
				SortCategory(category);
			}
		}

		private void AddSprite(KifintEntry kif, SpriteInfo sprite, SpriteCategoryInfo categoryInfo,
			ISpriteCategoryBuilder category) {
			object id;
			foreach (var nextCategoryInfo in SecondaryCategories) {
				id = nextCategoryInfo.GetId(sprite);
				if (!category.TryGet(id, out var nextCategory)) {
					nextCategory = nextCategoryInfo.Create(sprite, GameDatabase, CharacterDatabase);
					category.Add(nextCategory);
				}
				categoryInfo = nextCategoryInfo;
				category = (ISpriteCategoryBuilder) nextCategory;
			}

			int partType = sprite.PartType;
			int partId = sprite.Part;
			SpritePartList partList;
			if (!category.TryGet(partType, out var partListElement)) {
				partList = new SpritePartList { Id = partType };
				category.Add(partList);
			}
			else {
				partList = (SpritePartList) partListElement;
			}

			if (partList.Map.TryGetValue(partId, out var existingPart))
				Trace.WriteLine($"WARNING: \"{existingPart.FileName}\" has been overwritten by \"{sprite.FileName}\"!");
			SpritePart part = new SpritePart { Id = sprite.Part, FileName = kif.FileName };
			partList.Map[sprite.Part] = part;
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
					CharacterSpritePartGroup[] groups = CharacterDatabase.GetPartGroup(ga, ch);

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
					foreach (int typeId in partList.Map.Keys) {
						if (groupTypeIds.Add(typeId)) {
							Trace.WriteLine($"WARNING: Sprite part Id {typeId} has not been defined for {g.Id}-{c.Id}!");
						}
					}
				}
			}
		}

		#endregion
	}
}
