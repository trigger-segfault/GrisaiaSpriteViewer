using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Grisaia.Asmodean;
using Grisaia.Categories;
using Grisaia.Categories.Sprites;
using Newtonsoft.Json;

namespace Grisaia.Testing {
	class Program {
		static void Main(string[] args) {
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

			Stopwatch watch = Stopwatch.StartNew();
			//var gameDb = JsonConvert.DeserializeObject<GameDatabase>(File.ReadAllText("Games.json"));
			//var charDb = JsonConvert.DeserializeObject<CharacterDatabase>(File.ReadAllText("Characters.json"));
			var gameDb = GameDatabase.FromJsonFile(Path.Combine(AppContext.BaseDirectory, "Games.json"));
			var charDb = CharacterDatabase.FromJsonFile(Path.Combine(AppContext.BaseDirectory, "Characters.json"));
			gameDb.LocateGames();

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

			gameDb.LoadCache();
			Console.WriteLine("Time: " + watch.ElapsedMilliseconds); watch.Restart();
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
			hg3.SaveJsonToDirectory(gameDb.CachePath);

			Console.WriteLine("Time: " + watch.ElapsedMilliseconds); watch.Restart();
			var spriteDb = new SpriteDatabase(gameDb, charDb,
				new SpriteCategoryInfo[] {
					SpriteCategoryPool.Character,
					SpriteCategoryPool.Game,
				},
				new SpriteCategoryInfo[] {
					SpriteCategoryPool.Distance,
					SpriteCategoryPool.Lighting,
					SpriteCategoryPool.Pose,
					SpriteCategoryPool.Blush,
				});
			spriteDb.Build();
			Console.WriteLine("Time: " + watch.ElapsedMilliseconds);
			Console.WriteLine("Sprites: " + spriteDb.SpriteCount);
			Console.WriteLine("Finished");
			Console.Read();
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
