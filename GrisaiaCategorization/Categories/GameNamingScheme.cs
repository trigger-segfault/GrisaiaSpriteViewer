using System;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace Grisaia.Categories {
	/// <summary>
	///  A list of settings defining how game infos should display their name.
	/// </summary>
	public sealed class GameNamingScheme : ObservableObject {
		#region Fields
		
		/// <summary>
		///  True if all naming rules should be ignored and the Id should be used instead.
		/// </summary>'
		[JsonIgnore]
		private bool onlyId = false;
		/// <summary>
		///  True if the English name of the game should be used.
		/// </summary>
		[JsonIgnore]
		private bool englishName = false;
		/// <summary>
		///  True if the short name of the game should be used.
		/// </summary>
		[JsonIgnore]
		private bool shortName = true;

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
		///  Gets or sets if the English name of the game should be used.
		/// </summary>
		[JsonProperty("en_name")]
		public bool EnglishName {
			get => englishName;
			set => Set(ref englishName, value);
		}
		/// <summary>
		///  Gets or sets if the short name of the game should be used.
		/// </summary>
		[JsonProperty("short_name")]
		public bool ShortName {
			get => shortName;
			set => Set(ref shortName, value);
		}

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
			if (IdOnly)
				return game.Id;
			if (ShortName) {
				if (EnglishName)
					return game.ENShortName;
				else
					return game.JPShortName;
			}
			else {
				if (EnglishName)
					return game.ENName;
				else
					return game.JPName;
			}
		}

		#endregion

		#region Clone

		/// <summary>
		///  Creates a deep clone of this game naming scheme.
		/// </summary>
		/// <returns>The clone of this game naming scheme.</returns>
		public GameNamingScheme Clone() {
			return new GameNamingScheme {
				IdOnly = IdOnly,
				EnglishName = EnglishName,
				ShortName = ShortName,
			};
		}

		#endregion
	}
}
