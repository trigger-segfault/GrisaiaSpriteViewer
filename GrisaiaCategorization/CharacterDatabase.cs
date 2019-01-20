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
		/// <summary>
		///  Gets the default character parts layout when one is not specified.
		/// </summary>
		[JsonProperty("default_parts")]
		public CharacterSpritePartGroup[] DefaultParts { get; private set; }
		
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
		///  Gets the number of total characters in the database.
		/// </summary>
		[JsonIgnore]
		public int Count => characterList.Count;
		/// <summary>
		///  Gets the list of known characters in the games.
		/// </summary>
		[JsonProperty("characters")]
		public IReadOnlyList<CharacterInfo> Characters {
			get => readonlyList;
			private set {
				characterList.Clear();
				characterMap.Clear();
				characterList.AddRange(value);
				foreach (CharacterInfo character in characterList) {
					foreach (string id in character.Ids)
						characterMap.Add(id, character);
				}
			}
		}
		/// <summary>
		///  Gets the character info at the specified index in the list.
		/// </summary>
		/// <param name="index">The index of the character info to get.</param>
		/// <returns>The character info at the specified index.</returns>
		/// 
		/// <exception cref="IndexOutOfRangeException">
		///  <paramref name="index"/> the index was outside the bounds of the list.
		/// </exception>
		[JsonIgnore]
		public CharacterInfo this[int index] => characterList[index];

		#endregion

		#region Accessors

		/*/// <summary>
		///  Gets the character info at the specified index in the list.
		/// </summary>
		/// <param name="index">The index of the character info to get.</param>
		/// <returns>The character info at the specified index.</returns>
		/// 
		/// <exception cref="IndexOutOfRangeException">
		///  <paramref name="index"/> the index was outside the bounds of the list.
		/// </exception>
		public CharacterInfo At(int index) {
			return characterList[index];
		}*/
		/// <summary>
		///  Gets the character info with the specified Id.
		/// </summary>
		/// <param name="id">The Id of the character info to get.</param>
		/// <returns>The character info with the specified Id.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="id"/> is null.
		/// </exception>
		public CharacterInfo Get(string id) {
			if (id == null)
				throw new ArgumentNullException(nameof(id));
			if (!characterMap.TryGetValue(id, out var characterInfo)) {
				Trace.WriteLine($"WARNING: Character Id \"{id}\" has not been defined!");
				characterInfo = CharacterInfo.MakeDefault(id, DefaultParts);
				characterMap.Add(id, characterInfo);
				characterList.Add(characterInfo);
			}
			return characterInfo;
		}
		/// <summary>
		///  Searches for the index of the character info in the list.
		/// </summary>
		/// <param name="character">The character to look for.</param>
		/// <returns>The index of the character if it was found, otherwise -1.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="character"/> is null.
		/// </exception>
		public int IndexOf(CharacterInfo character) {
			if (character == null)
				throw new ArgumentNullException(nameof(character));
			return characterList.IndexOf(character);
		}
		/// <summary>
		///  Searches for the index of the character info with the specified Id in the list.
		/// </summary>
		/// <param name="id">The Id of the character to look for.</param>
		/// <returns>The index of the character if it was found, otherwise -1.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="id"/> is null.
		/// </exception>
		public int IndexOf(string id) {
			if (id == null)
				throw new ArgumentNullException(nameof(id));
			return characterList.FindIndex(c => c.ContainsId(id));
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
