using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Extensions;

namespace Grisaia.Asmodean {
	partial class Kifint {
		public static KifintLookup DecryptImages(string installDir, string exeName) {
			KifintLookup lookup = new KifintLookup();
			foreach (string intFile in Directory.GetFiles(installDir, "image*.int")) {
				using (Stream stream = File.OpenRead(intFile))
					lookup.Merge(DecryptImages(stream, intFile, Path.Combine(installDir, exeName)));
			}
			return lookup;
		}

		private static Kifint DecryptImages(Stream stream, string intFile, string exeFile) {
			Stopwatch watch = Stopwatch.StartNew();
			DateTime startTime = DateTime.UtcNow;
			string binFile = Path.ChangeExtension(exeFile, ".bin");
			if (File.Exists(binFile))
				exeFile = binFile;
			string gameVCode2 = FindVCode2(exeFile);

			BinaryReader reader = new BinaryReader(stream);
			KIFHDR hdr = reader.ReadStruct<KIFHDR>();

			if (hdr.Signature != "KIF") // It's really a KIF INT file
				throw new UnexpectedFileTypeException(intFile, "INT");

			KIFENTRY[] entries = reader.ReadStructArray<KIFENTRY>(hdr.EntryCount);

			uint tocSeed = GenTocSeed(gameVCode2);
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
			
			return new Kifint(intFile, entries, decrypt, fileKey);
		}

		public static Hg3Image ExtractHgx(KifintEntry entry, string outDir) {
			var intFile = entry.Kifint;
			using (Stream stream = File.OpenRead(intFile.FilePath)) {
				BinaryReader reader = new BinaryReader(stream);
				stream.Position = entry.Offset;
				byte[] buffer = reader.ReadBytes(entry.Length);

				if (intFile.FileKey.HasValue) {
					DecryptData(buffer, entry.Length, intFile.FileKey.Value);
				}
				using (MemoryStream ms = new MemoryStream(buffer))
					return Hgx2png.Extract(ms, outDir, entry.FileName, false);
			}
		}

		/// <summary>
		///  Generates the seed used during <see cref="UnobfuscateFileName"/>.
		/// </summary>
		/// <param name="vcode2">The decrypted VCode2 extracted from the game resource.</param>
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


		private static void UnobfuscateFileName(byte[] s, uint seed) {
			const string FWD = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
			const string REV = "zyxwvutsrqponmlkjihgfedcbaZYXWVUTSRQPONMLKJIHGFEDCBA";

			//MersenneTwister.Seed(seed);
			//uint key = MersenneTwister.GenRand();
			uint key = MersenneTwister.GenRand(seed);
			int shift = (byte) ((key >> 24) + (key >> 16) + (key >> 8) + key);

			for (int i = 0; i < s.Length; i++, shift++) {
				byte c = s[i];

				if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')) {
					int index = 0;
					int index2 = shift;

					while (REV[index2 % 0x34] != c) {
						if (REV[(shift + index + 1) % 0x34] == c) {
							index += 1;
							break;
						}

						if (REV[(shift + index + 2) % 0x34] == c) {
							index += 2;
							break;
						}

						if (REV[(shift + index + 3) % 0x34] == c) {
							index += 3;
							break;
						}

						index += 4;
						index2 += 4;

						if (index > 0x34) {
							break;
						}
					}

					if (index < 0x34) {
						s[i] = (byte) FWD[index];
					}
				}

				//shift++;
			}

			return;
		}

		private static void UnobfuscateFileName(char[] s, uint seed) {
			const string FWD = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
			const string REV = "zyxwvutsrqponmlkjihgfedcbaZYXWVUTSRQPONMLKJIHGFEDCBA";

			//MersenneTwister.Seed(seed);
			//uint key = MersenneTwister.GenRand();
			uint key = MersenneTwister.GenRand(seed);
			int shift = (byte) ((key >> 24) + (key >> 16) + (key >> 8) + key);

			for (int i = 0; i < s.Length; i++) {
				char c = s[i];
				int index = 0;
				int index2 = shift;

				while (REV[index2 % 0x34] != c) {
					if (REV[(shift + index + 1) % 0x34] == c) {
						index += 1;
						break;
					}

					if (REV[(shift + index + 2) % 0x34] == c) {
						index += 2;
						break;
					}

					if (REV[(shift + index + 3) % 0x34] == c) {
						index += 3;
						break;
					}

					index += 4;
					index2 += 4;

					if (index > 0x34) {
						break;
					}
				}

				if (index < 0x34) {
					s[i] = FWD[index];
				}

				shift++;
			}

			return;
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
				throw new GrisaiaExecutableResourceException(name, type, "find");

			IntPtr g = LoadResource(h, r);
			if (g == IntPtr.Zero)
				throw new GrisaiaExecutableResourceException(name, type, "load");

			length = (int) SizeofResource(h, r);
			buffer = new byte[(length + 7) & ~7];

			IntPtr lockPtr = LockResource(g);
			if (lockPtr == IntPtr.Zero)
				throw new GrisaiaExecutableResourceException(name, type, "lock");

			Marshal.Copy(lockPtr, buffer, 0, length);
		}

		private static string FindVCode2(string exeFile) {
			IntPtr h = LoadLibraryEx(exeFile, IntPtr.Zero, LoadLibraryExFlags.LoadLibraryAsImageResource);
			if (h == IntPtr.Zero)
				throw new LoadModuleException(exeFile);

			CopyResource(h, "KEY", "KEY_CODE", out byte[] key, out int keyLength);

			for (int i = 0; i < key.Length; i++)
				key[i] ^= 0xCD;

			CopyResource(h, "DATA", "V_CODE2", out byte[] vcode2, out int vcode2Length);

			/*Blowfish bf = new Blowfish();
			fixed (byte* key_buff_ptr = keyBuffer)
				bf.Set_Key(key_buff_ptr, keyLength);
			bf.Decrypt(vcode2Buffer, (vcode2Length + 7) & ~7);
			string vcode2 = Encoding.ASCII.GetString(vcode2Buffer, 0, vcode2Length).NullTerminate();*/

			DecryptVCode2(key, keyLength, vcode2, vcode2Length);

			string result = Encoding.ASCII.GetString(vcode2).NullTerminate();

			FreeLibrary(h);

			return result;
		}
	}
}
