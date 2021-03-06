﻿using System.Runtime.InteropServices;
using System.Text;
using Grisaia.Extensions;

namespace Grisaia.Asmodean {
	partial class Kifint {
		/// <summary>
		///  The header structure for a KIFINT archive.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8, CharSet = CharSet.Ansi)]
		internal struct KIFHDR {
			#region Fields

			/// <summary>
			///  The raw character array signature of the file.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 4)]
			public char[] SignatureRaw; // "KIF\0"
			/// <summary>
			///  The number of <see cref="KIFENTRY"/>s in the KIFINT archive.
			/// </summary>
			public int EntryCount;

			#endregion

			#region Properties

			/// <summary>
			///  Gets the signature of the file.
			/// </summary>
			public string Signature => SignatureRaw.ToNullTerminatedString();

			#endregion
		}
		/// <summary>
		///  The entry structure for a KIFINT archive.
		/// </summary>
		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 72, CharSet = CharSet.Ansi)]
		internal struct KIFENTRY {
			#region Constants

			/// <summary>
			///  We use this to preserve the developer naming fuckups such as the full-width 'ｇ' in Meikyuu's
			///  "bｇ62t.hg3".
			/// </summary>
			private static readonly Encoding JapaneseEncoding = Encoding.GetEncoding(932);

			#endregion

			#region Fields
			
			/// <summary>
			///  The raw character array filename of the entry.
			/// </summary>
			[FieldOffset(0)]
			[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
			public byte[] FileNameRaw;
			/// <summary>
			///  We don't need to pass the <see cref="FileNameRaw"/> during P/Invoke, so we have this info structure.
			/// </summary>
			[FieldOffset(64)]
			public KIFENTRYINFO Info;
			/// <summary>
			///  The file offset to the entry's data.
			/// </summary>
			[FieldOffset(64)]
			public uint Offset;
			/// <summary>
			///  The file length to the entry's data.
			/// </summary>
			[FieldOffset(68)]
			public int Length;

			#endregion

			#region Properties

			/// <summary>
			///  Gets the filename of the entry.
			/// </summary>
			public string FileName => FileNameRaw.ToNullTerminatedString(JapaneseEncoding);

			#endregion
		}
		/// <summary>
		///  We don't need to pass the <see cref="KIFENTRY.FileNameRaw"/> during P/Invoke, so we have this info
		///  structure.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
		internal struct KIFENTRYINFO {
			/// <summary>
			///  The file offset to the entry's data.
			/// </summary>
			public uint Offset;
			/// <summary>
			///  The file length to the entry's data.
			/// </summary>
			public int Length;
		}
	}
}
