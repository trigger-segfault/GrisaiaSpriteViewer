using System.Collections.Generic;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  An interface storing an entire selection for a character sprite.
	/// </summary>
	public interface ISpriteSelection : IReadOnlySpriteSelection {
		#region Properties

		// Primary: 157 Total
		/// <summary>
		///  Gets or sets the Id for the sprite's Grisaia game.
		/// </summary>
		new string GameId { get; set; }
		/// <summary>
		///  Gets or sets the Id for the sprite's Grisaia character.
		/// </summary>
		new string CharacterId { get; set; }

		// Secondary: 360 Total
		/// <summary>
		///  Gets or sets the Id for the sprite's lighting level.
		/// </summary>
		new SpriteLighting Lighting { get; set; }
		/// <summary>
		///  Gets or sets the Id for the sprite's draw distance.
		/// </summary>
		new SpriteDistance Distance { get; set; }
		/// <summary>
		///  Gets or sets the Id for the sprite's character pose.
		/// </summary>
		new SpritePose Pose { get; set; }
		/// <summary>
		///  Gets or sets the Id for the sprite's blush level.
		/// </summary>
		new SpriteBlush Blush { get; set; }

		// Parts:
		/// <summary>
		///  Gets the sprite part group part Id selections.
		/// </summary>
		new IList<int> GroupPartIds { get; }
		/// <summary>
		///  Gets the sprite part group part frame index selections.
		/// </summary>
		new IList<int> GroupPartFrames { get; }

		#endregion
	}
}
