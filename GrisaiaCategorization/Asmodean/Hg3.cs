using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Grisaia.Asmodean {
	/// <summary>
	///  Dimensions and other information extracted from an HG-3 file.
	/// </summary>
	[JsonObject]
	public sealed partial class Hg3 : IReadOnlyCollection<Hg3Image> /*: IDisposable*/ {
		#region Fields

		/// <summary>
		///  Gets the file name of the HG-3 file with the .hg3 extension.
		/// </summary>
		[JsonProperty("file_name")]
		public string FileName { get; private set; }
		/// <summary>
		///  Gets if the HG-3 frames were saved while expended.
		/// </summary>
		[JsonProperty("expanded")]
		public bool Expanded { get; private set; }
		/// <summary>
		///  Gets the list of images associated with this HG-3 file.
		/// </summary>
		[JsonIgnore]
		private IReadOnlyList<Hg3Image> images;

		#endregion

		#region Properties

		/// <summary>
		///  Gets the list of images associated with this HG-3 file.
		/// </summary>
		[JsonProperty("images")]
		public IReadOnlyList<Hg3Image> Images {
			get => images;
			set {
				images = value;
				foreach (Hg3Image image in images) {
					image.Hg3 = this;
				}
			}
		}
		/// <summary>
		///  Gets if this HG-3 has multiple frames.
		/// </summary>
		[JsonIgnore]
		public bool IsAnimation => Images.Count != 1 || Images[0].FrameCount != 1;
		/// <summary>
		///  Gets the number of HG-3 images in <see cref="Images"/>.
		/// </summary>
		[JsonIgnore]
		public int Count => Images.Count;
		/// <summary>
		///  Gets the total number of frames in this HG-3.
		/// </summary>
		[JsonIgnore]
		public int FrameCount => Images.Sum(i => i.FrameCount);
		/// <summary>
		///  Gets the name of the file for loading the <see cref="Hg3"/> data.
		/// </summary>
		[JsonIgnore]
		public string JsonFileName => $"{Path.GetFileNameWithoutExtension(FileName)}.hg3.json";
		/// <summary>
		///  Gets the name of the file for loading or saving a single <see cref="Hg3"/> bitmap when not an animation.
		/// </summary>
		[JsonIgnore]
		public string BitmapFileName => Path.ChangeExtension(FileName, ".png");
		/*/// <summary>
		///  Gets the bitmap for the HG-3 when <see cref="IsAnimation"/> is false.
		/// </summary>
		/// 
		/// <exception cref="InvalidOperationException">
		///  <see cref="IsAnimation"/> is true.
		/// </exception>
		[JsonIgnore]
		public Bitmap Bitmap {
			get {
				if (IsAnimation)
					throw new InvalidOperationException($"{nameof(Bitmap)} cannot be called when " +
														$"{nameof(IsAnimation)} is true!");
				return Images[0].Bitmaps[0];
			}
		}*/

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs an unassigned HG-3 for use with loading via <see cref="Newtonsoft.Json"/>.
		/// </summary>
		public Hg3() { }
		/*/// <summary>
		///  Constructs an HG-3 with the specified file name and <see cref="Hg3.HG3STDINFO"/>.
		/// </summary>
		/// <param name="fileName">The file name of the HG-3 with the .hg3 extension.</param>
		/// <param name="stdInfos">The HG3STDINFO struct array containing image dimension information.</param>
		/// <param name="bitmaps">The processed bitmap jagged array extracted from the HG-3 file.</param>
		internal Hg3(string fileName, Hg3.HG3STDINFO[] stdInfos, Bitmap[][] bitmaps) {
			FileName = fileName;
			Hg3Image[] images = new Hg3Image[stdInfos.Length];
			for (int i = 0; i < images.Length; i++) {
				images[i] = new Hg3Image(i, stdInfos[i], bitmaps[i], this);
			}
			Images = Array.AsReadOnly(images);
		}*/
		/// <summary>
		///  Constructs an HG-3 with the specified file name and <see cref="Hg3.HG3STDINFO"/>.
		/// </summary>
		/// <param name="fileName">The file name of the HG-3 with the .hg3 extension.</param>
		/// <param name="stdInfos">The HG3STDINFO struct array containing image dimension information.</param>
		internal Hg3(string fileName, Hg3.HG3STDINFO[] stdInfos, bool expand) {
			FileName = fileName;
			Expanded = expand;
			Hg3Image[] images = new Hg3Image[stdInfos.Length];
			for (int i = 0; i < images.Length; i++) {
				images[i] = new Hg3Image(i, stdInfos[i], this);
			}
			Images = Array.AsReadOnly(images);
		}

		#endregion

		#region Helpers

		/// <summary>
		///  Gets the file name for the PNG image with the specified image and frame indecies.
		/// </summary>
		/// <param name="imgIndex">
		///  The first index, which is assocaited to an <see cref="Hg3.ImageIndex"/>.
		/// </param>
		/// <param name="frmIndex">
		///  The second index, which is associated to a frame inside an <see cref="Hg3Image"/>.
		/// </param>
		/// <returns>The file name of the frame.</returns>
		public string GetFrameFileName(int imgIndex, int frmIndex) {
			return GetFrameFileName(FileName, imgIndex, frmIndex);
		}
		/// <summary>
		///  Gets the file name for the PNG image with the specified image and frame indecies.
		/// </summary>
		/// <param name="fileName">The base filename of the <see cref="Hg3"/>.</param>
		/// <param name="imgIndex">
		///  The first index, which is assocaited to an <see cref="Hg3Image.ImageIndex"/>.
		/// </param>
		/// <param name="frmIndex">
		///  The second index, which is associated to a frame inside an <see cref="Hg3Image"/>.
		/// </param>
		/// <returns>The file name of the HG-3 image frame.</returns>
		public static string GetFrameFileName(string fileName, int imgIndex, int frmIndex) {
			return $"{Path.GetFileNameWithoutExtension(fileName)}+{imgIndex:D3}+{frmIndex:D3}.png";
		}

		#endregion

		#region I/O
		
		/// <summary>
		///  Deserializes the HG-3 frame from a json file in the specified directory and file name.
		/// </summary>
		/// <param name="directory">
		///  The directory for the json file to load and deserialize with <paramref name="fileName"/>.
		/// </param>
		/// <returns>The deserialized HG-3 frame.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="directory"/> or <paramref name="fileName"/> is null.
		/// </exception>
		public static Hg3 FromJsonDirectory(string directory, string fileName) {
			if (directory == null)
				throw new ArgumentNullException(nameof(directory));
			if (fileName == null)
				throw new ArgumentNullException(nameof(fileName));
			string jsonFile = Path.Combine(directory, $"{Path.GetFileNameWithoutExtension(fileName)}.hg3.json");
			return JsonConvert.DeserializeObject<Hg3>(File.ReadAllText(jsonFile));
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
			string jsonFile = Path.Combine(directory, JsonFileName);
			File.WriteAllText(jsonFile, JsonConvert.SerializeObject(this, Formatting.Indented));
		}
		/// <summary>
		///  Opens the stream to the bitmap from the specified directory.
		/// </summary>
		/// <param name="directory">The directory to load the bitmap from using <see cref="BitmapFileName"/>.</param>
		/// <returns>The file stream to the bitmap.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="directory"/> is null.
		/// </exception>
		public FileStream OpenBitmapStream(string directory) {
			if (directory == null)
				throw new ArgumentNullException(nameof(directory));
			return File.OpenRead(Path.Combine(directory, BitmapFileName));
		}
		/// <summary>
		///  Opens the stream to the bitmap from the specified directory.
		/// </summary>
		/// <param name="directory">The directory to load the bitmap from using <see cref="GetFrameFileName"/>.</param>
		/// <param name="imgIndex">
		///  The first index, which is assocaited to an <see cref="Hg3.ImageIndex"/>.
		/// </param>
		/// <param name="frmIndex">
		///  The second index, which is associated to a frame inside an <see cref="Hg3Image"/>.
		/// </param>
		/// <returns>The file stream to the bitmap.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="directory"/> is null.
		/// </exception>
		/// <exception cref="IndexOutOfRangeException">
		///  <paramref name="imgIndex"/> or <paramref name="frmIndex"/> is out of range.
		/// </exception>
		public FileStream OpenBitmapStream(string directory, int imgIndex, int frmIndex) {
			if (directory == null)
				throw new ArgumentNullException(nameof(directory));
			if (frmIndex < 0 || frmIndex >= Images[imgIndex].FrameCount)
				throw new IndexOutOfRangeException($"{nameof(frmIndex)} is out of range!");
			return File.OpenRead(Path.Combine(directory, GetFrameFileName(imgIndex, frmIndex)));
		}

		#endregion

		/*#region IDisposable Implementation

		/// <summary>
		///  Disposes of the HG-3's image's stored <see cref="Hg3Image.Bitmaps"/>.
		/// </summary>
		public void Dispose() {
			foreach (Hg3Image image in Images) {
				image.Dispose();
			}
		}

		#endregion*/

		#region IEnumerable Implementation
		
		public IEnumerator<Hg3Image> GetEnumerator() => Images.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion
	}
}
