using Grisaia.Asmodean;
using Grisaia.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Grisaia {
	/*/// <summary>How converted pngs should be categorized into folders.</summary>
	[Flags]
	public enum Hg3Sorting {
		/// <summary>No categorization.</summary>
		None = 0,

		/// <summary>Pngs are saved into a single folder.</summary>
		Unsorted = (1 << 0),
		/// <summary>Pngs are saved into categorized folders.</summary>
		Sorted = (1 << 1),

		/// <summary>Pngs are saved into both a single folder and categorized folders.</summary>
		Both = Sorted | Unsorted,
	}
	
	/// <summary>Arguments for .int extraction callbacks.</summary>
	public struct ExkifintArgs {
		/// <summary>The index of the current file being extracted.</summary>
		public int FileIndex { get; set; }
		/// <summary>The total number of files to extract.</summary>
		public int FileCount { get; set; }
		/// <summary>The name of the file without the extension.</summary>
		public string FileName { get; set; }
		/// <summary>The completion percentage.</summary>
		public double Percent { get; set; }
		/// <summary>The time ellapsed since the operation started.</summary>
		public TimeSpan Ellapsed { get; set; }
	}

	/// <summary>Arguments for .hg3 extraction callbacks.</summary>
	public struct Hgx2pngArgs {
		/// <summary>The index of the current file being extracted.</summary>
		public int FileIndex { get; set; }
		/// <summary>The total number of files to extract.</summary>
		public int FileCount { get; set; }
		/// <summary>The full path to the file.</summary>
		public string FilePath { get; set; }
		/// <summary>The name of the file without the extension.</summary>
		public string FileName { get; set; }
		/// <summary>The completion percentage.</summary>
		public double Percent { get; set; }
		/// <summary>The time ellapsed since the operation started.</summary>
		public TimeSpan Ellapsed { get; set; }
		
		/// <summary>The total number of errors that occurred during the operation.</summary>
		public int TotalErrors { get; set; }
	}*/
	
	/*/// <summary>A callback while extracting an .int file.</summary>
	public delegate bool ExkifintCallback(ExkifintArgs args);
	/// <summary>A callback while extracting .hg3's.</summary>
	public delegate bool Hgx2pngCallback(Hgx2pngArgs args);
	/// <summary>A callback with an error while extracting .hg3's.</summary>
	public delegate bool Hgx2pngErrorCallback(Exception ex, Hgx2pngArgs args);*/

}
