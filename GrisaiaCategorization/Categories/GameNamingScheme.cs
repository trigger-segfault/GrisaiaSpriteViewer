using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Grisaia.Categories {
	/// <summary>
	///  A list of settings defining how game infos should display their name.
	/// </summary>
	public sealed class GameNamingScheme {
		#region Fields

		/// <summary>
		///  Gets or sets if all naming rules should be ignored and the Id should be used instead.
		/// </summary>
		[JsonProperty("use_id")]
		public bool UseId { get; set; } = false;
		/// <summary>
		///  Gets or sets if the English name of the game should be used.
		/// </summary>
		[JsonProperty("use_en_name")]
		public bool UseEnglishName { get; set; } = false;
		/// <summary>
		///  Gets or sets if the short name of the game should be used.
		/// </summary>
		[JsonProperty("use_short_name")]
		public bool UseShortName { get; set; } = true;

		#endregion

		#region GetName

		/// <summary>
		///  Formats the name of the specified game info using the naming scheme settings.
		/// </summary>
		/// <param name="game">The game with the name to format.</param>
		/// <returns>The game's name with the naming scheme applied.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="game"/> is null.
		/// </exception>
		public string GetName(GameInfo game) {
			if (game == null)
				throw new ArgumentNullException(nameof(game));
			if (UseId)
				return game.Id;
			if (UseShortName) {
				if (UseEnglishName)
					return game.ENShortName;
				else
					return game.JPShortName;
			}
			else {
				if (UseEnglishName)
					return game.ENName;
				else
					return game.JPName;
			}
		}

		#endregion
	}
}
