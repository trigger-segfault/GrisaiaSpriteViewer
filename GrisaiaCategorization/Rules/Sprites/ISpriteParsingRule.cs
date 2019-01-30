using System;
using Grisaia.Categories.Sprites;

namespace Grisaia.Rules.Sprites {
	/// <summary>
	///  An interface for a rule for parsing a sprite file name into a <see cref="SpriteInfo"/>.
	/// </summary>
	public interface ISpriteParsingRule : IComparable<SpriteParsingRule>, IComparable {
		#region Properties

		/// <summary>
		///  Gets the sprite should be ignored if matched.
		/// </summary>
		bool IgnoreSprite { get; }

		#endregion

		#region TryParse

		/// <summary>
		///  Tries to parse the specified sprite based on it's path.
		/// </summary>
		/// <param name="fileName">The file name of the sprite.</param>
		/// <param name="sprite">The output sprite info upon success, otherwise null.</param>
		/// <returns>True if the sprite was successfully parsed.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="fileName"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <paramref name="fileName"/> is an empty string.
		/// </exception>
		bool TryParse(string fileName, out SpriteInfo sprite);

		#endregion
	}
}
