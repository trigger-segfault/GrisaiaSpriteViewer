using System.Text.RegularExpressions;

namespace Grisaia.Rules.Sprites {
	/// <summary>
	/// 
	/// </summary>
	/// 
	/// <remarks>
	///  Sprite Rules:
	///  " == default rule
	///  
	///  This sprite seems to stop wasting two sprites that could easily be one,
	///  the eyes and mouth, are now combined face with two equal Ids
	///  
	///  Trob01f_0044
	///  |\ /||| ||++- Part: 1-indexed (1-9,a-z,-,+,=), Both are same number
	///  | | ||| ++- PartType: number of padded 0's, No padded 0's is valid
	///  | | ||+- Distance: code (optional, SpriteDistance)
	///  | | |+- Value: 1-indexed (1-9,a-z,-,+,=) (occasionally 0-indexed)
	///  | | |+- Pose:  (Value > 3 ? ((Value - 1) % 3) + 1 : value) (+1 because of occasional 0-indexed)
	///  | | |+- Blush: (Value > 3 ? ((Value - 1) / 3) : value)
	///  | | +- Lighting: code (SpriteLighting)
	///  | +- CharacterId (see Characters.json)
	///  +- Sprite Identifier (always T)
	/// </remarks>
	public sealed class DoublePartSpriteParsingRule : SpriteParsingRule {
		#region Constants
		
		private const string Pattern =
			StartPattern +
			CharacterPattern +
			LightingPattern +
			PosePattern +
			DistancePattern +
			//SizePattern +
			@"_" +
			PartIdPattern +
			PartPattern + @"{2}" + // Repeat this twice
			EndPattern;

		#endregion

		#region Override Properties

		/// <summary>
		///  Gets the default regular expression used to parse the sprite.
		/// </summary>
		public override Regex SpriteRegex { get; } = new Regex(Pattern);
		/// <summary>
		///  Gets the sprite should be ignored if matched.
		/// </summary>
		public override bool IgnoreSprite => true;

		#endregion
	}
}
