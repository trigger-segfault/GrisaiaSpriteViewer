using Newtonsoft.Json;

namespace Grisaia.Asmodean {
	/// <summary>
	///  Dimensions and other information extracted from an HG-3 image.
	/// </summary>
	public sealed class Hg3Image {
		#region Fields

		/// <summary>
		///  Gets the HG-3 that contains this image set.
		/// </summary>
		[JsonIgnore]
		public Hg3 Hg3 { get; internal set; }

		/// <summary>
		///  Gets the file name of the image with the .hg3 extension.
		/// </summary>
		//[JsonProperty("file_name")]
		[JsonIgnore]
		//public string FileName { get; private set; }
		public string FileName => Hg3.FileName;
		/// <summary>
		///  Gets the index of the HG-3 image in the HG-3 file. This number comes before the frame index in the file
		///  name.
		/// </summary>
		[JsonProperty("image_index")]
		public int ImageIndex { get; private set; }
		/*/// <summary>
		///  Gets the number of frames in the animation.
		/// </summary>
		[JsonProperty("frame_count")]
		public int FrameCount { get; private set; }*/

		/// <summary>
		///  Gets the condensed width of the image.
		/// </summary>
		[JsonProperty("width")]
		public int Width { get; private set; }
		/// <summary>
		///  Gets the condensed height of the image.
		/// </summary>
		[JsonProperty("height")]
		public int Height { get; private set; }
		/// <summary>
		///  Gets the total width of the image with offsets applied.
		/// </summary>
		[JsonProperty("total_width")]
		public int TotalWidth { get; private set; }
		/// <summary>
		/// Gets the height width of the image with offsets applied.
		/// </summary>
		[JsonProperty("total_height")]
		public int TotalHeight { get; private set; }
		/// <summary>
		///  Gets the horizontal offset of the image from the left.
		/// </summary>
		[JsonProperty("offset_x")]
		public int OffsetX { get; private set; }
		/// <summary>
		///  Gets the vertical offset of the image from the top.
		/// </summary>
		[JsonProperty("offset_y")]
		public int OffsetY { get; private set; }

		/*/// <summary>
		///  This value is unknown, but it always seems to be 1 for character images.
		/// </summary>
		[JsonProperty("unknown")]
		public int Unknown { get; private set; }*/
		/// <summary>
		///  Gets the number of frames in the animation.
		/// </summary>
		[JsonProperty("frame_count")]
		public int FrameCount { get; private set; }
		/// <summary>
		///  Gets the horizontal center of the image. Used for drawing in the game.
		/// </summary>
		[JsonProperty("center")]
		public int Center { get; private set; }
		/// <summary>
		///  Gets the vertical baseline of the image. Used for drawing in the game.
		/// </summary>
		[JsonProperty("baseline")]
		public int Baseline { get; private set; }

		/*/// <summary>
		///  The bitmaps loaded with the HG-3 image dimensions. This may be null of the image is disposed of.
		/// </summary>
		[JsonIgnore]
		public IReadOnlyList<Bitmap> Bitmaps { get; private set; }*/

		#endregion

		#region Properties

		/// <summary>
		///  Gets the left margin of the image when expanded.<para/>
		///  This is the same as <see cref="OffsetX"/>.
		/// </summary>
		[JsonIgnore]
		public int MarginLeft => OffsetX;
		/// <summary>
		///  Gets the top margin of the image when expanded.<para/>
		///  This is the same as <see cref="OffsetY"/>.
		/// </summary>
		[JsonIgnore]
		public int MarginTop => OffsetY;
		/// <summary>
		///  Gets the bottom margin of the image when expanded.<para/>
		///  This is the same as <see cref="TotalWidth"/> - <see cref="Width"/> - <see cref="OffsetX"/>.
		/// </summary>
		[JsonIgnore]
		public int MarginRight => TotalWidth - Width - OffsetX;
		/// <summary>
		///  Gets the bottom margin of the image when expanded.<para/>
		///  This is the same as <see cref="TotalHeight"/> - <see cref="Height"/> - <see cref="OffsetY"/>.
		/// </summary>
		[JsonIgnore]
		public int MarginBottom => TotalHeight - Height - OffsetY;

