using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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
		[JsonProperty("game_id")]
		public string GameId { get; private set; }
		/// <summary>
		///  Gets the Id for the sprite's Grisaia character.
		/// </summary>
		[JsonProperty("character_id")]
		public string CharacterId { get; private set; }

		// Secondary: 360 Total
		/// <summary>
		///  Gets the Id for the sprite's lighting level.
		/// </summary>
		[JsonProperty("lighting")]
		public SpriteLighting Lighting { get; private set; }
		/// <summary>
		///  Gets the Id for the sprite's draw distance.
		/// </summary>
		[JsonProperty("distance")]
		public SpriteDistance Distance { get; private set; }
		/// <summary>
		///  Gets the Id for the sprite's character pose.
		/// </summary>
		[JsonProperty("pose")]
		public SpritePose Pose { get; private set; }
		/// <summary>
		///  Gets the Id for the sprite's blush level.
		/// </summary>
		[JsonProperty("blush")]
		public SpriteBlush Blush { get; private set; }

		// Parts:
		/// <summary>
		///  Gets the sprite part group part Id selections.
		/// </summary>
		[JsonProperty("group_part_ids")]
		public IReadOnlyList<int> GroupPartIds { get; private set; }
		/// <summary>
		///  Gets the sprite part group part frame index selections.
		/// </summary>
		[JsonProperty("group_part_frames")]
		public IReadOnlyList<int> GroupPartFrames { get; private set; }

		#endregion

		#region Constuctors

		/// <summary>
		///  Constructs the immutable sprite selection and sets the group part Ids to nothing.
		/// </summary>
		public ImmutableSpriteSelection() {
			GameId = string.Empty;
			CharacterId = string.Empty;
			int[] groupPartIds = new int[SpriteSelection.PartCount];
			for (int i = 0; i < SpriteSelection.PartCount; i++)
				groupPartIds[i] = SpriteSelection.NoPart;
			GroupPartIds = Array.AsReadOnly(groupPartIds);
			GroupPartFrames = Array.AsReadOnly(new int[SpriteSelection.PartCount]);
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
			GroupPartFrames = Array.AsReadOnly(spriteSelection.GroupPartFrames.ToArray());
		}

		#endregion

		#region ToImmutable
		
		/// <summary>
		///  Creates a mutable clone of the sprite selection.
		/// </summary>
		/// <returns>The mutable copy of the sprite selection.</returns>
		public SpriteSelection ToMutable() => new SpriteSelection(this);
		/// <summary>
		///  Creates an immutable clone of the sprite selection.
		/// </summary>
		/// <returns>The immutable copy of the sprite selection.</returns>
		/// 
		/// <remarks>
		///  In this case, we can safely return the same object, because we are already immutable.
		/// </remarks>
		public ImmutableSpriteSelection ToImmutable() => this;

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
					GroupPartIds.SequenceEqual(other.GroupPartIds) &&
					GroupPartFrames.SequenceEqual(other.GroupPartFrames);
		}

		#endregion

		#region HashCode

		/// <summary>
		///  Gets the <see cref="GameId"/> and <see cref="CharacterId"/> as a single hash code.
		/// </summary>
		[JsonIgnore]
		private int PrimaryHashCode => SpriteSelection.GetPrimaryHashCode(GameId, CharacterId);
		/// <summary>
		///  Gets the <see cref="Lighting"/>, <see cref="Distance"/>, <see cref="Pose"/>, <see cref="Blush"/>
		///  as a single hash code.
		/// </summary>
		[JsonIgnore]
		private int SecondaryHashCode => SpriteSelection.GetSecondaryHashCode(Lighting, Distance, Pose, Blush);
		/// <summary>
		///  Gets the <see cref="GroupPartIds"/> and <see cref="GroupPartFrames"/>, as a single hash code.
		/// </summary>
		[JsonIgnore]
		private int GroupPartsHashCode => SpriteSelection.GetGroupPartsHashCode(GroupPartIds, GroupPartFrames);

		#endregion
	}
}
