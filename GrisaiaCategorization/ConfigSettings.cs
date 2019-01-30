using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Categories;
using Newtonsoft.Json;

namespace Grisaia {
	/// <summary>
	///  The settings class that be serialized and deserialized to json.
	/// </summary>
	public sealed class ConfigSettings {
		#region Fields

		/// <summary>
		///  The naming scheme applied to character info names.
		/// </summary>
		[JsonIgnore]
		private CharacterNamingScheme characterNamingScheme = new CharacterNamingScheme();
		/// <summary>
		///  The naming scheme applied to game info names.
		/// </summary>
		[JsonIgnore]
		private GameNamingScheme gameNamingScheme = new GameNamingScheme();
		/// <summary>
		///  Gets the overrides for game installation directories.
		/// </summary>
		[JsonProperty("installdir_overrides")]
		public Dictionary<string, string> GameInstallDirOverrides { get; private set; }
			= new Dictionary<string, string>() {
				{ "kajitsu", @"installpath\thing" },
			};

		#endregion

		#region Properties

		/// <summary>
		///  Gets or sets the naming scheme applied to character info names.
		/// </summary>
		/// 
		/// <exception cref="ArgumentNullException">
		///  value is null.
		/// </exception>
		[JsonProperty("character_naming_scheme")]
		public CharacterNamingScheme CharacterNamingScheme {
			get => characterNamingScheme;
			set => characterNamingScheme = value ?? throw new ArgumentNullException(nameof(CharacterNamingScheme));
		}
		/// <summary>
		///  Gets or sets the naming scheme applied to game info names.
		/// </summary>
		/// 
		/// <exception cref="ArgumentNullException">
		///  value is null.
		/// </exception>
		[JsonProperty("game_naming_scheme")]
		public GameNamingScheme GameNamingScheme {
			get => gameNamingScheme;
			set => gameNamingScheme = value ?? throw new ArgumentNullException(nameof(GameNamingScheme));
		}

		#endregion
	}
}
