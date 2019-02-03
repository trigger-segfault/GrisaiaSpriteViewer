using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Grisaia.Asmodean;
using Grisaia.Categories;
using Grisaia.Categories.Sprites;
using Newtonsoft.Json;

namespace Grisaia.Testing {
	class Program {
		static event EventHandler Event;

		public class EventClass {
			public void OnEvent(object sender, EventArgs e) {
				Console.WriteLine("Fire");
			}
		}

		static void Main(string[] args) {
			Event += new EventClass().OnEvent;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			Event(null, null);
			Console.WriteLine(Event.GetInvocationList().Length);
			Console.ReadLine();
			GrisaiaDatabase grisaiaDb = new GrisaiaDatabase();
			grisaiaDb.GameDatabase.LocateGames();
			Console.WriteLine("Loading Cache...");
			HashSet<string> builtGames = new HashSet<string>();
			grisaiaDb.GameDatabase.LoadCache(callback: e => {
				if (e.IsBuilding && builtGames.Add(e.CurrentGame.Id))
					Console.WriteLine($"Building {e.CurrentGame.Id}...");
			});
			Console.WriteLine();
			Console.WriteLine("Searching HG-3's...");
			HashSet<int> valueSet = new HashSet<int>();
			foreach (GameInfo game in grisaiaDb.GameDatabase.LocatedGames) {
				IEnumerable<KifintEntry> images = game.Lookups.Image;
				if (game.Lookups.Update != null)
					images = images.Concat(game.Lookups.Update);

				string dir = Path.Combine(@"C:\Users\Onii-chan\Pictures\Sprites\Grisaia\hg3", game.Id);
				if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);
				Console.WriteLine($"Searching {game.Id}...");
				/*foreach (string file in Directory.EnumerateFiles(dir)) {
					string name = Path.GetFileNameWithoutExtension(file);
					name = name.Substring(0, name.Length - 4);
					Hg3 hg3 = Hg3.FromJsonDirectory(dir, name);
					if (hg3.WonkeyPadding.Count == 0 && hg3.Unknown3 != 412)
						Console.WriteLine($"{hg3.Unknown3} {hg3.FileName}");
					//valueSet.Add(hg3.Unknown3);
					//if ((hg3.Unknown3 & 0x100) != 0)
					//	Console.WriteLine($"{Convert.ToString(hg3.Unknown3, 2).PadLeft(32, '0')} {hg3.FileName}");
				}*/
				/*foreach (string file in Directory.EnumerateFiles(dir)) {
					string name = Path.GetFileNameWithoutExtension(file);
					name = name.Substring(0, name.Length - 4);
					Hg3 hg3 = Hg3.FromJsonDirectory(dir, name);
					valueSet.Add(hg3.Unknown3);
					//if ((hg3.Unknown3 & 0x100) != 0)
					//	Console.WriteLine($"{Convert.ToString(hg3.Unknown3, 2).PadLeft(32, '0')} {hg3.FileName}");
				}*/
				//Console.WriteLine(game.Id);
				foreach (KifintEntry kif in images) {
					if (/*kif.FileName == "bgmmode.hg3" &&*/ kif.Extension == ".hg3") {
						//kif.ExtractToDirectory(dir);
						Hg3 hg3 = kif.ExtractHg3();
						//hg3.SaveJsonToDirectory(dir);
						//if (hg3.Any(h => h.FrameCount != 0 && h.FrameCount != 1))
						//	kif.ExtractHg3AndImages()
					}
				}
				Console.WriteLine();
			}
			/*int[] no = Hg3.NoPadding.ToArray();
			Array.Sort(no);
			int[] yes = Hg3.YesPadding.ToArray();
			Array.Sort(yes);
			int[] both = no.Intersect(yes).ToArray();
			File.WriteAllText("nopadding.json", JsonConvert.SerializeObject(no));
			File.WriteAllText("yespadding.json", JsonConvert.SerializeObject(yes));
			File.WriteAllText("bothpadding.json", JsonConvert.SerializeObject(both));*/
			/*var list = valueSet.ToList();
			list.Sort();
			var hex = list.Select(v => Convert.ToString(v, 16).PadLeft(6, '0').ToUpper()).ToArray();
			var bin = list.Select(ToBinary).ToArray();
			
			for (int i = 0; i < valueSet.Count; i++) {
				foreach (char c in bin[i]) {
					if (c == '1')
						Console.ForegroundColor = ConsoleColor.Green;
					else
						Console.ForegroundColor = ConsoleColor.DarkGray;
					Console.Write(c);
				}
				Console.ResetColor();
				Console.Write($" 0x{hex[i]} {list[i]}");
				Console.WriteLine();
			}*/

