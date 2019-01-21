using Newtonsoft.Json;

namespace Grisaia.Categories.Sprites {
	public enum SpriteDistance {
		// tas*2 (Keiji Sakashita, Pose 2)
		[Name("Extra Close"), Code("lll")]
		ExtraClose = -1,
		[Name("Very Close"), Code("")]
		VeryClose,
		[Name("Close"), Code("l")]
		Close,
		[Name("Medium"), Code("m")]
		Medium,
		[Name("Far"), Code("s")]
		Far,
		[Name("Face"), Code("f")]
		Face,

		// micfly (Michiru Flying)
		[Name("Small"), Code("S")]
		Small,
		[Name("Large"), Code("L")]
		Large,


		// Rakuen full-body sprites.
		// No clue what they could possibly stand for.
		[Name("Full"), Code("lb")]
		Full_1,
		[Name("Full"), Code("t")]
		Full_2,
	}

	public enum SpriteLighting {
		[Name("Day"), Code("0", "")]
		Day = 0,
		[Name("Evening"), Code("1")]
		Evening = 1,
		[Name("Night"), Code("2")]
		Night = 2,
	}
	public enum SpriteBlush {
		[Name("Default")]
		Default = 0,
		[Name("None")]
		None = 1,
		// There has to be better terms to use than these. Somebody help.
		[Name("Half")]
		Half = 2,
		[Name("Full")]
		Full = 3,
	}

	public enum SpriteSize {
		[Name("Normal"), Code("")]
		Normal = 0,
		[Name("Small"), Code("S")]
		Small,
		[Name("Large"), Code("L")]
		Large,
	}
	public sealed class SpriteInfo {

		[JsonProperty("distance")]
		public SpriteDistance Distance { get; internal set; }
		[JsonProperty("size")]
		public SpriteSize Size { get; internal set; }
		[JsonProperty("part_id")]
		public int PartType { get; internal set; }
		[JsonProperty("part_index")]
		public int Part { get; internal set; }
		[JsonIgnore]
		internal int PoseInternal { get; set; }
		[JsonProperty("pose_index")]
		public int Pose {
			get {
				if (PoseInternal == 0)
					return 0;
				return ((PoseInternal - 1) % 3) + 1;
			}
		}
		[JsonProperty("blush_index")]
		public SpriteBlush Blush {
			get {
				if (PoseInternal == 0)
					return SpriteBlush.Default;
				return (SpriteBlush) ((PoseInternal - 1) / 3);
			}
		}
		[JsonProperty("file_name")]
		public string FileName { get; internal set; }
		[JsonProperty("lighting")]
		public SpriteLighting Lighting { get; internal set; }
		[JsonIgnore]
		public string CharacterName { get; internal set; }
		[JsonProperty("character_id")]
		public string CharacterId { get; internal set; }
		[JsonProperty("game_id")]
		public string GameId { get; internal set; }
		
	}
}
