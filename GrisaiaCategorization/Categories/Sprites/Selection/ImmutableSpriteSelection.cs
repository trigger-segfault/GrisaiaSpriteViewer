using System;
using System.Collections.Generic;
using System.Linq;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  An immutable class storing an entire selection for a character sprite.
	/// </summary>
	public class ImmutableSpriteSelection : IReadOnlySpriteSelection {
		#region Fields

		// Primary: 157 Total
		/// <summary>
		///  Gets the Id for the sprite's Grisaia game.
		/// </summary>
		public string GameId { get; } = "";
		/// <summary>
		///  Gets the Id for the sprite's Grisaia character.
		/// </summary>
		public string CharacterId { get; } = "";

		// Secondary: 360 Total
		/// <summary>
		///  Gets the Id for the sprite's lighting level.
		/// </summary>
		public SpriteLighting Lighting { get; }
		/// <summary>
		///  Gets the Id for the sprite's draw distance.
		/// </summary>
		public SpriteDistance Distance { get; }
		/// <summary>
		///  Gets the Id for the sprite's character pose.
		/// </summary>
		public SpritePose Pose { get; }
		/// <summary>
		///  Gets the Id for the sprite's blush level.
		/// </summary>
		public SpriteBlush Blush { get; }

		// Parts:
		/// <summary>
		///  Gets the sprite part group part Id selections.
		/// </summary>
		public IReadOnlyList<int> GroupPartIds { get; }

		#endregion

		#region Constuctors

		/// <summary>
		///  Constructs the immutable sprite selection and sets the group part Ids to nothing.
		/// </summary>
		public ImmutableSpriteSelection() {
			int[] groupPartIds = new int[SpriteSelection.PartCount];
			for (int i = 0; i < SpriteSelection.PartCount; i++)
				groupPartIds[i] = SpriteSelection.NoPart;
			GroupPartIds = Array.AsReadOnly(groupPartIds);
		}
		/// <summary>
		///  Constructs the an immutable copy of the sprite selection.
		/// </summary>
		/// <param name="spriteSelection">The sprite selection to make a copy of.</param>
		public ImmutableSpriteSelection(IReadOnlySpriteSelection spriteSelection) {
			GameId       = spriteSelection.GameId;
			CharacterId  = spriteSelection.CharacterId;

			Lighting     = spriteSelection.Lighting;
			Distance     = spriteSelection.Distance;
			Pose         = spriteSelection.Pose;
			Blush        = spriteSelection.Blush;

			GroupPartIds = Array.AsReadOnly(spriteSelection.GroupPartIds.ToArray());
		}

		#endregion

		#region ToImmutable
		
		/// <summary>
		///  Creates a mutable clone of the sprite selection.
		/// </summary>
		/// <returns>The mutable copy of the sprite selection.</returns>
		public SpriteSelection ToMutable() => new SpriteSelection(this);
		ISpriteSelection IReadOnlySpriteSelection.ToMutable() => ToMutable();
		/// <summary>
		///  Creates an immutable clone of the sprite selection.
		/// </summary>
		/// <returns>The immutable copy of the sprite selection.</returns>
		public ImmutableSpriteSelection ToImmutable() => new ImmutableSpriteSelection(this);
		IReadOnlySpriteSelection IReadOnlySpriteSelection.ToImmutable() => ToImmutable();

		#endregion
		
		#region Equality

		/// <summary>
		///  Gets the hash code for the sprite selection.
		/// </summary>
		/// <returns>The hash code for the sprite selection.</returns>
		public override int GetHashCode() => PrimaryHashCode ^ SecondaryHashCode ^ GroupPartsHashCode;
		/// <summary>
		///  Checks if <paramref name="obj"/> is a <see cref="IReadOnlySpriteSelection"/> and equal to this sprite
		///  selection.
		/// </summary>
		/// <param name="obj">The object to compare to this sprite selection.</param>
		/// <returns>True if the object is a sprite selection and equal to this sprite selection.</returns>
		public override bool Equals(object obj) {
			if (obj is IReadOnlySpriteSelection selection)
				return Equals(selection);
			return false;
		}
		/// <summary>
		///  Checks if <paramref name="other"/> is equal to this sprite selection.
		/// </summary>
		/// <param name="other">The other sprite selection to compare to this sprite selection.</param>
		/// <returns>True if the other sprite selection is equal to this sprite selection.</returns>
		public bool Equals(IReadOnlySpriteSelection other) {
			return	GameId == other.GameId &&
					CharacterId == other.CharacterId &&
					Lighting == other.Lighting &&
					Distance == other.Distance &&
					Pose == other.Pose &&
					Blush == other.Blush &&
					GroupPartIds.SequenceEqual(other.GroupPartIds);
		}

		#endregion

		#region HashCode

		/// <summary>
		///  Gets the <see cref="GameId"/> and <see cref="CharacterId"/> as a single hash code.
		/// </summary>
		private int PrimaryHashCode => SpriteSelection.GetPrimaryHashCode(GameId, CharacterId);
		/// <summary>
		///  Gets the <see cref="Lighting"/>, <see cref="Distance"/>, <see cref="Pose"/>, <see cref="Blush"/>
		///  as a single hash code.
		/// </summary>
		private int SecondaryHashCode => SpriteSelection.GetSecondaryHashCode(Lighting, Distance, Pose, Blush);
		/// <summary>
		///  Gets the <see cref="GroupPartIds"/>, as a single hash code.
		/// </summary>
		private int GroupPartsHashCode => SpriteSelection.GetGroupPartsHashCode(GroupPartIds);
		
		#endregion
	}
}
