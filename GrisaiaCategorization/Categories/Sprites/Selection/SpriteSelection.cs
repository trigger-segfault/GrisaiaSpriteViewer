using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  A mutable class storing an entire selection for a character sprite.
	/// </summary>
	public class SpriteSelection : ISpriteSelection {
		#region Constants

		private const int LightingCount  = (int) SpriteLighting.Count;
		private const int DistanceCount  = (int) SpriteDistance.Count;
		private const int PoseCount      = (int) SpritePose.Count;
		private const int BlushCount     = (int) SpriteBlush.Count;
		private const int LightingOffset = 1;
		private const int DistanceOffset = LightingCount;
		private const int PoseOffset     = LightingCount * DistanceCount;
		private const int BlushOffset    = LightingCount * DistanceCount * PoseCount;
		
		/// <summary>
		///  The total number of part group part Ids.
		/// </summary>
		public const int PartCount = 10;
		/// <summary>
		///  The value used to specify no part is selected.
		/// </summary>
		public const int NoPart = -1;

		#endregion

		#region Fields

		// Primary: 157 Total
		/// <summary>
		///  Gets or sets the Id for the sprite's Grisaia game.
		/// </summary>
		[JsonProperty("game_id")]
		public string GameId { get; set; }
		/// <summary>
		///  Gets or sets the Id for the sprite's Grisaia character.
		/// </summary>
		[JsonProperty("character_id")]
		public string CharacterId { get; set; }

		// Secondary: 360 Total
		/// <summary>
		///  Gets or sets the Id for the sprite's lighting level.
		/// </summary>
		[JsonProperty("lighting")]
		public SpriteLighting Lighting { get; set; }
		/// <summary>
		///  Gets or sets the Id for the sprite's draw distance.
		/// </summary>
		[JsonProperty("distance")]
		public SpriteDistance Distance { get; set; }
		/// <summary>
		///  Gets or sets the Id for the sprite's character pose.
		/// </summary>
		[JsonProperty("pose")]
		public SpritePose Pose { get; set; }
		/// <summary>
		///  Gets or sets the Id for the sprite's blush level.
		/// </summary>
		[JsonProperty("blush")]
		public SpriteBlush Blush { get; set; }

		// Parts:
		/// <summary>
		///  Gets or sets the sprite part group part Id selections.
		/// </summary>
		[JsonProperty("group_part_ids")]
		public int[] GroupPartIds { get; private set; }
		IList<int> ISpriteSelection.GroupPartIds => GroupPartIds;
		IReadOnlyList<int> IReadOnlySpriteSelection.GroupPartIds => GroupPartIds;
		/// <summary>
		///  Gets the sprite part group part frame index selections.
		/// </summary>
		[JsonProperty("group_part_frames")]
		public int[] GroupPartFrames { get; }
		IList<int> ISpriteSelection.GroupPartFrames => GroupPartFrames;
		IReadOnlyList<int> IReadOnlySpriteSelection.GroupPartFrames => GroupPartFrames;

		#endregion

		#region Constuctors

		/// <summary>
		///  Constructs the sprite selection and sets the group part Ids to nothing.
		/// </summary>
		public SpriteSelection() {
			GameId = string.Empty;
			CharacterId = string.Empty;
			GroupPartIds = new int[SpriteSelection.PartCount];
			for (int i = 0; i < SpriteSelection.PartCount; i++)
				GroupPartIds[i] = SpriteSelection.NoPart;
			GroupPartFrames = new int[SpriteSelection.PartCount];
		}
		/// <summary>
		///  Constructs the a mutable copy of the sprite selection.
		/// </summary>
		/// <param name="spriteSelection">The sprite selection to make a copy of.</param>
		public SpriteSelection(IReadOnlySpriteSelection spriteSelection) {
			GameId      = spriteSelection.GameId;
			CharacterId = spriteSelection.CharacterId;

			Lighting    = spriteSelection.Lighting;
			Distance    = spriteSelection.Distance;
			Pose        = spriteSelection.Pose;
			Blush       = spriteSelection.Blush;

			GroupPartIds    = spriteSelection.GroupPartIds.ToArray();
			GroupPartFrames = spriteSelection.GroupPartFrames.ToArray();
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
		public ImmutableSpriteSelection ToImmutable() => new ImmutableSpriteSelection(this);

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
		private int PrimaryHashCode => GetPrimaryHashCode(GameId, CharacterId);
		/// <summary>
		///  Gets the <see cref="Lighting"/>, <see cref="Distance"/>, <see cref="Pose"/>, <see cref="Blush"/>
		///  as a single hash code.
		/// </summary>
		[JsonIgnore]
		private int SecondaryHashCode => GetSecondaryHashCode(Lighting, Distance, Pose, Blush);
		/// <summary>
		///  Gets the <see cref="GroupPartIds"/>, as a single hash code.
		/// </summary>
		[JsonIgnore]
		private int GroupPartsHashCode => GetGroupPartsHashCode(GroupPartIds, GroupPartFrames);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int GetPrimaryHashCode(string gameId, string characterId) {
			return gameId.GetHashCode() ^ characterId.GetHashCode();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int GetGroupPartsHashCode(IReadOnlyList<int> ids, IReadOnlyList<int> frames) {
			return ids.Sum() ^ frames.Sum();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int GetSecondaryHashCode(SpriteLighting l, SpriteDistance d, SpritePose p, SpriteBlush b) {
			int id = 0;
			id |= (int) l * LightingOffset;
			id |= (int) d * DistanceOffset;
			id |= (int) p * PoseOffset;
			id |= (int) b * BlushOffset;
			return id;
		}

		#endregion
	}
}