		/*/// <summary>
		///  Gets if this HG-3 image has multiple frames.
		/// </summary>
		[JsonIgnore]
		public bool IsAnimation => FrameCount > 1;
		/// <summary>
		///  Gets the name of the file for loading the <see cref="Bitmaps"/> image data.
		/// </summary>
		[JsonIgnore]
		public string BitmapFileName => Path.ChangeExtension(FileName, ".png");
		/// <summary>
		///  Gets the name of the file for loading the <see cref="Hg3Frame"/> data.
		/// </summary>
		[JsonIgnore]
		public string JsonFileName => $"{Path.GetFileNameWithoutExtension(FileName)}.hg3.json";*/

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs an unassigned HG-3 image for use with loading via <see cref="Newtonsoft.Json"/>.
		/// </summary>
		public Hg3Image() { }
		/// <summary>
		///  Constructs an HG-3 image with the specified file name, image index, <see cref="Hg3.HG3STDINFO"/>, and
		///  bitmap frames.
		/// </summary>
		/// <param name="fileName">The file name of the image with the .hg3 extension.</param>
		/// <param name="imageIndex">The frame index of the image.</param>
		/// <param name="stdInfo">The HG3STDINFO struct containing image dimension information.</param>
		/// <param name="bitmap">The processed bitmap extracted from the HG-3 file.</param>
		/*internal Hg3Frame(string fileName, int imageIndex, Hg3.HG3STDINFO stdInfo, Bitmap[] bitmaps) {
			FileName = fileName;
			ImageIndex = imageIndex;
			FrameCount = bitmaps.Length;

			Width = stdInfo.Width;
			Height = stdInfo.Height;
			TotalWidth = stdInfo.TotalWidth;
			TotalHeight = stdInfo.TotalHeight;
			OffsetX = stdInfo.OffsetX;
			OffsetY = stdInfo.OffsetY;
			Unknown = stdInfo.Unknown;
			Center = stdInfo.Center;
			Baseline = stdInfo.Baseline;

			Bitmaps = bitmaps;
		}*/
		/// <summary>
		///  Constructs an HG-3 image with the specified file name, image index, <see cref="Hg3.HG3STDINFO"/>, and
		///  bitmap frames.
		/// </summary>
		/// <param name="imageIndex">The frame index of the image.</param>
		/// <param name="stdInfo">The HG3STDINFO struct containing image dimension information.</param>
		/// <param name="hg3">The HG-3 containing this image set.</param>
		internal Hg3Image(int imageIndex, Hg3.HG3STDINFO stdInfo, Hg3 hg3) {
			Hg3 = hg3;
			ImageIndex = imageIndex;
			//FrameCount = bitmaps.Length;

			Width = stdInfo.Width;
			Height = stdInfo.Height;
			TotalWidth = stdInfo.TotalWidth;
			TotalHeight = stdInfo.TotalHeight;
			OffsetX = stdInfo.OffsetX;
			OffsetY = stdInfo.OffsetY;
			//Unknown = stdInfo.FrameCount;
			FrameCount = stdInfo.FrameCount;
			Center = stdInfo.Center;
			Baseline = stdInfo.Baseline;

			//Bitmaps = Array.AsReadOnly(bitmaps);
		}

		#endregion

		#region Helpers

		/// <summary>
		///  Gets the file name for the PNG image with the specified image and frame indecies.
		/// </summary>
		/// <param name="frmIndex">
		///  The second index, which is associated to a frame inside an <see cref="Hg3Image"/>.
		/// </param>
		/// <returns>The file name of the frame.</returns>
		public string GetFrameFileName(int frmIndex) {
			return Hg3.GetFrameFileName(FileName, ImageIndex, frmIndex);
		}

		#endregion

