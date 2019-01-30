using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Grisaia.Asmodean {
	partial class VCode2 {
		/// <summary>
		///  The action to be taken when loading the module using see <see cref="LoadLibraryEx"/>.
		/// </summary>
		private enum LoadLibraryExFlags {
			None = 0,
			LoadLibraryAsImageResource = 0x00000020,
		}

		/// <summary>
		///  Loads the specified module into the address space of the calling process. The specified module may cause
		///  other modules to be loaded.
		/// </summary>
		/// <param name="lpLibFileName">A string that specifies the file name of the module to load.</param>
		/// <param name="hFile">This parameter is reserved for future use. It must be null.</param>
		/// <param name="dwFlags">The action to be taken when loading the module.</param>
		/// <returns></returns>
		[DllImport("kernel32.dll")]
		private extern static IntPtr LoadLibraryEx(
			string lpLibFileName,
			IntPtr hFile,
			[MarshalAs(UnmanagedType.U4)] LoadLibraryExFlags dwFlags);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.I1)]
		private extern static bool FreeLibrary(
			IntPtr hLibModule);

		[DllImport("kernel32.dll")]
		private extern static IntPtr FindResource(
			IntPtr hModule,
			string lpName,
			string lpType);

		[DllImport("kernel32.dll", SetLastError = true)]
		private extern static IntPtr LoadResource(
			IntPtr hModule,
			IntPtr hResInfo);

		[DllImport("kernel32.dll", SetLastError = true)]
		private extern static int SizeofResource(
			IntPtr hModule,
			IntPtr hResInfo);

		[DllImport("kernel32.dll")]
		private extern static IntPtr LockResource(
			IntPtr hGlobal);


		[DllImport("asmodean.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		private extern static void DecryptVCode2(
			byte[] keyBuffer,
			int keyLength,
			byte[] vcode2Buffer,
			int vcode2Length);
	}
}
