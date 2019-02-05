using System;
using System.Runtime.InteropServices;
using System.Text;
using Grisaia.Extensions;

namespace Grisaia.Asmodean {
	/// <summary>
	///  A static class for extracting the Grisaia executable or bin file's V_CODE2.
	/// </summary>
	public static partial class VCode2 {
		#region FindVCode2

		/// <summary>
		///  Locates the V_CODE2 in the module file, which is used to decrypt the KIFINT archive's entry file names.
		/// </summary>
		/// <param name="exeFile">The file path to the executable or bin file.</param>
		/// <returns>The decrypted V_CODE2 string resource.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="exeFile"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <paramref name="exeFile"/> is an empty string or whitespace.
		/// </exception>
		/// <exception cref="GrisaiaLoadModuleException">
		///  Failed to load <paramref name="exeFile"/> as a library module.
		/// </exception>
		/// <exception cref="GrisaiaResourceException">
		///  An error occurred while trying to copy the KEY_CODE or V_CODE2 resource.
		/// </exception>
		public static string Find(string exeFile) {
			if (exeFile == null)
				throw new ArgumentNullException(nameof(exeFile));
			if (string.IsNullOrWhiteSpace(exeFile))
				throw new ArgumentException($"{nameof(exeFile)} is empty or whitespace!", nameof(exeFile));
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
				
				return vcode2.ToNullTerminatedString(Encoding.ASCII);
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
		/// 
		/// <exception cref="GrisaiaResourceException">
		///  An error occurred while trying to copy the resource.
		/// </exception>
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
		}

		#endregion
	}
}
