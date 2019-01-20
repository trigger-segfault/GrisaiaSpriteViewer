using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Asmodean;
using Newtonsoft.Json;

namespace Grisaia.Asmodean {
	/// <summary>
	///  Dimensions and other information extracted from an .hg3 file.
	/// </summary>
	public class Hg3Image {
		#region Fields

		/// <summary>
		///  Gets the file name of the sprite.
		/// </summary>
		[JsonProperty("file_name")]
		public string FileName { get; private set; }

		/// <summary>
		///  Gets the frame index of the sprite. -1 means no frame.
		/// </summary>
		[JsonProperty("frame")]
		public int Frame { get; private set; }

		/// <summary>
		///  Gets the condensed width of the sprite.
		/// </summary>
		[JsonProperty("width")]
		public int Width { get; private set; }
		/// <summary>
		///  Gets the condensed height of the sprite.
		/// </summary>
		[JsonProperty("height")]
		public int Height { get; private set; }
		/// <summary>
		///  Gets the total width of the sprite with offsets applied.
		/// </summary>
		[JsonProperty("total_width")]
		public int TotalWidth { get; private set; }
		/// <summary>
		/// Gets the height width of the sprite with offsets applied.
		/// </summary>
		[JsonProperty("total_height")]
		public int TotalHeight { get; private set; }
		/// <summary>
		///  Gets the horizontal offset of the sprite from the left.
		/// </summary>
		[JsonProperty("offset_x")]
		public int OffsetX { get; private set; }
		/// <summary>
		///  Gets the vertical offset of the sprite from the top.
		/// </summary>
		[JsonProperty("offset_y")]
		public int OffsetY { get; private set; }

		/// <summary>
		///  This value is unknown, but it always seems to be 1 for character sprites.
		/// </summary>
		[JsonProperty("unknown")]
		public int Unknown { get; private set; }
		/// <summary>
		///  Gets the horizontal center of the sprite. Used for drawing in the game.
		/// </summary>
		[JsonProperty("center")]
		public int Center { get; private set; }
		/// <summary>
		///  Gets the vertical baseline of the sprite. Used for drawing in the game.
		/// </summary>
		[JsonProperty("baseline")]
		public int Baseline { get; private set; }

		#endregion

		#region Properties

		[JsonIgnore]
		public int MarginLeft => OffsetX;
		[JsonIgnore]
		public int MarginTop => OffsetY;
		[JsonIgnore]
		public int MarginRight => TotalWidth - Width - MarginLeft;
		[JsonIgnore]
		public int MarginBottom => TotalHeight - Height - MarginTop;

		#endregion

		#region Constructors

		public Hg3Image() { }
		internal Hg3Image(string fileName, Hgx2png.HG3STDINFO stdInfo) {
			FileName = fileName;
			Width = stdInfo.Width;
			Height = stdInfo.Height;
			TotalWidth = stdInfo.TotalWidth;
			TotalHeight = stdInfo.TotalHeight;
			OffsetX = stdInfo.OffsetX;
			OffsetY = stdInfo.OffsetY;
			Unknown = stdInfo.Unknown;
			Center = stdInfo.Center;
			Baseline = stdInfo.Baseline;
		}

		#endregion
	}
}
