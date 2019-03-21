using System.IO;
using Microsoft.Win32;
using TriggersTools.SharpUtils.IO;

namespace Grisaia.Locators {
	/// <summary>
	///  A game locator for Frontwing-installed games.
	/// </summary>
	public static class FrontwingLocator {
		#region Constant
		
		/// <summary>
		///  The path to the Frontwing games registry key.
		/// </summary>
		private const string FrontwingRegistryPath = @"HKEY_CURRENT_USER\Software\Frontwing";
		/// <summary>
		///  The name of the registry value specifying the game installation path.
		/// </summary>
		private const string InstallPathValue = @"InstallPath";

		#endregion

		#region LocateGame

		/// <summary>
		///  Attempts to locate the game installation directory based off of the frontwing game registry key.
		/// </summary>
		/// <param name="registryKey">The registry key name for the Frontwing game.</param>
		/// <returns>The installation path for the game if found, otherwise null.</returns>
		public static string LocateGame(string registryKey) {
			if (registryKey == null)
				return null;
			if (!(Registry.GetValue($@"{FrontwingRegistryPath}\{registryKey}", InstallPathValue, null)
				is string installDir))
				return null;
			if (!PathUtils.IsValidDirectory(installDir) && !Directory.Exists(installDir))
				return null;
			return installDir;
		}

		#endregion
	}
}
