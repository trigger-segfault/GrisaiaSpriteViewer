using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;
using Grisaia.Json;
using Grisaia.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Grisaia.Categories {
	/// <summary>
	///  A list of settings defining how character infos should display their name.
	/// </summary>
	public sealed class CharacterNamingScheme : ObservableObject {
		#region Constants

		/// <summary>
		///  The regex used to identify tokens to replace with a special name.
		/// </summary>
		private static readonly Regex TokenRegex = new Regex(@"\$(?:(?'other'\w+):)?(?'token'\w+)\$");

		#endregion

		#region Fields

		/// <summary>
		///  True if all naming rules should be ignored and the Id should be used instead.
		/// </summary>
		[JsonIgnore]
		private bool onlyId = false;
		/// <summary>
		///  True if the Id is appended to the end of the name when <see cref="IdOnly"/> is false.
		/// </summary>
		[JsonIgnore]
		private bool appendId = false;
		/// <summary>
		///  How a character's first and last name are displayed.
		/// </summary>
		[JsonIgnore]
		private CharacterRealNameType realName = CharacterRealNameType.FirstLast;
		/// <summary>
		///  The order used to determine which character naming method goes first.
		/// </summary>
		[JsonIgnore]
		private CharacterNamingOrder order = new CharacterNamingOrder();

		#endregion

		#region Properties

		/// <summary>
		///  Gets or sets if all naming rules should be ignored and the Id should be used instead.
		/// </summary>
		[JsonProperty("id_only")]
		public bool IdOnly {
			get => onlyId;
			set => Set(ref onlyId, value);
		}
		/// <summary>
		///  Gets or sets if the Id is appended to the end of the name when <see cref="IdOnly"/> is false.
		/// </summary>
		[JsonProperty("append_id")]
		public bool AppendId {
			get => appendId;
			set => Set(ref appendId, value);
		}
		/// <summary>
		///  Gets how a character's first and last name are displayed.
		/// </summary>
		[JsonProperty("real_name")]
		public CharacterRealNameType RealName {
			get => realName;
			set {
				if (!Enum.IsDefined(typeof(CharacterRealNameType), value))
					throw new ArgumentException($"{nameof(CharacterRealNameType)} {value} is undefined!",
						nameof(RealName));
				Set(ref realName, value);
			}
		}
		/// <summary>
		///  Gets the order used to determine which character naming method goes first.
		/// </summary>
		[JsonProperty("order")]
		public CharacterNamingOrder Order {
			get => order;
			set {
				if (value == null)
					throw new ArgumentNullException(nameof(Order));
				Set(ref order, value);
			}
		}

		#endregion

		#region GetName

		/// <summary>
		///  Formats the name of the specified character info using the naming scheme settings.
		/// </summary>
		/// <param name="character">The character with the name to format.</param>
		/// <returns>The character's name with the naming scheme applied.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="character"/> is null.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		///  A name token is invalid or a token character does not have a name for the specified token.
		/// </exception>
		public string GetName(CharacterInfo character) {
			if (character == null)
				throw new ArgumentNullException(nameof(character));
			if (IdOnly)
				return character.Id;
			
			StringBuilder str = new StringBuilder();

			foreach (CharacterNameType type in Order) {
				switch (type) {
				case CharacterNameType.RealName: str.Append(GetRealFullName(character)); break;
				case CharacterNameType.Nickname: str.Append(character.Nickname); break;
				case CharacterNameType.Title:    str.Append(character.Title); break;
				}
				if (str.Length != 0)
					break; // We've found a naming method with an existing name to use.
			}

			if (str.Length != 0) {
				if (character.Label != null)
					str.Append($" ({character.Label})");

				// Apply tokens here:
				MatchCollection matches = TokenRegex.Matches(str.ToString());
				foreach (Match match in matches.Cast<Match>().Reverse()) {
					string other = match.Groups["other"].Value;
					string token = match.Groups["token"].Value;
					CharacterInfo tokenCharacter;
					switch (other) {
					case "":
						tokenCharacter = character;
						break;
					case "PARENT":
						tokenCharacter = character.Parent ??
							throw new InvalidOperationException($"Character \"{character.Id}\" does not have parent!");
						break;
					case "RELATION":
						tokenCharacter = character.Relation ??
							throw new InvalidOperationException($"Character \"{character.Id}\" does not have relation!");
						break;
					default:
						throw new InvalidOperationException($"Unknown other character token \"{match.Value}\"!");
					}
					string newValue = GetTokenName(tokenCharacter, token);
					str.Remove(match.Index, match.Length);
					str.Insert(match.Index, newValue);
				}

				// Append the Id if requested.
				if (AppendId)
					str.Append($" \"{character.Id}\"");
				return str.ToString();
			}

			// Last resort, we should not have to end up here.
			return character.Id;
		}
		/// <summary>
		///  Formats the full name of the specified character info using the naming scheme settings.
		/// </summary>
		/// <param name="character">The character with the name to format.</param>
		/// <returns>
		///  The character's full name with the first/last naming scheme applied.<para/>
		///  Returns null if the character does not have a first or last name.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="character"/> is null.
		/// </exception>
		public string GetRealFullName(CharacterInfo character) {
			if (character == null)
				throw new ArgumentNullException(nameof(character));

			if (character.FirstName == null) {
				if (character.LastName == null)
					return null;
				return character.LastName;
			}
			else if (character.LastName == null) {
				return character.FirstName;
			}
			switch (RealName) {
			case CharacterRealNameType.First:
				return character.FirstName;
			case CharacterRealNameType.FirstLast:
				return $"{character.FirstName} {character.LastName}";
			case CharacterRealNameType.LastFirst:
				return $"{character.LastName} {character.FirstName}";
			default:
				throw new ArgumentException($"Invalid {nameof(RealName)}!");
			}
		}
		/// <summary>
		///  Formats the single name of the specified character info using the naming scheme settings.
		/// </summary>
		/// <param name="character">The character with the name to format.</param>
		/// <returns>
		///  The character's single name with the first/last naming scheme applied.<para/>
		///  Returns null if the character does not have a first or last name.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="character"/> is null.
		/// </exception>
		public string GetRealName(CharacterInfo character) {
			if (character == null)
				throw new ArgumentNullException(nameof(character));

			if (character.FirstName == null) {
				if (character.LastName == null)
					return null;
				return character.LastName;
			}
			else if (character.LastName == null) {
				return character.FirstName;
			}
			switch (RealName) {
			case CharacterRealNameType.First:
			case CharacterRealNameType.FirstLast:
				return character.FirstName;
			case CharacterRealNameType.LastFirst:
				return character.LastName;
			default:
				throw new ArgumentException($"Invalid {nameof(RealName)}!");
			}
		}
		/// <summary>
		///  Gets the token name for a specific character.
		/// </summary>
		/// <param name="c">The character to get the name of.</param>
		/// <param name="token">The token for the type of name to get.</param>
		/// <returns>The character's name based on the token.</returns>
		/// 
		/// <exception cref="InvalidOperationException">
		///  <paramref name="token"/> is invalid or <paramref name="c"/> does not have a name for the specified token.
		/// </exception>
		private string GetTokenName(CharacterInfo c, string token) {
			string value;
			switch (token) {
			case "NAME":
			case "FULLNAME":
				value = (token == "NAME" ? GetRealName(c) : GetRealFullName(c));
				if (value == null)
					throw new InvalidOperationException($"Character \"{c.Id}\" does not have a first or last name!");
				break;
			case "NICK":
				value = c.Nickname;
				if (value == null)
					throw new InvalidOperationException($"Character \"{c.Id}\" does not have a nickname!");
				break;
			case "TITLE":
				value = c.Title;
				if (value == null)
					throw new InvalidOperationException($"Character \"{c.Id}\" does not have a title!");
				break;
			default:
				throw new InvalidOperationException($"Unknown token type \"${token}$\"!");
			}
			return value;
		}

		#endregion

		#region Clone

		/// <summary>
		///  Creates a deep clone of this character naming scheme.
		/// </summary>
		/// <returns>The clone of this character naming scheme.</returns>
		public CharacterNamingScheme Clone() {
			return new CharacterNamingScheme {
				IdOnly = IdOnly,
				AppendId = AppendId,
				RealName = RealName,
				Order = Order.Clone(),
			};
		}

		#endregion
	}
	/// <summary>
	///  An enumeration deciding how character's first and last names are displayed.
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum CharacterRealNameType {
		/// <summary>Only the character's first name will be displayed.</summary>
		[JsonProperty("first")]
		[Name("First")]
		First,
		/// <summary>The character's first then last name will be displayed.</summary>
		[JsonProperty("first_last")]
		[Name("First Last")]
		FirstLast,
		/// <summary>The character's last then first name will be displayed.</summary>
		[JsonProperty("last_first")]
		[Name("Last First")]
		LastFirst,
	}
	/// <summary>
	///  An enum used in <see cref="CharacterNamingOrder"/> to determine what type of naming method goes first.
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum CharacterNameType {
		/// <summary>Character's real name.</summary>
		[JsonProperty("real_name")]
		[Name("Real Name")]
		RealName,
		/// <summary>Character's nickname.</summary>
		[JsonProperty("nickname")]
		[Name("Nickname")]
		Nickname,
		/// <summary>Character's title.</summary>
		[JsonProperty("title")]
		[Name("Title")]
		Title,
	}
	/// <summary>
	///  The order used to determine which character naming method goes first.
	/// </summary>
	[JsonConverter(typeof(JsonCharacterNamingOrderConverter))]
	public sealed class CharacterNamingOrder : ObservableArray<CharacterNameType> {
		#region Converters

		private class JsonCharacterNamingOrderConverter : JsonConverter {
			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
				JToken token = JToken.Load(reader);
				if (token.Type == JTokenType.Array) {
					return new CharacterNamingOrder(token.ToObject<CharacterNameType[]>());
				}
				throw new InvalidOperationException("Value must be an array!");
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				CharacterNamingOrder order = (CharacterNamingOrder) value;
				value = order.ToArray();
				serializer.Serialize(writer, value);
			}

			public override bool CanConvert(Type objectType) => (objectType == typeof(CharacterNamingOrder));

			public override bool CanWrite => true;
		}

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs the character naming order using the default order of Nickname, FirstLast, then Title.
		/// </summary>
		public CharacterNamingOrder()
			: base(new[] { // Default order
				CharacterNameType.Nickname,
				CharacterNameType.RealName,
				CharacterNameType.Title,
			})
		{
		}
		/// <summary>
		///  Constructs the character naming order using an existing order.
		/// </summary>
		/// <param name="order">The existing order to copy.</param>
		internal CharacterNamingOrder(IReadOnlyList<CharacterNameType> order) : base(order) {
			// Make sure someone didn't fudge the config file
			HashSet<CharacterNameType> used = new HashSet<CharacterNameType>();
			foreach (CharacterNameType type in order) {
				if (!used.Add(type))
					throw new ArgumentException($"Character name type \"{type}\" is already contained in array!");
			}
		}

		#endregion

		#region Mutators

		public void Swap(CharacterNameType typeA, CharacterNameType typeB) {
			if (!Enum.IsDefined(typeof(CharacterNameType), typeA))
				throw new ArgumentException($"{nameof(CharacterNameType)} {typeA} is invalid!", nameof(typeA));
			if (!Enum.IsDefined(typeof(CharacterNameType), typeB))
				throw new ArgumentException($"{nameof(CharacterNameType)} {typeB} is invalid!", nameof(typeB));
			Swap(IndexOf(typeA), IndexOf(typeB));
		}
		public void MoveUp(CharacterNameType type) {
			if (!Enum.IsDefined(typeof(CharacterNameType), type))
				throw new ArgumentException($"{nameof(CharacterNameType)} {type} is invalid!", nameof(type));
			MoveUp(IndexOf(type));
		}
		public void MoveDown(CharacterNameType type) {
			if (!Enum.IsDefined(typeof(CharacterNameType), type))
				throw new ArgumentException($"{nameof(CharacterNameType)} {type} is invalid!", nameof(type));
			MoveDown(IndexOf(type));
		}
		public void Swap(int indexA, int indexB) {
			if (indexA < 0 || indexA >= Count)
				throw new ArgumentOutOfRangeException(nameof(indexA));
			if (indexB < 0 || indexB >= Count)
				throw new ArgumentOutOfRangeException(nameof(indexB));
			if (indexA == indexB)
				return; // Do nothing
			CharacterNameType orderSwap = this[indexA];
			this[indexA] = this[indexB];
			this[indexB] = orderSwap;
		}
		public void MoveUp(int index) {
			if (index < 0 || index >= Count)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (index == 0)
				return; // Do nothing
			Swap(index, index - 1);
		}
		public void MoveDown(int index) {
			if (index < 0 || index >= Count)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (index + 1 >= Count)
				return; // Do nothing
			Swap(index, index + 1);
		}

		#endregion

		#region Accessors

		/*public int IndexOf(CharacterNameType type) {
			if (!Enum.IsDefined(typeof(CharacterNameType), type))
				throw new ArgumentException($"{nameof(CharacterNameType)} {type} is undefined!", nameof(type));
			return ((IEnumerable<CharacterNameType>) this).IndexOf(type);
		}*/

		#endregion

		#region Clone

		/// <summary>
		///  Creates a deep clone of this character naming order.
		/// </summary>
		/// <returns>The clone of this character naming order.</returns>
		public CharacterNamingOrder Clone() => new CharacterNamingOrder(this);

		#endregion
		
		/*#region IReadOnlyList Implementation

		/// <summary>
		///  Gets the character naming type at the specified index in the order.
		/// </summary>
		/// <param name="index">The index of the character naming type.</param>
		/// <returns>The character naming type at the specified index.</returns>
		/// 
		/// <exception cref="IndexOutOfRangeException">
		///  <paramref name="index"/> is outside the bounds of the order.
		/// </exception>
		public CharacterNameType this[int index] => order[index];
		/// <summary>
		///  Gets the number of naming types in the order.
		/// </summary>
		public int Count => order.Count;

		/// <summary>
		///  Gets the enumerator for the character naming types.
		/// </summary>
		/// <returns>The order's character naming type enumerator.</returns>
		public IEnumerator<CharacterNameType> GetEnumerator() => order.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion*/
	}

	/*/// <summary>
	///  The settings for how a character's first and last name should be displayed.
	/// </summary>
	public sealed class FirstLastNamingScheme {
		#region Fields

		/// <summary>
		///  Gets or sets if the last name should be displayed only.
		/// </summary>
		[JsonProperty("last_name_first")]
		public bool LastNameFirst { get; set; } = false;
		/// <summary>
		///  Gets or sets if the name after the first listed name should not be displayed.
		/// </summary>
		[JsonProperty("no_second_name")]
		public bool NoSecondName { get; set; } = false;

		#endregion

		#region Clone

		/// <summary>
		///  Creates a deep clone of this character naming scheme.
		/// </summary>
		/// <returns>The clone of this character naming scheme.</returns>
		public FirstLastNamingScheme Clone() {
			return new FirstLastNamingScheme {
				LastNameFirst = LastNameFirst,
				NoSecondName = NoSecondName,
			};
		}

		#endregion

		#region GetName

		/// <summary>
		///  Formats the full name of the specified character info using the naming scheme settings.
		/// </summary>
		/// <param name="character">The character with the name to format.</param>
		/// <returns>
		///  The character's full name with the first/last naming scheme applied.<para/>
		///  Returns null if the character does not have a first or last name.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="character"/> is null.
		/// </exception>
		public string GetFullName(CharacterInfo character) {
			if (character == null)
				throw new ArgumentNullException(nameof(character));

			if (character.FirstName == null) {
				if (character.LastName == null)
					return null;
				return character.LastName;
			}
			else if (character.LastName == null) {
				return character.FirstName;
			}
			else if (LastNameFirst) {
				if (NoSecondName)
					return character.LastName;
				return $"{character.LastName} {character.FirstName}";
			}
			else {
				if (NoSecondName)
					return character.FirstName;
				return $"{character.FirstName} {character.LastName}";
			}
		}
		/// <summary>
		///  Formats the single name of the specified character info using the naming scheme settings.
		/// </summary>
		/// <param name="character">The character with the name to format.</param>
		/// <returns>
		///  The character's single name with the first/last naming scheme applied.<para/>
		///  Returns null if the character does not have a first or last name.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="character"/> is null.
		/// </exception>
		public string GetName(CharacterInfo character) {
			if (character == null)
				throw new ArgumentNullException(nameof(character));
			
			if (character.FirstName == null) {
				if (character.LastName == null)
					return null;
				return character.LastName;
			}
			else if (character.LastName == null) {
				return character.FirstName;
			}
			else if (LastNameFirst) {
				return character.LastName;
			}
			else {
				return character.FirstName;
			}
		}

		#endregion
	}*/
}
