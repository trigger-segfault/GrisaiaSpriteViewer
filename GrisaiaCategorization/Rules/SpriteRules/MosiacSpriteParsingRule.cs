using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Grisaia.Rules {
	/// <summary>
	/// 
	/// </summary>
	/// 
	/// <remarks>
	/// Sprite Rules:
	/// " == default rule
	/// 
	/// Tyum03l_moz
	/// "\ /""" \ /
	///   |      +- CharacterId: Mosiac identifier (for our purposes, we're making a unique character entry for this)
	///   +- CharacterId (see Characters.json) + "moz"
	/// </remarks>
	public class MosiacSpriteParsingRule : SpriteParsingRule {
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
		/// Gets the default regular expression used to parse the sprite.
		/// </summary>
		public override Regex SpriteRegex { get; } = new Regex(Pattern);

		#endregion

		#region Override Parsing

		/// <summary>
		/// Tries to parse the specified sprite based on it's path.
		/// </summary>
		/// <param name="path">The file path of the sprite.</param>
		/// <param name="sprite">The output sprite info upon success, otherwise null.</param>
		/// <returns>True if the sprite was successfully parsed.</returns>
		public override bool TryParse(string path, out SpriteInfo sprite) {
			sprite = null;
			SpriteInfo s = new SpriteInfo {
				FilePath = path,
			};
			// Trim because some files contain spaces at the end. Yeah, it's dumb
			string file = Path.GetFileNameWithoutExtension(path).TrimEnd();
			Match m = SpriteRegex.Match(file);
			if (m.Success) {
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
				sprite = s;
				return true;
			}
			return false;
		}

		#endregion
	}
}
