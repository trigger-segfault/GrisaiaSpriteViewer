using System;
using System.Runtime.InteropServices;

namespace Grisaia.Asmodean {
	public static partial class Hgx2png {

		//[DllImport("zlib1.dll", EntryPoint = "uncompress", CallingConvention = CallingConvention.Cdecl)]
		//private extern static int Uncompress(byte[] dest, ref int destLen, byte[] source, int sourceLen);

		[DllImport("asmodean.dll", CallingConvention = CallingConvention.Cdecl)]
		private extern static void ProcessImage(
			byte[] bufferTmp,
			int length,
			int origLength,
			byte[] cmdBufferTmp,
			int cmdLength,
			int origCmdLength,
			//byte[] rgbaBuffer,
			out IntPtr pRgbaBuffer,
			out int rgbaLength,
			int width,
			int height,
			int depthBytes);
	}
}