		/*#region I/O

		/// <summary>
		///  Deserializes the HG-3 frame from a json file.
		/// </summary>
		/// <param name="jsonFile">The path to the json file to load and deserialize.</param>
		/// <returns>The deserialized HG-3 frame.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="jsonFile"/> is null.
		/// </exception>
		public static Hg3Frame FromJsonFile(string jsonFile) {
			return JsonConvert.DeserializeObject<Hg3Frame>(File.ReadAllText(jsonFile));
		}
		/// <summary>
		///  Serializes the HG-3 frame to a json file.
		/// </summary>
		/// <param name="jsonFile">The path to the json file to serialize and save.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="jsonFile"/> is null.
		/// </exception>
		public void SaveJsonToFile(string jsonFile) {
			File.WriteAllText(jsonFile, JsonConvert.SerializeObject(this, Formatting.Indented));
		}
		/// <summary>
		///  Serializes the HG-3 frame to a json file in the specified directory.
		/// </summary>
		/// <param name="directory">The directory to save the json file to using <see cref="JsonFileName"/>.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="directory"/> is null.
		/// </exception>
		public void SaveJsonToDirectory(string directory) {
			if (directory == null)
				throw new ArgumentNullException(nameof(directory));
			SaveJsonToFile(Path.Combine(directory, JsonFileName));
		}
		/// <summary>
		///  Saves the loaded bitmap to a file as a PNG.
		/// </summary>
		/// <param name="bitmapFile">The path to the file to save the bitmap to.</param>
		/// <param name="dispose">True if the bitmap is disposed of after saving.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="bitmapFile"/> is null.
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		///  <see cref="Bitmaps"/> has been disposed of or was never loaded.
		/// </exception>
		public void SaveBitmapToFile(string bitmapFile, bool dispose) {
			SaveBitmapToFile(bitmapFile, ImageFormat.Png, dispose);
		}
		/// <summary>
		///  Saves the loaded bitmap to a file as the specified format.
		/// </summary>
		/// <param name="bitmapFile">The path to the file to save the bitmap to.</param>
		/// <param name="format">The image format to save the bitmap as.</param>
		/// <param name="dispose">True if the bitmap is disposed of after saving.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="bitmapFile"/> is null.
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		///  <see cref="Bitmaps"/> has been disposed of or was never loaded.
		/// </exception>
		public void SaveBitmapToFile(string bitmapFile, ImageFormat format, bool dispose) {
			if (Bitmaps == null)
				throw new ObjectDisposedException($"This {nameof(Hg3Frame)} does not have a loaded bitmap!");
			Bitmaps.Save(bitmapFile, format);
		}
		/// <summary>
		///  Saves the loaded bitmap to a file as a PNG in the specified directory.
		/// </summary>
		/// <param name="directory">The directory to save the bitmap to using <see cref="BitmapFileName"/>.</param>
		/// <param name="dispose">True if the bitmap is disposed of after saving.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="directory"/> is null.
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		///  <see cref="Bitmaps"/> has been disposed of or was never loaded.
		/// </exception>
		public void SaveBitmapToDirectory(string directory, bool dispose) {
			SaveBitmapToDirectory(directory, ImageFormat.Png, dispose);
		}
		/// <summary>
		///  Saves the loaded bitmap to a file as the specified format in the specified directory.
		/// </summary>
		/// <param name="directory">The directory to save the bitmap to using <see cref="BitmapFileName"/>.</param>
		/// <param name="format">The image format to save the bitmap as.</param>
		/// <param name="dispose">True if the bitmap is disposed of after saving.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="directory"/> is null.
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		///  <see cref="Bitmaps"/> has been disposed of or was never loaded.
		/// </exception>
		public void SaveBitmapToDirectory(string directory, ImageFormat format, bool dispose) {
			if (directory == null)
				throw new ArgumentNullException(nameof(directory));
			SaveBitmapToFile(Path.Combine(directory, BitmapFileName), format, dispose);
		}
		/// <summary>
		///  Saves the loaded bitmap and HG-3 frame to files in the specified directory.<para/>
		///  <see cref="Bitmaps"/> will use the PNG format.
		/// </summary>
		/// <param name="directory">
		///  The directory to save the bitmap and HG-3 frame to using <see cref="BitmapFileName"/> and
		///  <see cref="JsonFileName"/>.
		///  </param>
		/// <param name="dispose">True if the bitmap is disposed of after saving.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="directory"/> is null.
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		///  <see cref="Bitmaps"/> has been disposed of or was never loaded.
		/// </exception>
		public void SaveToDirectory(string directory, bool dispose) {
			SaveJsonToDirectory(directory);
			SaveBitmapToDirectory(directory, dispose);
		}
		/// <summary>
		///  Saves the loaded bitmap and HG-3 frame to files in the specified directory.<para/>
		///  <see cref="Bitmaps"/> will use the specified format.
		/// </summary>
		/// <param name="directory">
		///  The directory to save the bitmap and HG-3 frame to using <see cref="BitmapFileName"/> and
		///  <see cref="JsonFileName"/>.
		///  </param>
		/// <param name="format">The image format to save the bitmap as.</param>
		/// <param name="dispose">True if the bitmap is disposed of after saving.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="directory"/> is null.
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		///  <see cref="Bitmaps"/> has been disposed of or was never loaded.
		/// </exception>
		public void SaveToDirectory(string directory, ImageFormat format, bool dispose) {
			SaveJsonToDirectory(directory);
			SaveBitmapToDirectory(directory, format, dispose);
		}
		/// <summary>
		///  Opens the stream to the bitmap from the specified directory.
		/// </summary>
		/// <param name="directory">The directory to load the bitmap from using <see cref="BitmapFileName"/>.</param>
		/// <returns>The file stream to the bitmap.</returns>
		public FileStream OpenBitmapStream(string directory) {
			return File.OpenRead(Path.Combine(directory, BitmapFileName));
		}

		#endregion*/

		/*#region IDisposable Implementation

		/// <summary>
		///  Disposes of the HG-3 image's frames stored in <see cref="Bitmaps"/>.
		/// </summary>
		internal void Dispose() {
			if (Bitmaps != null) {
				foreach (Bitmap bitmap in Bitmaps)
					bitmap.Dispose();
				Bitmaps = null;
			}
		}

		#endregion*/
	}
}
