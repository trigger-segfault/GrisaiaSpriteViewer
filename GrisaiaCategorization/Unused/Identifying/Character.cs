using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GrisaiaExtractor.Identifying {

	public enum CharacterType {
		[Name("CG"), Code("")]
		[Group("CG")]
		CG = 0,

		[Name("Chibi"), Code("sd", "_sd", "__sd", IgnoreCase = true)]
		[Group("Chibi")]
		Chibi,

		[Name("Sprite"), Code("T")]
		[Group("Sprites")]
		Sprite,
	}

	/*public enum CGScale {
		[Name("Normal"), Code("")]
		Normal = 0,

		[Name("Thumb")]
	}*/

	
	
	public class Character {

		public static readonly Dictionary<string, string> Characters =
			new Dictionary<string, string>()
		{
			{ "hir", "Amane Classmates/Hiroka Tamaki" },
			{ "ibu", "Amane Classmates/Haruna Ibuki" },
			{ "kan", "Amane Classmates/Saaya Kaneda" },
			{ "koi", "Amane Classmates/Ritsu Koide" },
			{ "kom", "Amane Classmates/Megumi Komori" },
			{ "och", "Amane Classmates/Yoshihiko Ochi" },
			{ "sak", "Amane Classmates/Chiaki Sakashita" },
			{ "skm", "Amane Classmates/Minori Sakuma" },
			{ "skr", "Amane Classmates/Mifuyu Sakurai" },
			{ "tas", "Amane Classmates/Keiji Sakashita" },
			{ "sas", "Sachi Komine/Battle" },
			{ "mib", "Michiru Matsushima/Black Hair" },
			{ "ama", "Amane Suou" },
			{ "mak", "Makina Irisu" },
			{ "mic", "Michiru Matsushima" },
			{ "sac", "Sachi Komine" },
			{ "yum", "Yumiko Sakaki" },
			{ "kar", "Kazuki Kazami" },
			{ "kap", "Kazuki Kazami" },
			{ "kaz", "Kazuki Kazami" },
			{ "chi", "Chizuru Tachibana" },
			{ "jb", "JB" },
			{ "jbs", "JB" },
			{ "asa", "Asako Kusakabe" },
			{ "yuj", "Yuuji Kazami" },
			{ "kia", "Chiara Farrell" },
			{ "sam", "Sachi's Mother" },
			{ "saf", "Sachi's Father" },
			{ "yuf", "Michiaki Sakaki (Yumiko's Father)" },
			{ "mif", "Michiru's Friend" },
			{ "kiy", "Kiyoka Irisu (Makina's Mother)" },
			{ "amm", "Amane Suou (Middle School)" },
			{ "amp", "Amane Suou (Middle School)" },
			{ "nya", "Nyanmel" },
			{ "tan", "Thanatochu" },
			{ "kam", "Kami-sama" },
			{ "yjf", "Ryouji Kazami (Yuuji's Father)" },
			{ "yjm", "Satoko Kazami (Yuuji's Mother)" },
			{ "dan", "Daniel Bone" },
			{ "edi", "Edward Walker" },
			{ "gar", "Agnes Garrett" },
			{ "jei", "Justin Mikemeyer" },
			{ "joh", "John (Yuuji's dog)" },
			{ "osr", "Heath Oslo" },
			{ "osl", "Heath Oslo" },
			{ "mar", "Marin" },
			{ "mir", "Milliela Stanfield" },
			{ "rob", "Robert Wallson" },
			{ "dave", "Professor Dave" },
			{ "dav", "Professor Dave" },
			//{ "ev", "Everyone" },
			{ "oth", "Other" },
			//{ "op", "Other" },
		};

		public static readonly Dictionary<string, string> SpriteOnlyCharacters =
			new Dictionary<string, string>()
		{
			{ "meg", "Goddess" }, // TODO: What is her name?
			{ "cha", "Chaos" },
			{ "gho", "Ghost" },
			{ "str", "Stranger" },
			//{ "nan", "Nan" },
			//{ "ren", "Ren" },
			//{ "mag", "Mag" },
		};

		public static readonly Regex SpriteRegex = new Regex(
			@"^T" +
			@"(?'name'" + string.Join("|", Characters.Keys.Concat(SpriteOnlyCharacters.Keys)) + @")" +
			@"(?'lighting'\d)" +
			@"(?'pose'\d)" +
			@"(?'height'[flms]?)" +
			@"_" +
			@"(?'part'0*[1-9a-z\-\+\=])" +
			@"$");
	}
}