			Console.Beep();
			Console.WriteLine("Finished!");
			Console.ReadLine();
			/*Dictionary<string, List<Anm>> allAnims = new Dictionary<string, List<Anm>>();
			foreach (string dir in Directory.EnumerateDirectories(@"C:\Users\Onii-chan\Pictures\Sprites\Grisaia")) {
				string gameName = Path.GetFileName(dir);
				string rawDir = Path.Combine(dir, "Raw");
				if (Directory.Exists(rawDir)) {
					List<Anm> all = new List<Anm>();
					allAnims.Add(gameName, all);
					foreach (string file in Directory.EnumerateFiles(rawDir)) {
						if (Path.GetExtension(file) == ".anm")
							all.Add(Anm.FromFile(file));
					}
				}
			}


			foreach (string dir in Directory.EnumerateDirectories(@"C:\Users\Onii-chan\Pictures\Sprites\Grisaia\anm")) {
				Test(Path.GetFileName(dir));
			}*/

			/*string configPath = Path.Combine(AppContext.BaseDirectory, "ConfigSettings.json");
			File.WriteAllText(configPath, JsonConvert.SerializeObject(new ConfigSettings(), Formatting.Indented));
			ConfigSettings settings = JsonConvert.DeserializeObject<ConfigSettings>(File.ReadAllText(configPath));

			Stopwatch watch = Stopwatch.StartNew();
			//var gameDb = JsonConvert.DeserializeObject<GameDatabase>(File.ReadAllText("Games.json"));
			//var charDb = JsonConvert.DeserializeObject<CharacterDatabase>(File.ReadAllText("Characters.json"));
			var gameDb = GameDatabase.FromJsonFile(Path.Combine(AppContext.BaseDirectory, "Games.json"));
			var charDb = CharacterDatabase.FromJsonFile(Path.Combine(AppContext.BaseDirectory, "Characters.json"));
			gameDb.LocateGames();*/

			/*Dictionary<string, HashSet<string>> kifintExtensions = new Dictionary<string, HashSet<string>>();

			foreach (GameInfo game in gameDb.LocatedGames) {
				Trace.WriteLine($"==== {game.Id} ====");
				foreach (string kifintPath in Directory.EnumerateFiles(game.InstallDir, "*.int")) {
					string fileName = Path.GetFileName(kifintPath);
					if (!kifintExtensions.TryGetValue(fileName, out var extensions)) {
						extensions = new HashSet<string>();
						kifintExtensions.Add(fileName, extensions);
					}
					
					Trace.Write($"{fileName}: ");
					string[] exts = Kifint.IdentifyFileTypes(kifintPath, game.Executable);
					foreach (string ext in exts)
						extensions.Add(ext);
					Trace.WriteLine(string.Join(", ", exts));
				}
				Trace.WriteLine("");
			}
			int padding = kifintExtensions.Keys.Max(k => k.Length + 1);

			Trace.WriteLine($"==== ALL GAMES ====");
			var pairs = kifintExtensions.ToList();
			pairs.Sort((a, b) => string.Compare(a.Key, b.Key));
			foreach (var pair in pairs) {
				string fileName = pair.Key;
				var exts = pair.Value.ToList();
				exts.Sort();
				Trace.WriteLine($"{fileName.PadRight(padding)}: {string.Join(", ", exts)}");
			}

			Console.WriteLine("FINISHED!");
			//Console.Beep();
			//Console.ReadLine();
			Environment.Exit(0);*/

