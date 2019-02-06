using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Grisaia.Extensions;

namespace Grisaia.Asmodean {
	partial class Kifint {
		#region Decrypt Private

		/// <summary>
		///  Decrypts the KIFINT archives using the known archive type, install directory, and name of executable with
		///  the V_CODE2 used to decrypt.
		/// </summary>
		/// <param name="type">The type of archive to look for and decrypt.</param>
		/// <param name="stream">The stream to the open KIFINT archive.</param>
		/// <param name="installDir">The installation directory for both the archives and executable.</param>
		/// <param name="exePath">The path to the executable to extract the V_CODE2 key from.</param>
		/// <returns>The <see cref="KifintLookup"/> merged with all loaded archives.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="stream"/>, <paramref name="kifintPath"/>, or <paramref name="exePath"/> is null.
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		///  The <paramref name="stream"/> is closed.
		/// </exception>
		private static Kifint Decrypt(KifintType type, Stream stream, string kifintPath, string vcode2,
			KifintProgressArgs progress, KifintProgressCallback callback)
		{
			if (kifintPath == null)
				throw new ArgumentNullException(nameof(kifintPath));
			if (vcode2 == null)
				throw new ArgumentNullException(nameof(vcode2));

			BinaryReader reader = new BinaryReader(stream);
			KIFHDR hdr = reader.ReadUnmanaged<KIFHDR>();

			if (hdr.Signature != "KIF") // It's really a KIF INT file
				throw new UnexpectedFileTypeException(kifintPath, "KIF");

			KIFENTRY[] entries = reader.ReadUnmanagedArray<KIFENTRY>(hdr.EntryCount);

			progress.EntryIndex = 0;
			progress.EntryCount = entries.Length;

			uint tocSeed = GenTocSeed(vcode2);
			uint fileKey = 0;
			bool decrypt = false;

			// Obtain the decryption file key if one exists
			for (int i = 0; i < hdr.EntryCount; i++) {
				if (entries[i].FileName == "__key__.dat") {
					fileKey = MersenneTwister.GenRand(entries[i].Length);
					decrypt = true;
					break;
				}
			}

			const int ProgressThreshold = 500;

			// Decrypt the KIFINT entries using the file key
			if (decrypt) {
				for (uint i = 0; i < hdr.EntryCount; i++) {
					if (entries[i].FileName == "__key__.dat")
						continue;

					progress.EntryIndex++;
					if (i % ProgressThreshold == 0)
						callback?.Invoke(progress);

					// Give the entry the correct name
					UnobfuscateFileName(entries[i].FileNameRaw, tocSeed + i);
					// Give apply the extra offset to be decrypted
					entries[i].Offset += i;
					// Decrypt the entry's length and offset
					DecryptEntry(ref entries[i].Info, fileKey);
				}
			}

			return new Kifint(kifintPath, entries, decrypt, fileKey, type);
		}

		#endregion

		#region UnobfuscateFileName

		/// <summary>
		///  Unobfuscates the <see cref="KIFENTRY.FileNameRaw"/> field using the specified seed.
		/// </summary>
		/// <param name="fileName">The raw file name in bytes.</param>
		/// <param name="seed">The seed used to generate the unobfuscation key.</param>
		private static void UnobfuscateFileName(byte[] fileName, uint seed) {
			const int Length = 52;
			const string FWD = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
			const string REV = "zyxwvutsrqponmlkjihgfedcbaZYXWVUTSRQPONMLKJIHGFEDCBA";

			uint key = MersenneTwister.GenRand(seed);
			int shift = (byte) ((key >> 24) + (key >> 16) + (key >> 8) + key);

			for (int i = 0; i < fileName.Length; i++, shift++) {
				byte c = fileName[i];

				if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')) {
					int index = 0;
					int index2 = shift;

					while (REV[index2 % Length] != c) {
						if (REV[(shift + index + 1) % Length] == c) {
							index += 1;
							break;
						}

						if (REV[(shift + index + 2) % Length] == c) {
							index += 2;
							break;
						}

						if (REV[(shift + index + 3) % Length] == c) {
							index += 3;
							break;
						}

						index += 4;
						index2 += 4;

						if (index >= Length) // We're outside the array, no need to continue
							break;
					}

					if (index < Length) // Only assign if we're inside the array
						fileName[i] = (byte) FWD[index];
				}
			}
		}
		/// <summary>
		///  Generates the seed used during <see cref="UnobfuscateFileName"/>.
		/// </summary>
		/// <param name="vcode2">The decrypted V_CODE2 extracted from the game resource.</param>
		/// <returns>The generated seed.</returns>
		private static uint GenTocSeed(string vcode2) {
			const uint magic = 0x4C11DB7;
			uint seed = uint.MaxValue;

			for (int i = 0; i < vcode2.Length; i++) {
				seed ^= ((uint) vcode2[i]) << 24;

				for (int j = 0; j < 8; j++) {
					if ((seed & 0x80000000) != 0) {
						seed *= 2;
						seed ^= magic;
					}
					else {
						seed *= 2;
					}
				}

				seed = ~seed;
			}

			return seed;
		}

		#endregion
	}
}
