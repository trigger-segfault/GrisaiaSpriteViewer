using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Grisaia {
	/// <summary>
	///  Sprite part groups and other information for a specific Grisaia character.
	/// </summary>
	public sealed class CharacterInfo {
		#region Fields

#pragma warning disable CS0649, IDE0044
		/// <summary>
		///  The character's real name.
		/// </summary>
		[JsonProperty("name")]
		private string name;
		/// <summary>
		///  The character's title, such as X's Father, etc.
		/// </summary>
		[JsonProperty("title")]
		private string title;
		/// <summary>
		///  The label for the character.
		/// </summary>
		[JsonProperty("label")]
		private string label;
#pragma warning restore CS0649, IDE0044

		/// <summary>
		///  Gets the Ids associated with this character.
		/// </summary>
		[JsonProperty("ids")]
		public string[] Ids { get; private set; }
		/*/// <summary>
		///  Gets the sorting subgroup. Also functions as the label when none is present.
		/// </summary>
		[JsonProperty("sub")]
		public string SubGroup { get; private set; }*/

		/// <summary>
		///  Gets the default part groups associated with this character.
		/// </summary>
		[JsonProperty("parts")]
		public CharacterSpritePartGroup[] Parts { get; private set; }

		/// <summary>
		///  Gets the list of game-specific part groups associated with this character.
		/// </summary>
		[JsonProperty("game_parts")]
		public CharacterGameSpecificParts[] GameParts { get; private set; }

		#endregion

		#region Properties

		/// <summary>
		///  Gets the single-Id representation of the character.
		/// </summary>
		[JsonProperty("id")]
		private string Id {
			get => Ids?.FirstOrDefault();
			set => Ids = new string[] { value };
		}

		/// <summary>
		///  Gets the user-friendly character's name, or title if they have no title.
		/// </summary>
		[JsonIgnore]
		public string Name => name ?? title;
		/// <summary>
		///  Gets the user-friendly character's title, or name if they have no title.
		/// </summary>
		[JsonIgnore]
		public string Title => title ?? name;
		/// <summary>
		///  Gets the additional label added to the character name.
		/// </summary>
		[JsonIgnore]
		public string Label => label;// ?? SubGroup;

		#endregion

		#region Accessors

		/// <summary>
		///  Returns true if this character info contains has the specified Id.
		/// </summary>
		/// <param name="id">The id to check for.</param>
		/// <returns>True if the character has the specified Id.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="id"/> is null.
		/// </exception>
		public bool ContainsId(string id) {
			if (id == null)
				throw new ArgumentNullException(nameof(id));
			return Array.IndexOf(Ids, id) != -1;
		}

		#endregion

		#region ToString Override

		public override string ToString() => Name + (Label != null ? " " + Label : "");

		#endregion

		#region MakeDefault

		/// <summary>
		///  Makes the default character info for an undefined character.
		/// </summary>
		/// <param name="id">The sprite Id of the character.</param>
		/// <param name="defaultParts">The default parts to use for the character info.</param>
		/// <returns>The newly constructed character info.</returns>
		public static CharacterInfo MakeDefault(string id, CharacterSpritePartGroup[] defaultParts) {
			return new CharacterInfo {
				Id = id,
				name = id,
				Parts = defaultParts,
			};
		}

		#endregion
	}

	/// <summary>
	///  A <see cref="CharacterSpritePartGroup[]"/> for a specific game and character.
	/// </summary>
	public sealed class CharacterGameSpecificParts {
		#region Fields

		/// <summary>
		///  Gets the game Ids associated this part categorization.
		/// </summary>
		[JsonProperty("game_ids")]
		public string[] GameIds { get; private set; }
		/// <summary>
		///  Gets the list of part groups associated with this game and character.
		/// </summary>
		[JsonProperty("parts")]
		public CharacterSpritePartGroup[] Parts { get; private set; }

		#endregion

		#region Properties

		/// <summary>
		///  Gets the single game Id.
		/// </summary>
		[JsonProperty("game_id")]
		private string GameId {
			get => GameIds?.FirstOrDefault();
			set => GameIds = new string[] { value };
		}

		#endregion
	}
	/// <summary>
	///  A single group of sprites that form a single part of the entire character sprite.
	/// </summary>
	public sealed class CharacterSpritePartGroup {
		#region Fields

		/// <summary>
		///  Gets the user-friendly name of the sprite part group.
		/// </summary>
		[JsonProperty("Name")]
		public string Name { get; private set; }
		/// <summary>
		///  Gets the type Ids associated with this part group.
		/// </summary>
		[JsonProperty("ids")]
		public int[] TypeIds { get; private set; }
		/// <summary>
		///  Gets if the part is enabled by default.
		/// </summary>
		[JsonProperty("enabled")]
		public bool Enabled { get; private set; } = true;

		#endregion

		#region Properties

		/// <summary>
		///  Gets the single part type Id.
		/// </summary>
		[JsonProperty("id")]
		private int TypeId {
			get => TypeIds.First();
			set => TypeIds = new int[] { value };
		}

		#endregion
	}
}
