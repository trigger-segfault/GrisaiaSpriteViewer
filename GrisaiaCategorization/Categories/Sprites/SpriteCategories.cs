using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grisaia;
using Grisaia.Asmodean;

namespace Grisaia.Categories.Sprites {
	internal class SpriteGame : SpriteCategory<string, SpriteGame> {
		#region Fields

		/// <summary>
		///  Gets the index of the game info in the database.
		/// </summary>
		public int GameIndex { get; set; }
		/// <summary>
		///  Gets the game info associated with this category.
		/// </summary>
		public GameInfo GameInfo { get; set; }

		#endregion

		#region SpriteCategory Overrides

		/// <summary>
		///  Gets the sprite category entry for this category.
		/// </summary>
		public override SpriteCategoryInfo Category => SpriteCategoryPool.Game;
		/// <summary>
		///  Compares the order between this sprite element and another.
		/// </summary>
		/// <param name="other">The sprite element to compare.</param>
		/// <returns>The comparison between this sprite element and <paramref name="other"/>.</returns>
		public override int CompareTo(SpriteGame other) => GameIndex.CompareTo(other.GameIndex);

		#endregion
		
		#region ToString Override

		public override string ToString() => GameInfo.JPShortName;

		#endregion
	}
	internal class SpriteCharacter : SpriteCategory<string, SpriteCharacter> {
		#region Fields

		/// <summary>
		///  Gets the character info associated with this category.
		/// </summary>
		public CharacterInfo CharacterInfo { get; set; }

		#endregion
		
		#region SpriteCategory Overrides

		/// <summary>
		///  Gets the sprite category entry for this category.
		/// </summary>
		public override SpriteCategoryInfo Category => SpriteCategoryPool.Character;
		/// <summary>
		///  Compares the order between this sprite element and another.
		/// </summary>
		/// <param name="other">The sprite element to compare.</param>
		/// <returns>The comparison between this sprite element and <paramref name="other"/>.</returns>
		public override int CompareTo(SpriteCharacter other) => string.Compare(ToString(), other.ToString(), false);

		#endregion

		#region ToString Override

		public override string ToString() => $"{CharacterInfo.Name} ({Id})";

		#endregion
	}
	internal class SpriteCharacterLighting : SpriteCategory<SpriteLighting, SpriteCharacterLighting> {
		#region SpriteCategory Overrides

		/// <summary>
		///  Gets the sprite category entry for this category.
		/// </summary>
		public override SpriteCategoryInfo Category => SpriteCategoryPool.Lighting;

		#endregion

		#region ToString Override

		public override string ToString() => AttributeHelper.GetName(Id);

		#endregion
	}
	internal class SpriteCharacterDistance : SpriteCategory<SpriteDistance, SpriteCharacterDistance> {
		#region SpriteCategory Overrides

		/// <summary>
		///  Gets the sprite category entry for this category.
		/// </summary>
		public override SpriteCategoryInfo Category => SpriteCategoryPool.Distance;

		#endregion

		#region ToString Override

		public override string ToString() => AttributeHelper.GetName(Id);

		#endregion
	}
	/*internal class SpriteCharacterSize : SpriteCategory<SpriteSize, SpriteCharacterSize> {
		#region SpriteCategory Overrides

		/// <summary>
		///  Gets the sprite category entry for this category.
		/// </summary>
		public override SpriteCategoryInfo Category => SpriteCategoryPool.Size;

		#endregion

		#region ToString Override

		public override string ToString() => AttributeHelper.GetName(Id);

		#endregion
	}*/
	internal class SpriteCharacterPose : SpriteCategory<int, SpriteCharacterPose> {
		#region SpriteCategory Overrides

		/// <summary>
		///  Gets the sprite category entry for this category.
		/// </summary>
		public override SpriteCategoryInfo Category => SpriteCategoryPool.Pose;

		#endregion
	}
	internal class SpriteCharacterBlush : SpriteCategory<SpriteBlush, SpriteCharacterBlush> {
		#region SpriteCategory Overrides

		/// <summary>
		///  Gets the sprite category entry for this category.
		/// </summary>
		public override SpriteCategoryInfo Category => SpriteCategoryPool.Blush;

		#endregion

		#region ToString Override

		public override string ToString() => AttributeHelper.GetName(Id);

		#endregion
	}
}
