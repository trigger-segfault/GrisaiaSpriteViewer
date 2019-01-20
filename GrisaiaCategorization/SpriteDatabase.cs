using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Asmodean;
using Grisaia.Rules;

namespace Grisaia {
	public delegate void LoadGameCallback(string status, string currentGame, double completion);
	public sealed class SpriteDatabase {

		public GameDatabase GameDb { get; }
		public CharacterDatabase CharacterDb { get; }

		public Dictionary<string, SpriteGame> Games { get; } = new Dictionary<string, SpriteGame>();
		public List<SpriteGame> SortedGames { get; } = new List<SpriteGame>();

		public int Count { get; private set; } 

		public SpriteDatabase(GameDatabase gameDb, CharacterDatabase charDb, LoadGameCallback callback) {
			GameDb = gameDb;
			CharacterDb = charDb;
			int count = gameDb.LocatedCount;
			int index = 0;
			foreach (GameInfo game in gameDb.LocatedGames) {
				callback?.Invoke("Categorizing Sprites...", game.JPName, (double) index / count);
				SearchGame(game, game.ENName, game.JPName);
				index++;
			}
		}

		public void TraceUncategorizedSprites() {
			foreach (var g in SortedGames) {
				GameInfo ga = GameDb.Get(g.Id);
				foreach (var c in g.SortedCharacters) {
					CharacterInfo ch = CharacterDb.Get(c.Id);
					CharacterSpritePartGroup[] groups = CharacterDb.GetPartGroup(ga, ch);
					HashSet<int> groupTypeIds = new HashSet<int>();
					foreach (int typeId in groups.SelectMany(gr => gr.TypeIds)) {
						if (!groupTypeIds.Add(typeId)) {
							Trace.WriteLine($"WARNING: Sprite part Id {typeId} has already been defined for {g.Id}-{c.Id}!");
						}
					}
					foreach (var l in c.SortedLightings) {
						foreach (var d in l.SortedDistances) {
							//foreach (var s in d.SortedSizes) {
							//foreach (var p in s.SortedPoses) {
							foreach (var p in d.SortedPoses) {
								foreach (var b in p.SortedBlushes) {
									foreach (int typeId in b.PartTypes.Keys) {
										if (groupTypeIds.Add(typeId)) {
											Trace.WriteLine($"WARNING: Sprite part Id {typeId} has not been defined for {g.Id}-{c.Id}!");
										}
									}
								}
								//}
							}
						}
					}
				}
			}
		}

		private void SearchGame(GameInfo gameInfo, string gameName, string jpGameName) {
			SpriteGame game = new SpriteGame {
				Name = gameName,
				JapaneseName = jpGameName,
				Id = gameInfo.Id,
			};
			ISpriteParsingRule[] rules = new ISpriteParsingRule[] {
				new SpriteParsingRule(),
				new DoublePartSpriteParsingRule(),
				new IdolMahouSpriteParsingRule(),
				new MosiacSpriteParsingRule()
			};
			Trace.WriteLine(gameInfo.Id);
			foreach (KifintEntry kif in gameInfo.ImageLookup) {
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
							var c = GetOrAdd(game.Characters, sprite.CharacterId);
							var l = GetOrAdd(c.Lightings, sprite.Lighting);
							var d = GetOrAdd(l.Distances, sprite.Distance);
							var p = GetOrAdd(d.Poses, sprite.Pose);
							//var s = GetOrAdd(d.Sizes, sprite.Size);
							//var p = GetOrAdd(s.Poses, sprite.Pose);
							var b = GetOrAdd(p.Blushes, sprite.Blush);
							var plist = GetOrAdd(b.PartTypes, sprite.PartType);
							if (plist.Parts.TryGetValue(sprite.Part, out var existingPart))
								Trace.WriteLine($"WARNING: \"{existingPart.FileName}\" has been overwritten by \"{sprite.FileName}\"!");
							plist.Parts[sprite.Part] = new SpritePart { Id = sprite.Part, FileName = kif.FileName };
							Count++;
							break;
						}
					}
					if (ignored) {
						Trace.WriteLine($"Ignored {kif.FileName}!");
					}
					else if (!parsed) {
						Trace.WriteLine($"Failed to parse {kif.FileName}!");
						Kifint.ExtractHgx(kif, Path.Combine(AppContext.BaseDirectory, "cache"));
					}
				}
			}
			/*foreach (string character in SpriteInfo.Characters.Values) {
				if (game.Characters.ContainsKey(character))
					continue;
				string spriteDir = Path.Combine(path, "Characters", character, "Sprites");
				if (Directory.Exists(spriteDir)) {
					SearchCharacter(game, character, spriteDir);
				}
			}*/
			game.SortedCharacters.AddRange(game.Characters.Values);
			game.SortedCharacters.Sort((a, b) => string.Compare(a.Id, b.Id, true));
			for (int i = 0; i < game.Characters.Count; i++)
				game.SortedCharacters[i].Index = i;
			foreach (var c in game.SortedCharacters) {
				Sort(c.Lightings, c.SortedLightings);
				foreach (var l in c.SortedLightings) {
					Sort(l.Distances, l.SortedDistances);
					foreach (var d in l.SortedDistances) {
						//Sort(d.Sizes, d.SortedSizes);
						//foreach (var s in d.SortedSizes) {
							//Sort(s.Poses, s.SortedPoses);
							//foreach (var p in s.SortedPoses) {
							Sort(d.Poses, d.SortedPoses);
							foreach (var p in d.SortedPoses) {
								Sort(p.Blushes, p.SortedBlushes);
								foreach (var b in p.SortedBlushes) {
									Sort(b.PartTypes, b.SortedPartTypes);
									foreach (var plist in b.SortedPartTypes) {
										Sort(plist.Parts, plist.SortedParts);
									}
								}
							//}
						}
					}
				}
			}
			Games.Add(jpGameName, game);
			SortedGames.Add(game);
		}

		private static TValue GetOrAdd<TKey, TValue>(Dictionary<TKey, TValue> dic, TKey key)
			where TValue : IKey<TKey>, new() {
			if (!dic.TryGetValue(key, out var value)) {
				value = new TValue { Id = key };
				dic.Add(key, value);
			}
			return value;
		}

		private static void Sort<TKey, TValue>(Dictionary<TKey, TValue> dic, List<TValue> list)
			where TValue : IKey<TKey> {
			list.AddRange(dic.Values.OrderBy(v => v.Id));
			for (int i = 0; i < list.Count; i++)
				list[i].Index = i;
		}
	}
}
