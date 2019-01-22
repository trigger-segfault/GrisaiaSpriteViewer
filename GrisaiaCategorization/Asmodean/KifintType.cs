using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grisaia.Asmodean {
	/// <summary>
	///  An enum for determining how a KIFINT archive's files are used.
	/// </summary>
	public enum KifintKnownType {
		/// <summary>The file type is known, and can be used without special conversion.</summary>
		Known = 0,
		/// <summary>The file type is "known", but requires the use of an existing converter.</summary>
		Convert = 1,
		/// <summary>The file type is unknown and cannot be used without further investigation.</summary>
		Unknown = 2,
	}
	/// <summary>
	///  The types of known KIFINT archive files.
	/// </summary>
	public enum KifintType {
		/// <summary>No file type. This is usually invalid.</summary>
		None = 0,

		/// <summary>.ogg background music</summary>
		[KifintWildcard("bgm*.int")]
		[KifintFileTypes(KifintKnownType.Known, ".ogg")]
		Bgm,

		/// <summary>.cvs unknown | .txt unknown | .xml config | .dat unknown</summary>
		[KifintWildcard("config.int")]
		[KifintFileTypes(KifintKnownType.Known, ".cvs", ".txt", ".xml")]
		[KifintFileTypes(KifintKnownType.Unknown, ".dat")]
		Config,

		/// <summary>.zt unknown</summary>
		[KifintWildcard("export.int")]
		[KifintFileTypes(KifintKnownType.Unknown, ".zt")]
		Export,

		/// <summary>.fes unknown</summary>
		[KifintWildcard("fes.int")]
		[KifintFileTypes(KifintKnownType.Unknown, ".fes")]
		Fes,

		/// <summary>.RRD unknown (likely fonts)</summary>
		[KifintWildcard("font.int")]
		[KifintFileTypes(KifintKnownType.Unknown, ".RRD")]
		Font,

		/// <summary>.hg3 images | .anm animations</summary>
		[KifintWildcard("image*.int")]
		[KifintFileTypes(KifintKnownType.Convert, ".anm", ".hg3")]
		Image,

		/// <summary>.kcs unknown</summary>
		[KifintWildcard("kcs.int")]
		[KifintFileTypes(KifintKnownType.Unknown, ".kcs")]
		Kcs,

		/// <summary>.kx2 unknown</summary>
		[KifintWildcard("kx2.int")]
		[KifintFileTypes(KifintKnownType.Unknown, ".kx2")]
		Kx2,

		/// <summary>.kcs unknown</summary>
		[KifintWildcard("mot.int")]
		[KifintFileTypes(KifintKnownType.Unknown, ".kcs")]
		Mot,

		/// <summary>.mpg moves</summary>
		[KifintWildcard("movie*.int")]
		[KifintFileTypes(KifintKnownType.Known, ".mpg")]
		Movie,

		/// <summary>.ogg character voices</summary>
		[KifintWildcard("pcm_*.int")]
		[KifintFileTypes(KifintKnownType.Known, ".ogg")]
		Pcm,

		/// <summary>.kcs unknown</summary>
		[KifintWildcard("ptcl.int")]
		[KifintFileTypes(KifintKnownType.Unknown, ".kcs")]
		Ptcl,

		/// <summary>.cst scenes</summary>
		[KifintWildcard("scene*.int")]
		[KifintFileTypes(KifintKnownType.Unknown, ".cst")]
		Scene,

		/// <summary>.ogg sound effects</summary>
		[KifintWildcard("se*.int")]
		[KifintFileTypes(KifintKnownType.Known, ".ogg")]
		Se,

		/// <summary>
		///  Likely archive used to overwrite other archive entries. Will test later.
		/// </summary>
		[KifintWildcard("update*.int")]
		Update,
	}
}
