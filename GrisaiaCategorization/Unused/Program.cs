using System;
using System.IO;
using System.Linq;
using Grisaia;
using Grisaia.Asmodean;
using Newtonsoft.Json;

namespace Grisaia {
	class Program {
		static void Main(string[] args) {
			Console.WriteLine("Hello World!");
			GameDatabase gameDatabase =
				JsonConvert.DeserializeObject<GameDatabase>(File.ReadAllText("Games.json"));
			CharacterDatabase charDatabase =
				JsonConvert.DeserializeObject<CharacterDatabase>(File.ReadAllText("Characters.json"));
			gameDatabase.LocateGames();
			foreach (var game in gameDatabase.LocatedGames.Take(1)) {
				Console.WriteLine(game.JPName);
				game.ImageLookup = Kifint.DecryptImages(
					game.InstallDir,
					game.Executable/*,
					e => {
						//Console.Write($"\r{e.FileIndex}/{e.FileCount}");
						return true;
					}*/);
				Console.WriteLine();

			}
			SpriteDatabase spriteDb = new SpriteDatabase(gameDatabase);
			Console.WriteLine(spriteDb.Count);
			Console.ReadLine();
		}
	}
}
