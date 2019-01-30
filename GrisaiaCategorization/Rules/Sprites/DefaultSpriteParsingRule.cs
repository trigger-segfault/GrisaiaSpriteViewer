using System;
using System.IO;
using System.Text.RegularExpressions;
using Grisaia.Categories.Sprites;

namespace Grisaia.Rules.Sprites {
	/// <summary>
	///  The default rule for parsing character sprites.
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
	public sealed class DefaultSpriteParsingRule : SpriteParsingRule {
		#region Constants
		
		private const string Pattern =
			StartPattern +
			CharacterPattern +
			LightingPattern +
			PosePattern +
			DistancePattern +
			//SizePattern +
			@"_" +
			PartTypePattern +
			PartIdPattern +
			EndPattern;
		private static readonly Regex Regex = new Regex(Pattern);

		#endregion

		#region Virtual Properties

		/// <summary>
		///  Gets the default regular expression used to parse the sprite.
		/// </summary>
		public override Regex SpriteRegex => Regex;
		/// <summary>
		///  Gets the priority order of the parsing rule when parsing sprites.
		/// </summary>
		public override double Priority => 0d;

		#endregion

		#region Virtual Parsing
		
		/// <summary>
		///  Tries to parse the sprite's Regular Expression match from <see cref="SpriteRegex"/>.<para/>
		///  This is a virtual method that can be overridden to change how sprite categories are determined.
		/// </summary>
		/// <param name="s">The sprite being parsed.</param>
		/// <param name="m">The successful Regular Expression match from <see cref="SpriteRegex"/>.</param>
		/// <returns>True if the sprite was successfully parsed.</returns>
		protected override bool TryParseMatch(SpriteInfo s, Match m) {
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
	}
}
