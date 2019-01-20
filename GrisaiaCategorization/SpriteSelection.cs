using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grisaia {
	/// <summary>
	/// A group of identifiers used to select a sprite.
	/// </summary>
	public class SpriteSelection {

		// Basics
		/// <summary>
		/// Gets the game identifier for this sprite.
		/// </summary>
		public string GameId { get; set; }
		/// <summary>
		/// Gets the character identifier for this sprite.
		/// </summary>
		public string CharacterId { get; set; }

		// General
		/// <summary>
		/// Gets the lighting identifier for this sprite.
		/// </summary>
		public SpriteLighting LightingId { get; set; }
		/// <summary>
		/// Gets the distance identifier for this sprite.
		/// </summary>
		public SpriteDistance DistanceId { get; set; }
		/// <summary>
		/// Gets the pose identifier for this sprite.
		/// </summary>
		public int PoseId { get; set; }
		/// <summary>
		/// Gets the blush identifier for this sprite.
		/// </summary>
		public SpriteBlush BlushId { get; set; }

		// Parts
		/// <summary>
		/// Gets the part identifiers for this sprite. The index of the part is the type identifier.<para/>
		/// -1 means no sprite is selected. <see cref="int.MinValue"/> means the sprite cannot be set.
		/// </summary>
		public int[] SpritePartIds { get; } = new int[12];

		//public List<SpritePartSelection> Parts { get; } = new List<SpritePartSelection>();
	}

	public class SpritePartSelection {
		/// <summary>
		/// The type identifier of the sprite part.
		/// </summary>
		public int TypeId { get; set; }
		/// <summary>
		/// The part identifier of the sprite part. -1 means no sprite is selected.
		/// </summary>
		public int PartId { get; set; }
	}
}
