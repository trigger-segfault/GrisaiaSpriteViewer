using System.Runtime.InteropServices;

namespace Grisaia.Asmodean {
	partial class Kifint {
		[DllImport("asmodean.dll", CallingConvention = CallingConvention.Cdecl)]
		private extern static void DecryptEntry(
			ref KIFENTRYINFO entry,
			uint fileKey);

		[DllImport("asmodean.dll", CallingConvention = CallingConvention.Cdecl)]
		private extern static void DecryptData(
			byte[] buffer,
			int length,
			uint fileKey);
	}
}
