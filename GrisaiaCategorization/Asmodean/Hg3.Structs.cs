using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Grisaia.Extensions;

namespace Grisaia.Asmodean {
	partial class Hg3 {
		/// <summary>
		///  The header for an HG-3 file. Which is used to identify the number of entries as well as the signature.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20, CharSet = CharSet.Ansi)]
		internal struct HG3HDR {
			/// <summary>
			///  The raw character array for the header's signature. This should be "HG-3".
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 4)]
			public char[] SignatureRaw; // "HG-3"
			/// <summary>
			///  Unknown 4-byte value 1.
			/// </summary>
			public int Unknown1;
			/// <summary>
			///  Unknown 4-byte value 2.
			/// </summary>
			public int Unknown2;
			/// <summary>
			///  Unknown 4-byte value 3.
			/// </summary>
			public int Unknown3;
			/// <summary>
			///  The number of entries in the HG3 structure. This field doesn't seem to be very reliable.
			/// </summary>
			public int EntryCount;

			/// <summary>
			///  Gets the header's signature from the null-terminated character array.
			/// </summary>
			public string Signature => SignatureRaw.ToNullTerminatedString();

			#region ToString Override

			/// <summary>
			///  Gets the string representation of the HG-3 header.
			/// </summary>
			/// <returns>The string representation of the HG-3 header.</returns>
			public override string ToString() => $"HG3HDR \"{Signature}\"";

