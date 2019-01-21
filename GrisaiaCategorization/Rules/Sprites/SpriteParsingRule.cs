using System;
using System.IO;
using System.Text.RegularExpressions;
using Grisaia.Categories.Sprites;

namespace Grisaia.Rules.Sprites {
	/// <summary>
	/// 
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
	public class SpriteParsingRule : ISpriteParsingRule {
		#region Constants

		protected const string StartPattern		= @"^T";
		protected const string CharacterPattern	= @"(?'character'[A-Za-z]+)";
		protected const string LightingPattern	= @"(?'lighting'\d)";
		protected const string PosePattern		= @"(?'pose'" + NumberingRule.AlphaNumericPattern + @")";
		protected const string DistancePattern	= @"(?'distance'(?:[flmsSLt]?|lll|lb))"; //[flmsSL]?|lll
		//protected const string DistancePattern = @"(?'distance'(?:[flms]?|lll))";
		//protected const string SizePattern		= @"(?'size'[SL]?)";

		protected const string PartIdPattern	= @"(?'part_id'0*)";
		protected const string PartPattern		= @"(?'part'" + NumberingRule.AlphaNumericPattern + @")";
		protected const string EndPattern		= @"$";
		
		private const string Pattern =
			StartPattern +
			CharacterPattern +
			LightingPattern +
			PosePattern +
			DistancePattern +
			//SizePattern +
			@"_" +
			PartIdPattern +
			PartPattern +
			EndPattern;

		private static readonly NumberingRule PoseRule = new NumberingRule(NumberingOptions.AlphaNumeric | NumberingOptions.ZeroIndexed, 1);
		private static readonly NumberingRule PartRule = new NumberingRule(NumberingOptions.AlphaNumeric | NumberingOptions.ZeroIndexed, 1);

		#endregion

		#region Virtual Properties

		/// <summary>
		///  Gets the default regular expression used to parse the sprite.
		/// </summary>
		public virtual Regex SpriteRegex { get; } = new Regex(Pattern);
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
			if (!TryParsePose(s, m))
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

		protected bool TryParseCharacter(SpriteInfo s, Match m) {
			s.CharacterId = m.Groups["character"].Value.ToLower();
			return true;
		}
		protected bool TryParseLighting(SpriteInfo s, Match m) {
			s.Lighting = AttributeHelper.ParseCode<SpriteLighting>(m.Groups["lighting"].Value, out string unk);
			return (unk.Length == 0);
		}
		protected bool TryParsePose(SpriteInfo s, Match m) {
			if (!PoseRule.TryParse(m.Groups["pose"].Value, out int pose))
				return false;
			s.PoseInternal = pose;
			return true;
		}
		protected bool TryParseDistance(SpriteInfo s, Match m) {
			s.Distance = AttributeHelper.ParseCode<SpriteDistance>(m.Groups["distance"].Value, out string unk);
			return (unk.Length == 0);
		}
		/*protected bool TryParseSize(SpriteInfo s, Match m) {
			s.Size = AttributeHelper.ParseCode<SpriteSize>(m.Groups["size"].Value, out string unk);
			return (unk.Length == 0);
		}*/
		protected bool TryParsePart(SpriteInfo s, Match m) {
			s.PartType = m.Groups["part_id"].Value.Length;
			if (!PartRule.TryParse(m.Groups["part"].Value, out int part))
				return false;
			s.Part = part;
			return true;
		}

		#endregion
	}
}
