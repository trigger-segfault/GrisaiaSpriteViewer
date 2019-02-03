using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using Grisaia.Utils;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  An observable class storing an entire selection for a character sprite.
	/// </summary>
	public class ObservableSpriteSelection : ObservableObject, ISpriteSelection {
		#region Fields

		// Primary: 157 Total
		/// <summary>
		///  The Id for the sprite's Grisaia game.
		/// </summary>
		private string gameId; // 9 Total (really 7-8)
		/// <summary>
		///  The Id for the sprite's Grisaia character.
		/// </summary>
		private string charId; // 70 Total, 157 Total in all games

		// Secondary: 360 Total
		/// <summary>
		///  The Id for the sprite's lighting level.
		/// </summary>
		private SpriteLighting lighting; // 3 Total
		/// <summary>
		///  The Id for the sprite's draw distance.
		/// </summary>
		private SpriteDistance distance; // 10 Total (3 single-use cases)
		/// <summary>
		///  The Id for the sprite's character pose.
		/// </summary>
		private SpritePose pose; // 3 Total
		/// <summary>
		///  The Id for the sprite's blush level.
		/// </summary>
		private SpriteBlush blush; // 4 Total
		
		// Parts:
		/// <summary>
		///  Gets the sprite part group part Id selections.
		/// </summary>
		public ObservableArray<int> GroupPartIds { get; }
		IList<int> ISpriteSelection.GroupPartIds => GroupPartIds.ToArray();
		IReadOnlyList<int> IReadOnlySpriteSelection.GroupPartIds => GroupPartIds;
		/// <summary>
		///  Gets the sprite part group part frame index selections.
		/// </summary>
		public ObservableArray<int> GroupPartFrames { get; }
		IList<int> ISpriteSelection.GroupPartFrames => GroupPartFrames;
		IReadOnlyList<int> IReadOnlySpriteSelection.GroupPartFrames => GroupPartFrames;

		#endregion

		#region Constuctors

		/// <summary>
		///  Constructs the observable sprite selection and sets the group part Ids to nothing.
		/// </summary>
		public ObservableSpriteSelection() {
			GameId = string.Empty;
			CharacterId = string.Empty;
			GroupPartIds = new ObservableArray<int>(SpriteSelection.PartCount);
			for (int i = 0; i < SpriteSelection.PartCount; i++)
				GroupPartIds[i] = SpriteSelection.NoPart;
			GroupPartFrames = new ObservableArray<int>(SpriteSelection.PartCount);
		}
		/// <summary>
		///  Constructs the an observable copy of the sprite selection.
		/// </summary>
		/// <param name="spriteSelection">The sprite selection to make a copy of.</param>
		public ObservableSpriteSelection(IReadOnlySpriteSelection spriteSelection) {
			GameId      = spriteSelection.GameId;
			CharacterId = spriteSelection.CharacterId;

			Lighting    = spriteSelection.Lighting;
			Distance    = spriteSelection.Distance;
			Pose        = spriteSelection.Pose;
			Blush       = spriteSelection.Blush;

			GroupPartIds    = new ObservableArray<int>(spriteSelection.GroupPartIds);
			GroupPartFrames = new ObservableArray<int>(spriteSelection.GroupPartFrames);
		}

		#endregion

		#region Properties

		// Primary: 157 Total
		/// <summary>
		///  Gets or sets the Id for the sprite's Grisaia game.
		/// </summary>
		public string GameId {
			get => gameId;
			set => Set(ref gameId, value);
		}
		/// <summary>
		///  Gets or sets the Id for the sprite's Grisaia character.
		/// </summary>
		public string CharacterId {
			get => charId;
			set => Set(ref charId, value);
		}

		// Secondary: 360 Total
		/// <summary>
		///  Gets or sets the Id for the sprite's lighting level.
		/// </summary>
		public SpriteLighting Lighting {
			get => lighting;
			set => Set(ref lighting, value);
		}
		/// <summary>
		///  Gets or sets the Id for the sprite's draw distance.
		/// </summary>
		public SpriteDistance Distance {
			get => distance;
			set => Set(ref distance, value);
		}
		/// <summary>
		///  Gets or sets the Id for the sprite's character pose.
		/// </summary>
		public SpritePose Pose {
			get => pose;
			set => Set(ref pose, value);
		}
		/// <summary>
		///  Gets or sets the Id for the sprite's blush level.
		/// </summary>
		public SpriteBlush Blush {
			get => blush;
			set => Set(ref blush, value);
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
		private int PrimaryHashCode => SpriteSelection.GetPrimaryHashCode(GameId, CharacterId);
		/// <summary>
		///  Gets the <see cref="Lighting"/>, <see cref="Distance"/>, <see cref="Pose"/>, <see cref="Blush"/>
		///  as a single hash code.
		/// </summary>
		private int SecondaryHashCode => SpriteSelection.GetSecondaryHashCode(Lighting, Distance, Pose, Blush);
		/// <summary>
		///  Gets the <see cref="GroupPartIds"/>, as a single hash code.
		/// </summary>
		private int GroupPartsHashCode => SpriteSelection.GetGroupPartsHashCode(GroupPartIds, GroupPartFrames);
		
		#endregion
	}
}
