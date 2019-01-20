using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace Grisaia {
	/// <summary>
	///  A database for storing all known character infos along with their information.
	/// </summary>
	public sealed class CharacterDatabase {
		#region Fields

		/// <summary>
		///  The map of character infos by sprite Id.
		/// </summary>
		[JsonIgnore]
		private readonly Dictionary<string, CharacterInfo> characterMap = new Dictionary<string, CharacterInfo>();
		/// <summary>
		///  The mutable list of character infos.
		/// </summary>
		[JsonIgnore]
		private readonly List<CharacterInfo> characterList = new List<CharacterInfo>();
		/// <summary>
		///  The readonly copy of the character list.<para/>
		///  This is required because Newtonsoft.Json would write to the list otherwise.
		/// </summary>
		[JsonIgnore]
		private readonly IReadOnlyList<CharacterInfo> readonlyList;

		#endregion

		#region Constructors
		
		/// <summary>
		///  Constructs the character database and sets up the readonly list.
		/// </summary>
		public CharacterDatabase() {
			readonlyList = characterList.AsReadOnly();
		}
		
		#endregion

		#region Properties

		/// <summary>
		///  Gets the default character parts layout when one is not specified.
		/// </summary>
		[JsonProperty("default_parts")]
		public CharacterSpritePartGroup[] DefaultParts { get; private set; }
		/// <summary>
		///  Gets the list of known characters in the games.
		/// </summary>
		[JsonProperty("characters")]
		public IReadOnlyList<CharacterInfo> Characters {
			get => readonlyList;
			private set {
				//characterList = value;
				characterList.Clear();
				characterMap.Clear();
				characterList.AddRange(value);
				foreach (CharacterInfo character in value) {
					foreach (string id in character.Ids)
						characterMap.Add(id, character);
				}
			}
		}

		#endregion

		#region Accessors

		/// <summary>
		///  Gets the character info at the specified index in the list.
		/// </summary>
		/// <param name="index">The index of the character info to get.</param>
		/// <returns>The character info at the specified index.</returns>
		public CharacterInfo GetCharacterAt(int index) {
			return characterList[index];
		}
		/// <summary>
		///  Gets the character info with the specified Id.
		/// </summary>
		/// <param name="id">The Id of the character info to get.</param>
		/// <returns>The character info with the specified Id.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="id"/> is null.
		/// </exception>
		public CharacterInfo GetCharacter(string id) {
			if (id == null)
				throw new ArgumentNullException(nameof(id));
			if (!characterMap.TryGetValue(id, out var characterInfo)) {
				Trace.WriteLine($"WARNING: Character Id \"{id}\" has not been defined!");
				characterInfo = CharacterInfo.MakeDefault(id, DefaultParts);
				characterMap.Add(id, characterInfo);
				//characterList.Add(characterInfo);
			}
			return characterInfo;
		}
		/// <summary>
		///  Gets the part groups for a specified character from a specified game.
		/// </summary>
		/// <param name="game">The game to get the part groups for the character from.</param>
		/// <param name="character">The character to get the part groups for.</param>
		/// <returns>The part groups for the character.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="game"/> or <paramref name="character"/> is null.
		/// </exception>
		public CharacterSpritePartGroup[] GetPartGroup(GameInfo game, CharacterInfo character) {
			if (game == null)
				throw new ArgumentNullException(nameof(game));
			if (character == null)
				throw new ArgumentNullException(nameof(character));
			if (character.GameParts != null) {
				var partGroup = character.GameParts.FirstOrDefault(gp => gp.GameIds.Any(id => id == game.Id))?.Parts;
				if (partGroup != null)
					return partGroup;
			}
			return character.Parts ?? DefaultParts;
		}

		#endregion
	}
}
