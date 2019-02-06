using System;
using System.IO;
using Newtonsoft.Json;

namespace Grisaia.Categories {
	/// <summary>
	///  Installation information for a <see cref="GameInfo"/> entry.<para/>
	///  This can be a "located" installation, or a "custom" specified installation.
	/// </summary>
	public struct GameInstallInfo : IEquatable<GameInstallInfo> {
		#region Constants

		/// <summary>
		///  Gets an unused game install info structure.
		/// </summary>
		public static GameInstallInfo None => new GameInstallInfo();

		#endregion

		#region Fields

		/// <summary>
		///  Gets the installation directory of the game.
		/// </summary>
		[JsonProperty("dir")]
		public string Directory { get; set; }
		/// <summary>
		///  Gets the name of the game executable.
		/// </summary>
		[JsonProperty("exe")]
		public string Executable { get; set; }
		/// <summary>
		///  Gets the V_CODE2 resource used for KIFINT archive extraction.
		/// </summary>
		[JsonIgnore]
		public string VCode2 { get; internal set; }

		#endregion

		#region Properties

		/// <summary>
		///  Gets if this install information is unused. Only applies for "custom" installation info.
		/// </summary>
		[JsonIgnore]
		public bool IsUnused => Directory == null;
		/// <summary>
		///  Gets if the installation info has been validated and can be used.
		/// </summary>
		[JsonIgnore]
		public bool IsValidated => VCode2 != null;
		/// <summary>
		///  Gets the name of the executable as a .bin file.<para/>
		///  Returns null if <see cref="Executable"/> is null.
		/// </summary>
		[JsonIgnore]
		public string ExecutableBin => (Executable != null ? Path.ChangeExtension(Executable, ".bin") : null);
		/// <summary>
		///  Gets the full path to the executable.<para/>
		///  Returns null if <see cref="Executable"/> is null.
		/// </summary>
		/// 
		/// <exception cref="InvalidOperationException">
		///  <see cref="Executable"/> is null.
		/// </exception>
		[JsonIgnore]
		public string ExecutablePath {
			get {
				if (Directory == null)
					throw new InvalidOperationException($"Cannot call {nameof(ExecutablePath)} when " +
														$"{nameof(Directory)} is null.");
				
				return (Executable != null ? Path.Combine(Directory, Executable) : null);
			}
		}
		/// <summary>
		///  Gets the full path to the executable as a .bin file.<para/>
		///  Returns null if <see cref="Executable"/> is null.
		/// </summary>
		/// 
		/// <exception cref="InvalidOperationException">
		///  <see cref="Directory"/> is null.
		/// </exception>
		[JsonIgnore]
		public string ExecutableBinPath {
			get {
				if (Directory == null)
					throw new InvalidOperationException($"Cannot call {nameof(ExecutableBinPath)} when " +
														$"{nameof(Directory)} is null.");

				return (Executable != null ? Path.Combine(Directory, ExecutableBin) : null);
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs the game intallation info with the specified directory and optional executable name.
		/// </summary>
		/// <param name="installDir">The required install directory.</param>
		/// <param name="executable">The optional new name of the executable.</param>
		public GameInstallInfo(string installDir, string executable = null) {
			Directory  = installDir;
			Executable = executable;
			VCode2     = null;
		}

		internal GameInstallInfo(GameInstallInfo primary, GameInstallInfo backup) {
			Directory  = primary.Directory  ?? backup.Directory;
			Executable = primary.Executable ?? backup.Executable;
			VCode2     = primary.VCode2     ?? backup.VCode2;
		}

		#endregion

		#region Invalidate

		/// <summary>
		///  Invalidates the install location by setting the <see cref="VCode2"/> to null.
		/// </summary>
		public void Invalidate() => VCode2 = null;

		#endregion

		#region Object Overrides

		/// <summary>
		///  Gets the string representation of the game installation information.
		/// </summary>
		/// <returns>The string representation of the game install info.</returns>
		public override string ToString() => $"Directory={Directory}, Executable={Executable}";
		/// <summary>
		///  Gets the hash code for the game installation information's directory and executable.
		/// </summary>
		/// <returns>The hash code for the game installation information.</returns>
		public override int GetHashCode() => Directory.GetHashCode() ^ (Executable?.GetHashCode() ?? 0);
		/// <summary>
		///  Gets if the specified object is a game install info and equal to this install info.
		/// </summary>
		/// <param name="obj">The object to check for equality with.</param>
		/// <returns>True if the object is a game install info and equal to this game intall info.</returns>
		public override bool Equals(object obj) {
			if (obj is GameInstallInfo install)
				return Equals(install);
			return false;
		}
		/// <summary>
		///  Gets if the specified game install info is equal to this install info.
		/// </summary>
		/// <param name="other">The game install info to check for equality with.</param>
		/// <returns>True if the other game install info is equal to this game intall info.</returns>
		public bool Equals(GameInstallInfo other) {
			return Directory == other.Directory && Executable == other.Directory;
		}

		#endregion

		#region Binary Logic Operators

		public static bool operator ==(GameInstallInfo a, GameInstallInfo b) {
			return a.Directory == b.Directory && a.Executable == b.Executable;
		}
		public static bool operator !=(GameInstallInfo a, GameInstallInfo b) {
			return a.Directory != b.Directory || a.Executable != b.Executable;
		}

		#endregion
	}
}
