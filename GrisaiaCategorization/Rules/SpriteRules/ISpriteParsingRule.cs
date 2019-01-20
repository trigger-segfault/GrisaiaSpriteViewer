using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grisaia.Rules {
	/// <summary>
	///  An interface for a rule for parsing a sprite file name into a <see cref="SpriteInfo"/>.
	/// </summary>
	public interface ISpriteParsingRule {
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
		/// <param name="path">The file path of the sprite.</param>
		/// <param name="sprite">The output sprite info upon success, otherwise null.</param>
		/// <returns>True if the sprite was successfully parsed.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="path"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <paramref name="path"/> is an empty string.
		/// </exception>
		bool TryParse(string path, out SpriteInfo sprite);

		#endregion
	}
}