			#endregion
		}
		/// <summary>
		///  A tag for a specific entry in an HG-3 file.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16, CharSet = CharSet.Ansi)]
		internal struct HG3TAG {
			/// <summary>
			///  The raw character array for the tag's signature.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
			public char[] SignatureRaw;
			/// <summary>
			///  Gets the offset of the next tag in the HG-3 stream.
			/// </summary>
			public int OffsetNext;
			/// <summary>
			///  Gets the length of the tag's structure preceding this tag in the stream.
			/// </summary>
			public int Length;

			/// <summary>
			///  Gets the tag's signature from the null-terminated character array.
			/// </summary>
			public string Signature => SignatureRaw.ToNullTerminatedString();

			#region ToString Override

			/// <summary>
			///  Gets the string representation of the HG-3 entry tag.
			/// </summary>
			/// <returns>The string representation of the HG-3 entry tag.</returns>
			public override string ToString() => $"HG3TAG \"{Signature}\"";

			#endregion
		}
		/// <summary>
		///  Standard HG-3 image info, this tag uses the tag name
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 40)]
		internal struct HG3STDINFO {
			/// <summary>
			///  The condensed width of the image.
			/// </summary>
			public int Width;
			/// <summary>
			///  The condensed height of the image.
			/// </summary>
			public int Height;
			/// <summary>
			///  The depth of the image format in bits.
			/// </summary>
			public int DepthBits;
			/// <summary>
			///  The horizontal offset of the image from the left.
			/// </summary>
			public int OffsetX;
			/// <summary>
			///  The vertical offset of the image from the top.
			/// </summary>
			public int OffsetY;
			/// <summary>
			///  The total width of the image with <see cref="OffsetX"/> applied.
			/// </summary>
			public int TotalWidth;
			/// <summary>
			/// The height width of the image with <see cref="OffsetY"/> applied.
			/// </summary>
			public int TotalHeight;
			/// <summary>
			///  The number of frames in the animation.
			/// </summary>
			public int FrameCount;
			/// <summary>
			///  The horizontal center of the image. Used for drawing in the game.
			/// </summary>
			public int Center;
			/// <summary>
			///  The vertical baseline of the image. Used for drawing in the game.
			/// </summary>
			public int Baseline;

			#region HasTagSignature

			/// <summary>
			///  Gets if the tag's signature matches that if this structure.
			/// </summary>
			/// <param name="tagSignature">The <see cref="HG3TAG.Signature"/>.</param>
			/// <returns>True if the signatures match.</returns>
			public static bool HasTagSignature(string tagSignature) {
				return tagSignature.StartsWith("stdinfo");
			}

			#endregion

			#region ToString Override

			/// <summary>
			///  Gets the string representation of the HG-3 standard image info.
			/// </summary>
			/// <returns>The string representation of the HG-3 standard image info.</returns>
			public override string ToString() => $"HG3STDINFO {Width}x{Height} Count={FrameCount}";

			#endregion
		}
		/// <summary>
		///  The tag structure for a standard HG-3 image. Is identified as "img####".
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
		internal struct HG3IMG {
			#region Constants

			/// <summary>
			///  The regex used to match tag signatures of this type.
			/// </summary>
			private static readonly Regex SignatureRegex = new Regex(@"img\d+");

			#endregion

			public int Unknown;
			public int Height;
			public int DataLength;
			public int OriginalDataLength;
			public int CmdLength;
			public int OriginalCmdLength;

			#region HasTagSignature

			/// <summary>
			///  Gets if the tag's signature matches that if this structure.
			/// </summary>
			/// <param name="tagSignature">The <see cref="HG3TAG.Signature"/>.</param>
			/// <returns>True if the signatures match.</returns>
			public static bool HasTagSignature(string tagSignature) {
				return SignatureRegex.IsMatch(tagSignature);
			}

			#endregion

			#region ToString Override

			/// <summary>
			///  Gets the string representation of the standard HG-3 image.
			/// </summary>
			/// <returns>The string representation of the standard HG-3 image.</returns>
			public override string ToString() => $"HG3IMG Data={DataLength} Cmd={CmdLength}";

			#endregion
		}
		/// <summary>
		///  The tag structure for a standard HG-3 AL image. Is identified as "img_al".
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
		internal struct HG3IMG_AL {
			public int Length;
			public int OriginalLength;

			#region HasTagSignature

			/// <summary>
			///  Gets if the tag's signature matches that if this structure.
			/// </summary>
			/// <param name="tagSignature">The <see cref="HG3TAG.Signature"/>.</param>
			/// <returns>True if the signatures match.</returns>
			public static bool HasTagSignature(string tagSignature) {
				return tagSignature == "img_al";
			}

			#endregion

			#region ToString Override

			/// <summary>
			///  Gets the string representation of the HG-3 AL image.
			/// </summary>
			/// <returns>The string representation of the HG-3 AL image.</returns>
			public override string ToString() => $"HG3IMG_AL Length={Length}";

			#endregion
		}
		/// <summary>
		///  The tag structure for a standard HG-3 JPEG image. Is identified as "img_jpg".
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0)]
		internal struct HG3IMG_JPG {

			#region HasTagSignature

			/// <summary>
			///  Gets if the tag's signature matches that if this structure.
			/// </summary>
			/// <param name="tagSignature">The <see cref="HG3TAG.Signature"/>.</param>
			/// <returns>True if the signatures match.</returns>
			public static bool HasTagSignature(string tagSignature) {
				return tagSignature == "img_jpg";
			}

			#endregion

			#region ToString Override

			/// <summary>
			///  Gets the string representation of the HG-3 JPEG image.
			/// </summary>
			/// <returns>The string representation of the HG-3 JPEG image.</returns>
			public override string ToString() => $"HG3IMG_JPG Unknown";

			#endregion
		}
		/// <summary>
		///  The tag structure for a standard HG-3 image mode. Is identified as "imgmode".
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0)]
		internal struct HG3IMGMODE {

			#region HasTagSignature

			/// <summary>
			///  Gets if the tag's signature matches that if this structure.
			/// </summary>
			/// <param name="tagSignature">The <see cref="HG3TAG.Signature"/>.</param>
			/// <returns>True if the signatures match.</returns>
			public static bool HasTagSignature(string tagSignature) {
				return tagSignature == "imgmode";
			}

			#endregion

			#region ToString Override

			/// <summary>
			///  Gets the string representation of the HG-3 image mode.
			/// </summary>
			/// <returns>The string representation of the HG-3 image mode.</returns>
			public override string ToString() => $"HG3IMGMODE Unknown";

			#endregion
		}
	}
}
