using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Grisaia.Categories {
	/// <summary>
	///  Sprite part groups and other information for a specific Grisaia character.
	/// </summary>
	public sealed class CharacterInfo {
		#region Fields
		
		/// <summary>
		///  Gets the character database containing this character info.
		/// </summary>
		[JsonIgnore]
		public CharacterDatabase Database { get; internal set; }
		/// <summary>
		///  The parent to this character info. Used to avoid duplicate information.
		/// </summary>
		[JsonIgnore]
		private CharacterInfo parent;
		/// <summary>
		///  The character info related to this character in same way. Used for $RELATION$ name token.
		/// </summary>
		[JsonIgnore]
		public CharacterInfo Relation { get; internal set; }
		/// <summary>
		///  Gets the Id associated with this character.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <summary>
		///  Gets the Id of the parent to this character info. Used to avoid duplicate information.<para/>
		///  Only the following fields are duplicated: <see cref="FirstName"/>, <see cref="LastName"/>,
		///  <see cref="Nickname"/>, <see cref="Title"/>.<para/>
		///  These fields are also only copied if the underlying field is null.
		/// </summary>
		[JsonProperty("parent_id")]
		public string ParentId { get; private set; }
		/// <summary>
		///  Gets the Id of the character info related to this character in same way. Used for $RELATION$ name token.
		/// </summary>
		[JsonProperty("relation_id")]
		public string RelationId { get; private set; }

		/// <summary>
		///  Gets the character's first name. This can be null.
		/// </summary>
		[JsonProperty("first_name")]
		public string FirstName { get; private set; }
		/// <summary>
		///  Gets the character's last name. This can be null.
		/// </summary>
		[JsonProperty("last_name")]
		public string LastName { get; private set; }
		/// <summary>
		///  Gets the character's nickname. This can be null.
		/// </summary>
		[JsonProperty("nickname")]
		public string Nickname { get; private set; }
		/// <summary>
		///  Gets the character's name as a title. This can be null.
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; private set; }
		/// <summary>
		///  Gets the character's label appended to their name. This can be null.
		/// </summary>
		[JsonProperty("label")]
		public string Label { get; private set; }

		// Extractor:
		/// <summary>
		///  Gets the sorting subgroup. Also functions as the label when none is present.
		/// </summary>
		[JsonProperty("subgroup")]
		public string SubGroup { get; private set; }

		// Sprite Viewer:
		/// <summary>
		///  Gets the default part groups associated with this character.
		/// </summary>
		[JsonProperty("parts")]
		public IReadOnlyList<CharacterSpritePartGroupInfo> Parts { get; private set; }
		/// <summary>
		///  Gets the list of game-specific part groups associated with this character.
		/// </summary>
		[JsonProperty("game_parts")]
		public IReadOnlyList<CharacterGameSpecificPartsInfo> GameParts { get; private set; }

		#endregion

		#region Properties

		/*/// <summary>
		///  Gets the single-Id representation of the character.
		/// </summary>
		[JsonProperty("id")]
		private string Id {
			get => Ids?.FirstOrDefault();
			set => Ids = new string[] { value };
		}*/

		/// <summary>
		///  Formats the name of the this character info using the naming scheme settings.
		/// </summary>
		/// <returns>The character's name with the naming scheme applied.</returns>
		/// 
		/// <exception cref="InvalidOperationException">
		///  A name token is invalid or a token character does not have a name for the specified token.
		/// </exception>
		[JsonIgnore]
		public string FormattedName => Database.NamingScheme.GetName(this);
		/// <summary>
		///  Gets the parent to this character info. Used to avoid duplicate information.
		/// </summary>
		[JsonIgnore]
		public CharacterInfo Parent {
			get => parent;
			internal set {
				parent = value ?? throw new ArgumentNullException(nameof(Parent));

				if (FirstName == null) FirstName = value.FirstName;
				if (LastName == null) LastName = value.LastName;
				if (Nickname == null) Nickname = value.Nickname;
				if (Title == null) Title = value.Title;
			}
		}

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
			return Id == id;
			/*if (id == null)
				throw new ArgumentNullException(nameof(id));
			return Array.IndexOf(Ids, id) != -1;*/
		}

		#endregion

		#region ToString Override

		/// <summary>
		///  Gets the string representation of the character info.
		/// </summary>
		/// <returns>The string representation of the character info.</returns>
		public override string ToString() => Id;

		#endregion

		#region MakeDefault

		/// <summary>
		///  Makes the default character info for an undefined character.
		/// </summary>
		/// <param name="id">The Id of the character.</param>
		/// <param name="defaultParts">The default parts to use for the character info.</param>
		/// <returns>The newly constructed character info.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="id"/> or <paramref name="defaultParts"/> is null.
		/// </exception>
		public static CharacterInfo MakeDefault(string id, CharacterSpritePartGroupInfo[] defaultParts) {
			if (id == null)
				throw new ArgumentNullException(nameof(id));
			if (defaultParts == null)
				throw new ArgumentNullException(nameof(defaultParts));
			return new CharacterInfo {
				Id = id,
				Parts = defaultParts,
			};
		}

		#endregion
	}

	/// <summary>
	///  A <see cref="CharacterSpritePartGroupInfo[]"/> for a specific game and character.
	/// </summary>
	public sealed class CharacterGameSpecificPartsInfo {
		#region Fields

		/// <summary>
		///  Gets the game Ids associated this part categorization.
		/// </summary>
		[JsonProperty("game_ids")]
		public IReadOnlyList<string> GameIds { get; private set; }
		/// <summary>
		///  Gets the list of part groups associated with this game and character.
		/// </summary>
		[JsonProperty("parts")]
		public IReadOnlyList<CharacterSpritePartGroupInfo> Parts { get; private set; }

		#endregion

		#region Properties

		/// <summary>
		///  Gets the single game Id.
		/// </summary>
		[JsonProperty("game_id")]
		private string GameId {
			get => GameIds?.FirstOrDefault();
			set => GameIds = Array.AsReadOnly(new string[] { value });
		}

		#endregion
	}
	/// <summary>
	///  A single group of sprites that form a single part of the entire character sprite.
	/// </summary>
	public sealed class CharacterSpritePartGroupInfo {
		#region Fields

		/// <summary>
		///  Gets the user-friendly name of the sprite part group.
		/// </summary>
		[JsonProperty("Name")]
		public string Name { get; private set; }
		/// <summary>
		///  Gets the part type Ids associated with this part group.
		/// </summary>
		[JsonProperty("ids")]
		public IReadOnlyList<int> TypeIds { get; private set; }
		/// <summary>
		///  Gets if this group is enabled by default when activated.
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
			set => TypeIds = Array.AsReadOnly(new int[] { value });
		}

		#endregion
	}
}
