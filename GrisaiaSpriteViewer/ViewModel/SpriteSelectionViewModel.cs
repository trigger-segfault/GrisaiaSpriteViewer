using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Grisaia.SpriteViewer.ViewModel {
	public class SpriteSelectionViewModel : ObservableObject {
		#region Fields

		private GameDatabase gameDb;
		private CharacterDatabase charDb;
		private SpriteDatabase spriteDb;

		private SpriteGame game;
		private SpriteCharacter character;
		private SpriteCharacterLighting lighting;
		private SpriteCharacterDistance distance;
		private SpriteCharacterPose pose;
		private SpriteCharacterBlush blush;
		private SpritePart[] parts;

		private GameInfo currentGame;
		private CharacterInfo currentCharacter;
		private CharacterSpritePartGroup[] currentGroups;
		private SpritePart[] currentParts;

		private string gameId;
		private string characterId;
		private SpriteLighting lightingId;
		private SpriteDistance distanceId;
		private int poseId;
		private SpriteBlush blushId;

		#endregion

		#region Properties

		/// <summary>
		///  Gets the game for this sprite.
		/// </summary>
		public SpriteGame Game {
			get => game;
			set {
				currentGame = gameDb.Get(value.Id);
				Set(ref game, value);
			}
		}

		#endregion

		#region UpdateChanged

		private void UpdateGameChanged() {

		}

		#endregion

		// Basics
		/// <summary>
		///  Gets the game identifier for this sprite.
		/// </summary>
		public string GameId {
			get => game.Id;
			set {
				currentGame = gameDb.Get(value);
				game = spriteDb.Games[value];
				RaisePropertyChanged(nameof(GameId));
			}
		}
		/// <summary>
		///  Gets the character identifier for this sprite.
		/// </summary>
		public string CharacterId { get; set; }

		// General
		/// <summary>
		///  Gets the lighting identifier for this sprite.
		/// </summary>
		public SpriteLighting LightingId { get; set; }
		/// <summary>
		///  Gets the distance identifier for this sprite.
		/// </summary>
		public SpriteDistance DistanceId { get; set; }
		/// <summary>
		///  Gets the pose identifier for this sprite.
		/// </summary>
		public int PoseId { get; set; }
		/// <summary>
		///  Gets the blush identifier for this sprite.
		/// </summary>
		public SpriteBlush BlushId { get; set; }

		// Parts
		/// <summary>
		///  Gets the part identifiers for this sprite. The index of the part is the type identifier.<para/>
		///  -1 means no sprite is selected. <see cref="int.MinValue"/> means the sprite cannot be set.
		/// </summary>
		public int[] SpritePartIds { get; } = new int[12];
	}
}
