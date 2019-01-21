using System.Text.RegularExpressions;
using Grisaia.Categories.Sprites;

namespace Grisaia.Rules.Sprites {
	/// <summary>
	/// 
	/// </summary>
	/// 
	/// <remarks>
	///  Sprite Rules:
	///  " == default rule
	///  
	///  Tyum03l_moz
	///  "\ /""" \ /
	///    |      +- CharacterId: Mosiac identifier (for our purposes, we're making a unique character entry for this)
	///    +- CharacterId (see Characters.json) + "moz"
	/// </remarks>
	public sealed class MosiacSpriteParsingRule : SpriteParsingRule {
		#region Constants
		
		private const string Pattern =
			StartPattern +
			CharacterPattern +
			LightingPattern +
			PosePattern +
			DistancePattern +
			//SizePattern +
			@"_" +
			@"moz" +
			EndPattern;

		#endregion

		#region Override Properties

		/// <summary>
		///  Gets the default regular expression used to parse the sprite.
		/// </summary>
		public override Regex SpriteRegex { get; } = new Regex(Pattern);

		#endregion

		#region Override Parsing

		/// <summary>
		///  Tries to parse the sprite's Regular Expression match from <see cref="SpriteRegex"/>.
		/// </summary>
		/// <param name="s">The sprite being parsed.</param>
		/// <param name="m">The successful Regular Expression match from <see cref="SpriteRegex"/>.</param>
		/// <returns>True if the sprite was successfully parsed.</returns>
		protected override bool TryParseMatch(SpriteInfo s, Match m) {
			if (!TryParseCharacter(s, m))
				return false;
			s.CharacterId += "moz";

			if (!TryParseLighting(s, m))
				return false;
			if (!TryParsePose(s, m))
				return false;
			if (!TryParseDistance(s, m))
				return false;
			//if (!TryParseSize(s, m))
			//	return false;

			//if (!TryParsePart(s, m))
			//	return false;
			return true;
		}

		#endregion
	}
}
