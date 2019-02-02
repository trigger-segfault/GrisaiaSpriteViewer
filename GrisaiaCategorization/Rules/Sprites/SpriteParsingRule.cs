using System;
using System.IO;
using System.Text.RegularExpressions;
using Grisaia.Categories.Sprites;

namespace Grisaia.Rules.Sprites {
	/// <summary>
	///  The base class for implementing a sprite parsing rule.
	/// </summary>
	/// 
	/// <remarks>
	///  Sprite Rules:
	///  Tmic01f_01
	///  |\ /||| |+- Part: 1-indexed (1-9,a-z,-,+,=)
	///  | | ||| +- PartType: number of padded 0's, No padded 0's is valid
	///  | | ||+- Distance: code (optional, SpriteDistance)
	///  | | |+- Value: 1-indexed (1-9,a-z,-,+,=) (occasionally 0-indexed)
	///  | | |+- Pose:  (Value > 3 ? ((Value - 1) % 3) + 1 : value) (+1 because of occasional 0-indexed)
	///  | | |+- Blush: (Value > 3 ? ((Value - 1) / 3) : value)
	///  | | +- Lighting: code (SpriteLighting)
	///  | +- CharacterId (see Characters.json)
	///  +- Sprite Identifier (always T)
	/// </remarks>
	public abstract class SpriteParsingRule : ISpriteParsingRule {
		#region Constants
		
		protected const string StartPattern		= @"^T";
		protected const string CharacterPattern	= @"(?'character'[A-Za-z]+)";
		protected const string LightingPattern	= @"(?'lighting'\d)";
		protected const string PosePattern		= @"(?'pose'" + NumberingRule.AlphaNumericPattern + @")";
		protected const string DistancePattern	= @"(?'distance'(?:[flmsSLt]?|lll|lb))"; //[flmsSL]?|lll
		//protected const string DistancePattern = @"(?'distance'(?:[flms]?|lll))";
		//protected const string SizePattern		= @"(?'size'[SL]?)";

		protected const string PartTypePattern	= @"(?'part_type'0*)";
		protected const string PartIdPattern	= @"(?'part_id'" + NumberingRule.AlphaNumericPattern + @")";
		protected const string EndPattern		= @"$";

		/// <summary>
		///  The numbering rule used in <see cref="TryParsePoseAndBlush"/>.
		/// </summary>
		private static readonly NumberingRule PoseBlushRule = new NumberingRule(NumberingOptions.AlphaNumeric, 1);
		/// <summary>
		///  The numbering rule used in <see cref="TryParsePart"/>.
		/// </summary>
		private static readonly NumberingRule PartRule = new NumberingRule(NumberingOptions.AlphaNumeric | NumberingOptions.ZeroIndexed, 1);

		#endregion

		#region Virtual Properties

		/// <summary>
		///  Gets the default regular expression used to parse the sprite.
		/// </summary>
		public abstract Regex SpriteRegex { get; }
		/// <summary>
		///  Gets the priority order of the parsing rule when parsing sprites.
		/// </summary>
		public abstract double Priority { get; }
		/// <summary>
		///  Gets the sprite should be ignored if matched.
		/// </summary>
		public virtual bool IgnoreSprite => false;

		#endregion

		#region Virtual Parsing

		/// <summary>
		///  Tries to parse the specified sprite based on it's file name.
		/// </summary>
		/// <param name="fileName">The file name of the sprite.</param>
		/// <param name="sprite">The output sprite info upon success, otherwise null.</param>
		/// <returns>True if the sprite was successfully parsed.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="fileName"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <paramref name="fileName"/> is an empty string.
		/// </exception>
		public bool TryParse(string fileName, out SpriteInfo sprite) {
			if (fileName == null)
				throw new ArgumentNullException(nameof(fileName));
			if (fileName.Length == 0)
				throw new ArgumentException($"{nameof(fileName)} cannot be an empty string!", nameof(fileName));
			sprite = null;
			SpriteInfo s = new SpriteInfo {
				FileName = fileName,
			};
			// Trim because some files contain spaces at the end. Yeah, it's dumb
			string file = Path.GetFileNameWithoutExtension(fileName).TrimEnd();
			Match m = SpriteRegex.Match(file);
			if (m.Success && TryParseMatch(s, m)) {
				sprite = s;
				return true;
			}
			return false;
		}
		/// <summary>
		///  Tries to parse the sprite's Regular Expression match from <see cref="SpriteRegex"/>.<para/>
		///  This is a virtual method that can be overridden to change how sprite categories are determined.
		/// </summary>
		/// <param name="s">The sprite being parsed.</param>
		/// <param name="m">The successful Regular Expression match from <see cref="SpriteRegex"/>.</param>
		/// <returns>True if the sprite was successfully parsed.</returns>
		protected virtual bool TryParseMatch(SpriteInfo s, Match m) {
			if (!TryParseCharacter(s, m))
				return false;

			if (!TryParseLighting(s, m))
				return false;
			if (!TryParsePoseAndBlush(s, m))
				return false;
			if (!TryParseDistance(s, m))
				return false;
			//if (!TryParseSize(s, m))
			//	return false;

			if (!TryParsePart(s, m))
				return false;
			return true;
		}

