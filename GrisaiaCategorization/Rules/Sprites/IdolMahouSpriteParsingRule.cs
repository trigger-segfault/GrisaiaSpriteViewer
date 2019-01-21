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
	///  TmicflyL_04
	///  "\ /\ /| ""
	///    |  | +- Size: code (SpriteDistance)
	///    |  +- Flying identifier (always fly/kv)
	///    |  +- No Lighing, Pose, or Distance
	///    +- CharacterId (always mic/kaz (or kap for kz), Michiru/Kazuki)
	/// </remarks>
	public sealed class IdolMahouSpriteParsingRule : SpriteParsingRule {
		#region Constants
		
		private const string Pattern =
			StartPattern +
			@"(?'character'[A-Za-z]+(?:fly|kv))" +
			DistancePattern +
			//SizePattern +
			@"_" +
			PartIdPattern +
			PartPattern +
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

			//if (!TryParseLighting(s, m))
			//	return false;
			//if (!TryParsePose(s, m))
			//	return false;
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
