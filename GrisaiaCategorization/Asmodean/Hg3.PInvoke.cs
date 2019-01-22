using System;
using System.Runtime.InteropServices;

namespace Grisaia.Asmodean {
	partial class Hg3 {
		//[DllImport("zlib1.dll", EntryPoint = "uncompress", CallingConvention = CallingConvention.Cdecl)]
		//private extern static int Uncompress(byte[] dest, ref int destLen, byte[] source, int sourceLen);

		[DllImport("asmodean.dll", EntryPoint = "ProcessImage", CallingConvention = CallingConvention.Cdecl)]
		private extern static void ProcessImageNative(
			byte[] bufferTmp,
			int length,
			int origLength,
			byte[] cmdBufferTmp,
			int cmdLength,
			int origCmdLength,
			out IntPtr pRgbaBuffer,
			out int rgbaLength,
			int width,
			int height,
			int depthBytes);
	}
}