			//gameDb.LoadCache();
			/*Console.WriteLine("Time: " + watch.ElapsedMilliseconds); watch.Restart();
			var game = gameDb.Get("kajitsu");
			KifintEntry entry = game.ImageLookup["_conf_txt.hg3"];
			string dir = Path.Combine(gameDb.CachePath, "_conf_txt");
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			var hg3 = entry.ExtractHg3(dir, true, false);
			dir = Path.Combine(gameDb.CachePath, "_conf_txt#expanded");
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			entry.ExtractHg3(dir, true, true);
			hg3.SaveJsonToDirectory(gameDb.CachePath);*/

			/*Console.WriteLine("Time: " + watch.ElapsedMilliseconds); watch.Restart();
			var spriteDb = new SpriteDatabase(gameDb, charDb);
			spriteDb.Build(
				new SpriteCategoryInfo[] {
					SpriteCategoryPool.Character,
					SpriteCategoryPool.Game,

					SpriteCategoryPool.Distance,
					SpriteCategoryPool.Lighting,
					SpriteCategoryPool.Pose,
					SpriteCategoryPool.Blush,
				});
			int count = spriteDb.List.Sum(c => c.Count);
			spriteDb.Build(
				new SpriteCategoryInfo[] {
					SpriteCategoryPool.Game,
					SpriteCategoryPool.Character,

					SpriteCategoryPool.Distance,
					SpriteCategoryPool.Lighting,
					SpriteCategoryPool.Pose,
					SpriteCategoryPool.Blush,
				});
			int count2 = spriteDb.List.Sum(c => c.Count);
			Console.WriteLine(count);
			Console.WriteLine(count2);
			Console.WriteLine("Time: " + watch.ElapsedMilliseconds);
			Console.WriteLine("Sprites: " + spriteDb.SpriteCount);
			Console.WriteLine("Finished");
			Console.Read();*/
		}

		private static string ToBinary(int value) {
			StringBuilder str = new StringBuilder(Convert.ToString(value, 2).PadLeft(24, '0'));
			for (int i = 16; i > 0; i -= 4) {
				str.Insert(i, ' ');
			}
			return str.ToString();
		}

		static void Test(string name) {
			string dir = Path.Combine(@"C:\Users\Onii-chan\Pictures\Sprites\Grisaia\anm", name);
			string file = Path.Combine(dir, name);
			string anmFile = Path.ChangeExtension(file, ".anm");
			string pngFile = Path.ChangeExtension(file, ".png");
			string pngZeroIndexFile = Path.ChangeExtension(file + "+000+000", ".png");
			int frameCount = Directory.EnumerateFiles(dir).Where(f => Path.GetFileName(f).Contains("+")).Count();
			bool hasOriginal = File.Exists(pngFile);
			bool hasZeroIndex = File.Exists(pngZeroIndexFile);
			Dictionary<int, byte> nonZeroBytes = new Dictionary<int, byte>();
			/*byte[] data = File.ReadAllBytes(anmFile);
			for (int i = 4; i < data.Length; i++) {
				byte b = data[i];
				if (b != 0)
					nonZeroBytes.Add(i, b);
			}*/
			//Console.WriteLine(name);
			//Console.WriteLine($"         Bytes: {data.Length}");
			//Console.WriteLine($"Non-zero Bytes: {nonZeroBytes.Count}");

			var anm = Anm.Extract(anmFile);

			Console.WriteLine(anm);
			Console.WriteLine();
			Console.WriteLine($"   Frame Count: {frameCount}");
			Console.WriteLine($"  Has Original: {hasOriginal}");
			Console.WriteLine($"   Has 0-Index: {hasZeroIndex}");
			Console.WriteLine();
			foreach (AnmFrame frame in anm) {
				Console.WriteLine($"- {frame}");
			}

			/*for (int i = 0; i < data.Length; i++) {
				if (i % 34 == 0)
					Console.WriteLine();
				byte b = data[i];
				if (b != 0)
					Console.ForegroundColor = ConsoleColor.Blue;
				else
					Console.ForegroundColor = ConsoleColor.White;
				Console.Write($"{b:X2} ");
				Console.ResetColor();
			}
			Console.WriteLine();*/
			Console.WriteLine();
		}
	}
}
