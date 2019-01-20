using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Asmodean;
using Newtonsoft.Json;

namespace Grisaia {
	/// <summary>
	///  A class storing static information about a single Grisaia game.
	/// </summary>
	public sealed class GameInfo {
		#region Fields

		/// <summary>
		///  Gets the unique identifier for the game.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <summary>
		///  Gets the full name of the Japanese version of the game.
		/// </summary>
		[JsonProperty("jp_name")]
		public string JPName { get; private set; }
		/// <summary>
		///  Gets the full name of the Japanese version of the game.
		/// </summary>
		[JsonProperty("jp_short")]
		public string JPShortName { get; private set; }
		/// <summary>
		///  Gets the full name of the English version of the game.
		/// </summary>
		[JsonProperty("en_name")]
		public string ENName { get; private set; }
		/// <summary>
		///  Gets the full name of the English version of the game.
		/// </summary>
		[JsonProperty("en_short")]
		public string ENShortName { get; private set; }
		/// <summary>
		///  Gets the Id of the Steam game. 0 is an invalid Id.
		/// </summary>
		[JsonProperty("steam_id")]
		public uint? SteamId { get; private set; }
		/// <summary>
		///  Gets the Id of the VNDb game. 0 is an invalid Id.
		/// </summary>
		[JsonProperty("vndb_id")]
		public uint VNDb { get; private set; }

		/// <summary>
		///  Gets the name of the Frontwing installation registry key.
		/// </summary>
		[JsonProperty("registry")]
		public string FrontwingRegistryValue { get; private set; }

		/// <summary>
		///  Gets the name of the game executable.
		/// </summary>
		[JsonProperty("exe")]
		public string Executable { get; private set; }

		/// <summary>
		///  Gets the located installation directory of the game.
		/// </summary>
		public string InstallDir { get; set; }
		/// <summary>
		///  Gets the cached image lookup for the game.
		/// </summary>
		public KifintLookup ImageLookup { get; set; }

		#endregion

		#region ToString Override

		/// <summary>
		///  Gets the string representation of the game info.
		/// </summary>
		/// <returns>The string representation of the game info.</returns>
		public override string ToString() => JPName;

		#endregion
	}
}
