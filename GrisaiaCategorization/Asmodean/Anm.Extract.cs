using System;
using System.IO;
using Grisaia.Extensions;

namespace Grisaia.Asmodean {
	partial class Anm {
		#region Extract

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
		/// <param name="fileName">The path or name of the ANM file being extracted.</param>
		/// <returns>The extracted ANM animation.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="stream"/> or <paramref name="fileName"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  The <paramref name="stream"/> is closed.
		/// </exception>
		public static Anm Extract(Stream stream, string fileName) {
			if (fileName == null)
				throw new ArgumentNullException(nameof(fileName));
			BinaryReader reader = new BinaryReader(stream);

			ANMHDR hdr = reader.ReadUnmanaged<ANMHDR>();
			if (hdr.Signature != "ANM")
				throw new UnexpectedFileTypeException(fileName, "ANM");
			reader.ReadBytes(20); // Unused (?)

			ANMFRM[] frames = new ANMFRM[hdr.FrameCount];
			for (int i = 0; i < hdr.FrameCount; i++) {
				frames[i] = reader.ReadUnmanaged<ANMFRM>();
				reader.ReadInt32(); // Padding (Probably)
			}

			return new Anm(Path.GetFileName(fileName), hdr, frames);
		}

		#endregion
	}
}
