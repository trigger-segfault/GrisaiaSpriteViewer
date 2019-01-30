using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Grisaia.Categories {
	/// <summary>
	///  The event args used with <see cref="LocateGamesProgressHandler"/>.
	/// </summary>
	public struct LocateGamesProgressArgs {
		/// <summary>
		///  The current game whose installation directory is being located.
		/// </summary>
		public GameInfo CurrentGame { get; internal set; }
		/// <summary>
		///  The index of the current located game being parsed.
		/// </summary>
		public int GameIndex { get; internal set; }
		/// <summary>
		///  The total number of located games to parse.
		/// </summary>
		public int GameCount { get; internal set; }
		/// <summary>
		///  The total number of games that have been located.
		/// </summary>
		public int LocatedGames { get; internal set; }
	}
	/// <summary>
	///  An event handler for use during the locating of a Grisia games.
	/// </summary>
	/// <param name="sender">The game database sending this callback.</param>
	/// <param name="e">The progress event args.</param>
	public delegate void LocateGamesProgressHandler(object sender, LocateGamesProgressArgs e);
	/// <summary>
	///  A database for storing all known character infos along with their information.
	/// </summary>
	[JsonObject]
	public sealed class CharacterDatabase : IReadOnlyCollection<CharacterInfo> {
		#region Fields

		/// <summary>
		///  Gets the grisaia database containing this database.
		/// </summary>
		[JsonIgnore]
		public GrisaiaDatabase GrisaiaDatabase { get; private set; }
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
		public CharacterSpritePartGroupInfo[] DefaultParts { get; private set; }
		/// <summary>
		///  The naming scheme used for characters.
		/// </summary>
		[JsonIgnore]
		private CharacterNamingScheme namingScheme = new CharacterNamingScheme();
		
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
				void SetParent(CharacterInfo c, HashSet<string> encountered) {
					if (c.ParentId == null)
						return;
					if (!encountered.Add(c.Id))
						throw new InvalidOperationException($"Infinite loop detected in parents with character " +
															$"\"{c.Id}\"!");
					if (!TryGetValue(c.ParentId, out CharacterInfo p))
						throw new KeyNotFoundException($"Invalid parent Id \"{c.ParentId}\" for character " +
													   $"\"{c.Id}\"!");
					// Hook up any dependency parents first
					SetParent(p, encountered);
					// Then assign the current parent
					c.Parent = p;
				}
				void SetRelation(CharacterInfo c, HashSet<string> encountered) {
					if (c.RelationId == null)
						return;
					if (!encountered.Add(c.Id))
						throw new InvalidOperationException($"Infinite loop detected in relations with character " +
															$"\"{c.Id}\"!");
					if (!TryGetValue(c.RelationId, out CharacterInfo r))
						throw new KeyNotFoundException($"Invalid relation Id \"{c.RelationId}\" for character " +
													   $"\"{c.Id}\"!");
					// Hook up any dependency relations first
					SetRelation(r, encountered);
					// Then assign the current relation
					c.Relation = r;
				}
				// First hook up the map
				foreach (CharacterInfo character in characterList) {
					character.Database = this;
					characterMap.Add(character.Id, character);
				}
				// Then assign parents and relations
				HashSet<string> encounteredIds = new HashSet<string>();
				foreach (CharacterInfo character in characterList) {
					encounteredIds.Clear();
					SetParent(character, encounteredIds);
					encounteredIds.Clear();
					SetRelation(character, encounteredIds);
				}
			}
		}
		/// <summary>
		///  Gets or sets the naming scheme used for characters.
		/// </summary>
		[JsonIgnore]
		public CharacterNamingScheme NamingScheme {
			get => namingScheme;
			set => namingScheme = value ?? throw new ArgumentNullException(nameof(NamingScheme));
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

		public bool TryGetValue(string id, out CharacterInfo character) => characterMap.TryGetValue(id, out character);

		public bool ContainsKey(string id) => characterMap.ContainsKey(id);
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
		public CharacterSpritePartGroupInfo[] GetPartGroups(GameInfo game, CharacterInfo character) {
			if (game == null)
				throw new ArgumentNullException(nameof(game));
			if (character == null)
				throw new ArgumentNullException(nameof(character));
			if (character.GameParts != null) {
				var group = character.GameParts.FirstOrDefault(gp => gp.GameIds.Any(id => id == game.Id))?.Parts;
				if (group != null)
					return group.ToArray();
			}
			return character.Parts?.ToArray() ?? DefaultParts.ToArray();
		}

		#endregion

		#region I/O

		/// <summary>
		///  Deserializes the character database from a json file.
		/// </summary>
		/// <param name="jsonFile">The path to the json file to load and deserialize.</param>
		/// <param name="grisaiaDb">The grisaia database containing all databases.</param>
		/// <returns>The deserialized character database.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="jsonFile"/> or <see cref="grisaiaDb"/> is null.
		/// </exception>
		public static CharacterDatabase FromJsonFile(string jsonFile, GrisaiaDatabase grisaiaDb) {
			if (jsonFile == null)
				throw new ArgumentNullException(nameof(jsonFile));
			var db = JsonConvert.DeserializeObject<CharacterDatabase>(File.ReadAllText(jsonFile));
			db.GrisaiaDatabase = grisaiaDb ?? throw new ArgumentNullException(nameof(grisaiaDb));
			return db;
		}

		#endregion

		#region IEnumerable Implementation

		/// <summary>
		///  Gets the enumerator for all character infos in the character database.
		/// </summary>
		/// <returns>The enumerator for all character infos.</returns>
		public IEnumerator<CharacterInfo> GetEnumerator() => characterList.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion
	}
}
