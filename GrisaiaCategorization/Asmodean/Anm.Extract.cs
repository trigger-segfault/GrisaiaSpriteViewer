using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Extensions;

namespace Grisaia.Asmodean {
	partial class Anm {
		/// <summary>
		///  Extracts the ANM animation from an ANM file.
		/// </summary>
		/// <param name="anmFile">The path to the ANM file to extract.</param>
		/// <returns>The extracted ANM animation.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="anmFile"/> is null.
		/// </exception>
		public static Anm Extract(string anmFile) {
			using (var stream = File.OpenRead(anmFile))
				return Extract(stream, anmFile);
		}
		/// <summary>
		///  Extracts the ANM animation from an ANM file stream.
		/// </summary>
		/// <param name="stream">The stream to extract the ANM from.</param>
		/// <param name="anmFile">The path or name of the ANM file being extracted.</param>
		/// <returns>The extracted ANM animation.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="stream"/> or <paramref name="anmFile"/> is null.
		/// </exception>
		public static Anm Extract(Stream stream, string anmFile) {
			if (anmFile == null)
				throw new ArgumentNullException(nameof(anmFile));
			BinaryReader reader = new BinaryReader(stream);

			ANMHDR hdr = reader.ReadStruct<ANMHDR>();
			if (hdr.Signature != "ANM")
				throw new UnexpectedFileTypeException(anmFile, "ANM");
			reader.ReadBytes(20); // Unused (?)

			ANMFRM[] frames = new ANMFRM[hdr.FrameCount];
			for (int i = 0; i < hdr.FrameCount; i++) {
				frames[i] = reader.ReadStruct<ANMFRM>();
				reader.ReadInt32(); // Padding (Probably)
			}

			return new Anm {
				FileName = Path.GetFileName(anmFile),
				Frames = Array.AsReadOnly(frames.Select(f => new AnmFrame(f)).ToArray()),
			};
		}
	}
}
