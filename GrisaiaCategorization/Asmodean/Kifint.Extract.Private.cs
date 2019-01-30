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
		///  <paramref name="stream"/>, <paramref name="kifIntPath"/>, or <paramref name="exePath"/> is null.
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		///  The <paramref name="stream"/> is closed.
		/// </exception>
		private static Kifint Decrypt(KifintType type, Stream stream, string kifIntPath, string vcode2) {
			if (kifIntPath == null)
				throw new ArgumentNullException(nameof(kifIntPath));
			if (vcode2 == null)
				throw new ArgumentNullException(nameof(vcode2));
			/*string vCode2;
			try {
				vCode2 = VCode2.Find(exePath);
			} catch {
				string binPath = Path.ChangeExtension(exePath, ".bin");
				if (File.Exists(binPath)) {
					try {
						vCode2 = VCode2.Find(binPath);
					} catch {
						throw;
					}
				}
				else
					throw;
			}*/

			BinaryReader reader = new BinaryReader(stream);
			KIFHDR hdr = reader.ReadStruct<KIFHDR>();

			if (hdr.Signature != "KIF") // It's really a KIF INT file
				throw new UnexpectedFileTypeException(kifIntPath, "KIF");

			KIFENTRY[] entries = reader.ReadStructArray<KIFENTRY>(hdr.EntryCount);

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

			// Decrypt the KIFINT entries using the file key
			if (decrypt) {
				for (uint i = 0; i < hdr.EntryCount; i++) {
					if (entries[i].FileName == "__key__.dat")
						continue;

					// Give the entry the correct name
					UnobfuscateFileName(entries[i].FileNameRaw, tocSeed + i);
					// Give apply the extra offset to be decrypted
					entries[i].Offset += i;
					// Decrypt the entry's length and offset
					DecryptEntry(ref entries[i].Info, fileKey);
				}
			}

			return new Kifint(kifIntPath, entries, decrypt, fileKey, type);
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
		/*/// <summary>
		///  Locates the V_CODE2 in the executable file, which is used to decrypt the KIFINT archive.
		/// </summary>
		/// <param name="exeFile">The file path to the executable or bin file.</param>
		/// <returns>The descrypted V_CODE2 string resource.</returns>
		/// 
		/// <exception cref="GrisaiaLoadModuleException">
		///  Failed to load <paramref name="exeFile"/> as a library module.
		/// </exception>
		/// <exception cref="GrisaiaResourceException">
		///  An error occurred while trying to copy the KEY_CODE or V_CODE2 resource.
		/// </exception>
		public static string FindVCode2(string exeFile) {
			IntPtr h = LoadLibraryEx(exeFile, IntPtr.Zero, LoadLibraryExFlags.LoadLibraryAsImageResource);
			if (h == IntPtr.Zero)
				throw new GrisaiaLoadModuleException(exeFile);
			try {

				CopyResource(h, "KEY", "KEY_CODE", out byte[] key, out int keyLength);

				for (int i = 0; i < key.Length; i++)
					key[i] ^= 0xCD;

				CopyResource(h, "DATA", "V_CODE2", out byte[] vcode2, out int vcode2Length);

				//Blowfish bf = new Blowfish();
				//fixed (byte* key_buff_ptr = keyBuffer)
				//	bf.Set_Key(key_buff_ptr, keyLength);
				//bf.Decrypt(vcode2, (vcode2Length + 7) & ~7);

				DecryptVCode2(key, keyLength, vcode2, vcode2Length);

				string result = vcode2.ToNullTerminatedString(Encoding.ASCII);

				return result;
			} finally {
				FreeLibrary(h);
			}
		}
		/// <summary>
		///  Copies the resource with the specified name and type into output buffer.
		/// </summary>
		/// <param name="h">The handle to the library module of the Grisaia executable.</param>
		/// <param name="name">The name of the resource to load.</param>
		/// <param name="type">The type of the resource to load.</param>
		/// <param name="buffer">The output data for the resource.</param>
		/// <param name="length">The output length of the resource data.</param>
		private static void CopyResource(IntPtr h, string name, string type, out byte[] buffer, out int length) {
			IntPtr r = FindResource(h, name, type);
			if (r == IntPtr.Zero)
				throw new GrisaiaResourceException(name, type, "find");

			IntPtr g = LoadResource(h, r);
			if (g == IntPtr.Zero)
				throw new GrisaiaResourceException(name, type, "load");

			length = SizeofResource(h, r);
			buffer = new byte[(length + 7) & ~7];

			IntPtr lockPtr = LockResource(g);
			if (lockPtr == IntPtr.Zero)
				throw new GrisaiaResourceException(name, type, "lock");

			Marshal.Copy(lockPtr, buffer, 0, length);
		}*/

		#endregion
	}
}
