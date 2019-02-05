
using System.ComponentModel;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  A sprite category for a specific Grisaia game.
	/// </summary>
	internal sealed class SpriteGame : SpriteCategory<string, SpriteGame>, ISpriteGame {
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

		#region Events

		/// <summary>
		///  Raised when <see cref="ISpriteCategory.DisplayName"/> is updated.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		/// <summary>
		///  Raises the changed made to <see cref="ISpriteCategory.DisplayName"/>.
		/// </summary>
		public void RaiseDisplayNameChanged() {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
		}

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

		/// <summary>
		///  Gets the string representation of the sprite category.
		/// </summary>
		/// <returns>The sprite category's string representation.</returns>
		public override string ToString() => GameInfo.FormattedName;

		#endregion
	}
	/// <summary>
	///  A sprite category for a specific Grisaia character.
	/// </summary>
	internal sealed class SpriteCharacter : SpriteCategory<string, SpriteCharacter>, ISpriteCharacter {
		#region Fields

		/// <summary>
		///  Gets the character info associated with this category.
		/// </summary>
		public CharacterInfo CharacterInfo { get; set; }

		#endregion

		#region Events

		/// <summary>
		///  Raised when <see cref="ISpriteCategory.DisplayName"/> is updated.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		/// <summary>
		///  Raises the changed made to <see cref="ISpriteCategory.DisplayName"/>.
		/// </summary>
		public void RaiseDisplayNameChanged() {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
		}

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

		/// <summary>
		///  Gets the string representation of the sprite category.
		/// </summary>
		/// <returns>The sprite category's string representation.</returns>
		public override string ToString() => CharacterInfo.FormattedName;

		#endregion
	}
	/// <summary>
	///  A sprite category for a specific Grisaia character's lighting.
	/// </summary>
	internal sealed class SpriteCharacterLighting : SpriteCategory<SpriteLighting, SpriteCharacterLighting> {
		#region SpriteCategory Overrides

		/// <summary>
		///  Gets the sprite category entry for this category.
		/// </summary>
		public override SpriteCategoryInfo Category => SpriteCategoryPool.Lighting;

		#endregion

		#region ToString Override

		/// <summary>
		///  Gets the string representation of the sprite category.
		/// </summary>
		/// <returns>The sprite category's string representation.</returns>
		public override string ToString() => AttributeHelper.GetName(Id);

		#endregion
	}
	/// <summary>
	///  A sprite category for a specific Grisaia character's draw distance.
	/// </summary>
	internal sealed class SpriteCharacterDistance : SpriteCategory<SpriteDistance, SpriteCharacterDistance> {
		#region SpriteCategory Overrides

		/// <summary>
		///  Gets the sprite category entry for this category.
		/// </summary>
		public override SpriteCategoryInfo Category => SpriteCategoryPool.Distance;

		#endregion

		#region ToString Override

		/// <summary>
		///  Gets the string representation of the sprite category.
		/// </summary>
		/// <returns>The sprite category's string representation.</returns>
		public override string ToString() => AttributeHelper.GetName(Id);

		#endregion
	}

	/*/// <summary>
	///  A sprite category for a specific Grisaia character's draw size.
	/// </summary>
	internal sealed class SpriteCharacterSize : SpriteCategory<SpriteSize, SpriteCharacterSize> {
		#region SpriteCategory Overrides

		/// <summary>
		///  Gets the sprite category entry for this category.
		/// </summary>
		public override SpriteCategoryInfo Category => SpriteCategoryPool.Size;

		#endregion

		#region ToString Override
		
		/// <summary>
		///  Gets the string representation of the sprite category.
		/// </summary>
		/// <returns>The sprite category's string representation.</returns>
		public override string ToString() => AttributeHelper.GetName(Id);

		#endregion
	}*/
	/// <summary>
	///  A sprite category for a specific Grisaia character's pose.
	/// </summary>
	internal sealed class SpriteCharacterPose : SpriteCategory<SpritePose, SpriteCharacterPose> {
		#region SpriteCategory Overrides

		/// <summary>
		///  Gets the sprite category entry for this category.
		/// </summary>
		public override SpriteCategoryInfo Category => SpriteCategoryPool.Pose;

		#endregion

		#region ToString Override

		/// <summary>
		///  Gets the string representation of the sprite category.
		/// </summary>
		/// <returns>The sprite category's string representation.</returns>
		public override string ToString() => AttributeHelper.GetName(Id);

		#endregion
	}
	/// <summary>
	///  A sprite category for a specific Grisaia character's blush level.
	/// </summary>
	internal sealed class SpriteCharacterBlush : SpriteCategory<SpriteBlush, SpriteCharacterBlush> {
		#region SpriteCategory Overrides

		/// <summary>
		///  Gets the sprite category entry for this category.
		/// </summary>
		public override SpriteCategoryInfo Category => SpriteCategoryPool.Blush;

		#endregion

		#region ToString Override

		/// <summary>
		///  Gets the string representation of the sprite category.
		/// </summary>
		/// <returns>The sprite category's string representation.</returns>
		public override string ToString() => AttributeHelper.GetName(Id);

		#endregion
	}
}
