using System.Runtime.InteropServices;
using Grisaia.Extensions;

namespace Grisaia.Asmodean {
	partial class Anm {
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12, CharSet = CharSet.Ansi)]
		internal struct ANMHDR {
			[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 4)]
			public char[] SignatureRaw; // "ANM\0"
			public int Unknown;
			public int FrameCount;

			public string Signature => SignatureRaw.ToNullTerminatedString();
		}
		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 64, CharSet = CharSet.Ansi)]
		internal struct ANMFRM {
			[FieldOffset(0)]
			public int Start;
			[FieldOffset(8)]
			public int Index;
			[FieldOffset(16)]
			public int Duration;
			[FieldOffset(24)]
			public int NextFrame;
		}
	}
}
