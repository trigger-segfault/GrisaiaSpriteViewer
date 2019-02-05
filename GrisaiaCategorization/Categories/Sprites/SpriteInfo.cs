using System;
using System.ComponentModel;
using Grisaia.Utils;
using Newtonsoft.Json;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  The enumeration for the draw distance of a character sprite.
	/// </summary>
	public enum SpriteDistance : byte {
		// tas*2 (Keiji Sakashita, Pose 2)
		[Name("Extra Close"), Code("lll")]
		ExtraClose = 0,
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

		// micfly,kazfly (Michiru, Kazuki Flying)
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

		/// <summary>Gets the number of possibilities for <see cref="SpriteDistance"/>.</summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		Count,
	}
	/// <summary>
	///  The enumeration for the pose of a character sprite.
	/// </summary>
	public enum SpritePose : byte {
		[Name("A")]
		A = 0,
		[Name("B")]
		B = 1,
		[Name("C")]
		C = 2,

		/// <summary>Gets the number of possibilities for <see cref="SpritePose"/>.</summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		Count,
	}
	/// <summary>
	///  The enumeration for the time-of-day lighting of a character sprite.
	/// </summary>
	public enum SpriteLighting : byte {
		[Name("Day"), Code("0")]
		Day = 0,
		[Name("Evening"), Code("1")]
		Evening = 1,
		[Name("Night"), Code("2")]
		Night = 2,

		/// <summary>Gets the number of possibilities for <see cref="SpriteLighting"/>.</summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		Count,
	}
	/// <summary>
	///  The enumeration for the blush level of a character sprite.
	/// </summary>
	public enum SpriteBlush : byte {
		[Name("Default")]
		Default = 0,
		[Name("None")]
		None = 1,
		// There HAS to be better terms to use than these. Somebody please help.
		[Name("Half")]
		Half = 2,
		[Name("Full")]
		Full = 3,

		/// <summary>Gets the number of possibilities for <see cref="SpriteBlush"/>.</summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		Count,
	}
	/// <summary>
	///  The enumeration for the draw size of a character sprite.
	/// </summary>
	public enum SpriteSize : byte {
		[Name("Normal"), Code("")]
		Normal = 0,
		[Name("Small"), Code("S")]
		Small,
		[Name("Large"), Code("L")]
		Large,

		/// <summary>Gets the number of possibilities for <see cref="SpriteSize"/>.</summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		Count,
	}
	public sealed class SpriteInfo {

		[JsonProperty("lighting_index")]
		public SpriteLighting Lighting { get; internal set; }
		[JsonProperty("distance_code")]
		public SpriteDistance Distance { get; internal set; }
		[JsonProperty("pose_index")]
		public SpritePose Pose { get; internal set; }
		[JsonProperty("blush_index")]
		public SpriteBlush Blush { get; internal set; }
		//[JsonProperty("size_code")]
		//public SpriteSize Size { get; internal set; }
		[JsonProperty("part_type")]
		public int PartType { get; internal set; }
		[JsonProperty("part_index")]
		public int PartId { get; internal set; }
		//[JsonIgnore]
		//private int poseInternal;// = 1;
		/*[JsonIgnore]
		internal int PoseInternal {
			get => ((int) Pose + ((int) Blush * 3));
			set {
				if (value < 0 || value >= 12)
					throw new ArgumentOutOfRangeException(nameof(PoseInternal));
				Pose  = (SpritePose)  (value % 3);
				Blush = (SpriteBlush) (value / 3);
			}
		}*/
		[JsonProperty("file_name")]
		public string FileName { get; internal set; }
		[JsonProperty("character_id")]
		public string CharacterId { get; internal set; }
		[JsonProperty("game_id")]
		public string GameId { get; internal set; }

	}


	
}
