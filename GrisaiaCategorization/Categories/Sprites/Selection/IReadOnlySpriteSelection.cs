using System;
using System.Collections.Generic;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  An interface storing an entire read only selection for a character sprite.
	/// </summary>
	public interface IReadOnlySpriteSelection : IEquatable<IReadOnlySpriteSelection> {
		#region Properties

		// Primary: 157 Total
		/// <summary>
		///  Gets the Id for the sprite's Grisaia game.
		/// </summary>
		string GameId { get; }
		/// <summary>
		///  Gets the Id for the sprite's Grisaia character.
		/// </summary>
		string CharacterId { get; }

		// Secondary: 360 Total
		/// <summary>
		///  Gets the Id for the sprite's lighting level.
		/// </summary>
		SpriteLighting Lighting { get; }
		/// <summary>
		///  Gets the Id for the sprite's draw distance.
		/// </summary>
		SpriteDistance Distance { get; }
		/// <summary>
		///  Gets the Id for the sprite's character pose.
		/// </summary>
		SpritePose Pose { get; }
		/// <summary>
		///  Gets the Id for the sprite's blush level.
		/// </summary>
		SpriteBlush Blush { get; }

		// Parts:
		/// <summary>
		///  Gets the sprite part group part Id selections.
		/// </summary>
		IReadOnlyList<int> GroupPartIds { get; }
		/// <summary>
		///  Gets the sprite part group part frame index selections.
		/// </summary>
		IReadOnlyList<int> GroupPartFrames { get; }

		#endregion

		#region ToImmutable

		/// <summary>
		///  Creates a mutable clone of the sprite selection.
		/// </summary>
		/// <returns>The mutable copy of the sprite selection.</returns>
		ISpriteSelection ToMutable();
		/// <summary>
		///  Creates an immutable clone of the sprite selection.
		/// </summary>
		/// <returns>The immutable copy of the sprite selection.</returns>
		IReadOnlySpriteSelection ToImmutable();

		#endregion
	}
}
