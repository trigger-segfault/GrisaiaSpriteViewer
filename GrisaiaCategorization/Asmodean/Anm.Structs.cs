using System.Runtime.InteropServices;
using Grisaia.Extensions;

namespace Grisaia.Asmodean {
	partial class Anm {
		/// <summary>
		///  The header for an ANM animation file.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12, CharSet = CharSet.Ansi)]
		internal struct ANMHDR {
			/// <summary>
			///  The raw character array for the header's signature. This should be "ANM\0".
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 4)]
			public char[] SignatureRaw; // "ANM\0"
			/// <summary>
			///  Unknown 4-byte value 1.
			/// </summary>
			public int Unknown;
			/// <summary>
			///  The number of frame entries in the animation.
			/// </summary>
			public int FrameCount;

			/// <summary>
			///  Gets the header's signature from the null-terminated character array.
			/// </summary>
			public string Signature => SignatureRaw.ToNullTerminatedString();

			#region ToString Override

			/// <summary>
			///  Gets the string representation of the ANM header.
			/// </summary>
			/// <returns>The ANM header's string representation.</returns>
			public override string ToString() => $"ANMHDR \"{Signature}\"";

			#endregion
		}
		/// <summary>
		///  The frame entry in an ANM animation file.
		/// </summary>
		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 64, CharSet = CharSet.Ansi)]
		internal struct ANMFRM {
			/// <summary>
			///  Unknown. Seems to an some sort of identifier for control frames.
			/// </summary>
			[FieldOffset(0)]
			public int Start;
			/// <summary>
			///  The index of the frame image.
			/// </summary>
			[FieldOffset(8)]
			public int Index;
			/// <summary>
			///  The number of ticks this frame lasts for.
			/// </summary>
			[FieldOffset(16)]
			public int Duration;
			/// <summary>
			///  The number of ticks until the next frame.<para/>
			///  This may actually be swapped with <see cref="Duration"/>. They're always the same value.
			/// </summary>
			[FieldOffset(24)]
			public int NextFrame;

			#region ToString Override

			/// <summary>
			///  Gets the string representation of the ANM frame.
			/// </summary>
			/// <returns>The ANM frame's string representation.</returns>
			public override string ToString() => $"ANMFRM S={Start} I={Index:D2} D={Duration:D2} N={NextFrame:D2}";

			#endregion
		}
	}
}