		#endregion

		#region TryParse

		/// <summary>
		///  Tries to parse the character's identifier name.
		/// </summary>
		/// <param name="s">The sprite info to assign the parsed value to.</param>
		/// <param name="m">The regex match to get the value to parse from.</param>
		/// <returns>True if the parse was successful, otherwise false.</returns>
		protected bool TryParseCharacter(SpriteInfo s, Match m) {
			s.CharacterId = m.Groups["character"].Value.ToLower();
			return true;
		}
		/// <summary>
		///  Tries to parse the character's identifier name and appends a string to the end of the -name.
		/// </summary>
		/// <param name="s">The sprite info to assign the parsed value to.</param>
		/// <param name="m">The regex match to get the value to parse from.</param>
		/// <param name="append">The extra part to append to the character identifier name.</param>
		/// <returns>True if the parse was successful, otherwise false.</returns>
		protected bool TryParseCharacter(SpriteInfo s, Match m, string append) {
			s.CharacterId = m.Groups["character"].Value.ToLower() + append;
			return true;
		}
		/// <summary>
		///  Tries to parse the sprite's lighting level.
		/// </summary>
		/// <param name="s">The sprite info to assign the parsed value to.</param>
		/// <param name="m">The regex match to get the value to parse from.</param>
		/// <returns>True if the parse was successful, otherwise false.</returns>
		protected bool TryParseLighting(SpriteInfo s, Match m) {
			if (AttributeHelper.TryParseCode(m.Groups["lighting"].Value, out SpriteLighting l)) {
				s.Lighting = l;
				return true;
			}
			return false;
		}
		/// <summary>
		///  Tries to parse the sprite's pose and blush level.
		/// </summary>
		/// <param name="s">The sprite info to assign the parsed value to.</param>
		/// <param name="m">The regex match to get the value to parse from.</param>
		/// <returns>True if the parse was successful, otherwise false.</returns>
		protected bool TryParsePoseAndBlush(SpriteInfo s, Match m) {
			if (!PoseBlushRule.TryParse(m.Groups["pose"].Value, out int pose))
				return false;
			if (pose < 0 || pose >= 12)
				return false;
			s.Pose  = (SpritePose)  (pose % 3);
			s.Blush = (SpriteBlush) (pose / 3);
			return true;
		}
		/// <summary>
		///  Tries to parse the sprite's draw distance.
		/// </summary>
		/// <param name="s">The sprite info to assign the parsed value to.</param>
		/// <param name="m">The regex match to get the value to parse from.</param>
		/// <returns>True if the parse was successful, otherwise false.</returns>
		protected bool TryParseDistance(SpriteInfo s, Match m) {
			if (AttributeHelper.TryParseCode(m.Groups["distance"].Value, out SpriteDistance d)) {
				s.Distance = d;
				return true;
			}
			return false;
		}
		/*/// <summary>
		///  Tries to parse the sprite's draw size.
		/// </summary>
		/// <param name="s">The sprite info to assign the parsed value to.</param>
		/// <param name="m">The regex match to get the value to parse from.</param>
		/// <returns>True if the parse was successful, otherwise false.</returns>
		protected bool TryParseSize(SpriteInfo s, Match m) {
			if (AttributeHelper.TryParseCode(m.Groups["size"].Value, out SpriteSize s)) {
				s.Size = s;
				return true;
			}
			return false;
		}*/
		/// <summary>
		///  Tries to parse the sprite's part type and Id.
		/// </summary>
		/// <param name="s">The sprite info to assign the parsed value to.</param>
		/// <param name="m">The regex match to get the value to parse from.</param>
		/// <returns>True if the parse was successful, otherwise false.</returns>
		protected bool TryParsePart(SpriteInfo s, Match m) {
			if (PartRule.TryParse(m.Groups["part_id"].Value, out int part)) {
				s.PartType = m.Groups["part_type"].Value.Length;
				s.PartId = part;
				return true;
			}
			return false;
		}
		
		#endregion

		#region IComparable Implementation

		/// <summary>
		///  Compares the order between this sprite parsing rule and another.
		/// </summary>
		/// <param name="obj">The sprite parsing rule to compare.</param>
		/// <returns>The comparison between this sprite parsing rule and <paramref name="obj"/>.</returns>
		public int CompareTo(object obj) {
			if (obj is SpriteParsingRule objRule) return CompareTo(objRule);
			throw new ArgumentException($"{nameof(obj)} is not a {GetType().Name}!");
		}
		/// <summary>
		///  Compares the order between this sprite parsing rule and another.
		/// </summary>
		/// <param name="other">The sprite parsing rule to compare.</param>
		/// <returns>The comparison between this sprite parsing rule and <paramref name="other"/>.</returns>
		public int CompareTo(SpriteParsingRule other) => Priority.CompareTo(other.Priority);

		#endregion
	}
}
