using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Grisaia.Extensions;

namespace Grisaia.SpriteViewer {
	/// <summary>
	/// https://stackoverflow.com/a/46424800/7517185
	/// </summary>
	public class ImageClipboard {
		/// <summary>
		/// Copies the given image to the clipboard as PNG, DIB and standard Bitmap format.
		/// </summary>
		/// <param name="image">Image to put on the clipboard.</param>
		/// <param name="imageNoTr">Optional specifically nontransparent version of the image to put on the clipboard.</param>
		/// <param name="data">Clipboard data object to put the image into. Might already contain other stuff. Leave null to create a new one.</param>
		public static void SetClipboardImage(Bitmap image, Bitmap imageNoTr, string tempFile, DataObject data = null) {
			/*IDataObject content = Clipboard.GetDataObject();
			string[] formats = content.GetFormats();
			Console.WriteLine(string.Join("\n", formats));
			if (content.GetDataPresent(DataFormats.FileDrop)) {
				Console.WriteLine(string.Join("\n", (string[]) content.GetData(DataFormats.FileDrop)));
			}*/
			/*{
				data = new DataObject();
				foreach (string format in formats) {
					if (format != "Preferred DropEffect" && !format.Contains("moz") && !format.Contains("html") && !format.Contains("HTML")) {
						data.SetData(format, false, content.GetData(format, false));
					}
				}
				Clipboard.SetDataObject(data);
				return;
				data = null;
			}*/
			/*byte[] dib_a = new byte[0], dib_b = new byte[0], dibV5_a = new byte[0], dibV5_b = new byte[0];
			BITMAPINFOHEADER bmi_a = new BITMAPINFOHEADER();
			BITMAPINFOHEADER bmi_b = new BITMAPINFOHEADER();
			BITMAPV5HEADER bmiV5_a = new BITMAPV5HEADER();
			BITMAPV5HEADER bmiV5_b = new BITMAPV5HEADER();
			if (content.GetDataPresent(DataFormats.Dib)) {
				using (var ms = (MemoryStream) content.GetData(DataFormats.Dib)) {
					dib_a = ms.ToArray();
					BinaryReader reader = new BinaryReader(ms);
					bmi_a = reader.ReadStruct<BITMAPINFOHEADER>();
				}
			}
			if (content.GetDataPresent("Format17")) {
				using (var ms = (MemoryStream) content.GetData("Format17")) {
					dibV5_a = ms.ToArray();
					BinaryReader reader = new BinaryReader(ms);
					bmiV5_a = reader.ReadStruct<BITMAPV5HEADER>();
				}
			}
			object htmlData = null;
			string html, html2;*/
			/*Console.WriteLine(DataFormats.Html);
			if (content.GetDataPresent("text/html", false)) {
				htmlData = content.GetData("text/html", true);
				Console.WriteLine(htmlData);
				using (var ms = (MemoryStream) content.GetData("text/html", false)) {
					StreamReader reader = new StreamReader(ms, Encoding.Unicode);
					html = reader.ReadToEnd();
					Console.WriteLine(html);
				}
				html2 = (string) content.GetData(DataFormats.Html, true);
			}*/
			Clipboard.Clear();
			if (data == null)
				data = new DataObject();
			if (imageNoTr == null)
				imageNoTr = image;
			//using (image = (Bitmap) Image.FromFile(@"C:\Users\Onii-chan\Source\C#\GrisaiaExtractor\GrisaiaBrowser\bin\Debug\cache\sachi_scales.png"))
			using (MemoryStream pngMemStream = new MemoryStream())
			//using (MemoryStream png2MemStream = new MemoryStream())
			//using (MemoryStream htmlMemStream = new MemoryStream())
			using (MemoryStream dibMemStream = new MemoryStream())
			using (MemoryStream dibv5MemStream = new MemoryStream()) {
				// As standard bitmap, without transparency support
				data.SetData(DataFormats.Bitmap, true, imageNoTr);
				// As PNG. Gimp will prefer this over the other two.
				image.Save(pngMemStream, ImageFormat.Png);
				data.SetData("PNG", false, pngMemStream);
				// As DIB. This is (wrongly) accepted as ARGB by many applications.

				#region HTML
				/*if (tempFile != null) {
					string src = "file:///" + HttpUtility.HtmlEncode(tempFile.Replace('\\', '/'));
					int startHTML, endHTML, startFragment, endFragment;
					string img = $"<img src=\"{src}\" alt=\"{src}\" class=\"transparent\">";
					string htmlTmp = "Version:0.9" + Environment.NewLine +
									 "StartHTML:00000000" + Environment.NewLine +
									 "EndHTML:00000000" + Environment.NewLine +
									 "StartFragment:00000000" + Environment.NewLine +
									 "EndFragment:00000000" + Environment.NewLine;

					startHTML = htmlTmp.Length;
					htmlTmp       += Environment.NewLine +
									 "<html><body>" + Environment.NewLine +
									 "<!--StartFragment-->";
					startFragment = htmlTmp.Length;
					htmlTmp += img;
					endFragment = htmlTmp.Length;
					htmlTmp       += Environment.NewLine +
									 "</body>" + Environment.NewLine +
									 "</html>";
					endHTML = htmlTmp.Length;

					string imgHtml = $"Version:0.9" + Environment.NewLine +
									 $"StartHTML:{startHTML:D8}" + Environment.NewLine +
									 $"EndHTML:{endHTML:D8}" + Environment.NewLine +
									 $"StartFragment:{startFragment:D8}" + Environment.NewLine +
									 $"EndFragment:{endFragment:D8}" + Environment.NewLine +
									 $"<html><body>" + Environment.NewLine +
									 $"<!--StartFragment-->" + img + "<!--EndFragment-->" + Environment.NewLine +
									 $"</body>" + Environment.NewLine +
									 $"</html>";
					StreamWriter writer = new StreamWriter(htmlMemStream, Encoding.Unicode);
					writer.Write(img + "?");
					writer.Flush();
					//data.SetData("text/html", false, htmlMemStream);
					//data.SetData(DataFormats.Html, true, imgHtml);
				}*/
				#endregion

				byte[] dibData = ConvertToDib(image);
				//if (dib_a != null)
				//	dibMemStream.Write(dib_a, 0, dib_a.Length);
				//else
				dibMemStream.Write(dibData, 0, dibData.Length);
				data.SetData(DataFormats.Dib, false, dibMemStream);

				byte[] dibv5Data = BitmapHelper.CreatePackedDIBV5(image);
				//if (dibV5_a != null)
				//	dibv5MemStream.Write(dibV5_a, 0, dibV5_a.Length);
				//else
				dibv5MemStream.Write(dibv5Data, 0, dibv5Data.Length);
				data.SetData("Format17", false, dibv5MemStream);

				if (tempFile != null)
					data.SetFileDropList(new StringCollection { tempFile });

				// The 'copy=true' argument means the MemoryStreams can be safely disposed after the operation.
				Clipboard.SetDataObject(data, true);
			}
			/*content = Clipboard.GetDataObject();
			formats = content.GetFormats();
			if (content.GetDataPresent("text/html", false)) {
				htmlData = content.GetData("text/html", false);
				Console.WriteLine(htmlData);
				if (htmlData is MemoryStream ms) {
					using (ms) {
						StreamReader reader = new StreamReader(ms, Encoding.Unicode);
						html = reader.ReadToEnd();
						Console.WriteLine(html);
					}
				}
				html2 = (string) content.GetData(DataFormats.Html, true);
			}
			//Console.WriteLine(string.Join("\n", formats));
			if (content.GetDataPresent(DataFormats.Dib)) {
				using (var ms = (MemoryStream) content.GetData(DataFormats.Dib)) {
					dib_b = ms.ToArray();
					BinaryReader reader = new BinaryReader(ms);
					bmi_b = reader.ReadStruct<BITMAPINFOHEADER>();
				}
			}
			if (content.GetDataPresent("Format17")) {
				using (var ms = (MemoryStream) content.GetData("Format17")) {
					dibV5_b = ms.ToArray();
					BinaryReader reader = new BinaryReader(ms);
					bmiV5_b = reader.ReadStruct<BITMAPV5HEADER>();
				}
			}
			Dictionary<int, KeyValuePair<byte, byte>> dif_dib = new Dictionary<int, KeyValuePair<byte, byte>>();
			Dictionary<int, KeyValuePair<byte, byte>> dif_dibV5 = new Dictionary<int, KeyValuePair<byte, byte>>();
			for (int i = 0; i < dib_a.Length && i < dib_b.Length; i++) {
				byte a = dib_a[i];
				byte b = dib_b[i];
				if (a != b)
					dif_dib.Add(i, new KeyValuePair<byte, byte>(a, b));
			}
			for (int i = 0; i < dibV5_a.Length && i < dibV5_b.Length; i++) {
				byte a = dibV5_a[i];
				byte b = dibV5_b[i];
				if (a != b)
					dif_dibV5.Add(i, new KeyValuePair<byte, byte>(a, b));
			}
			Console.Write("");*/
		}

		/// <summary>
		/// Converts the image to Device Independent Bitmap format of type BITFIELDS.
		/// This is (wrongly) accepted by many applications as containing transparency,
		/// so I'm abusing it for that.
		/// </summary>
		/// <param name="image">Image to convert to DIB</param>
		/// <returns>The image converted to DIB, in bytes.</returns>
		private static byte[] ConvertToDib(Image image) {
			byte[] bm32bData;
			int width = image.Width;
			int height = image.Height;
			// Ensure image is 32bppARGB by painting it on a new 32bppARGB image.
			using (Bitmap bm32b = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppPArgb)) {
				using (Graphics gr = Graphics.FromImage(bm32b))
					gr.DrawImage(image, new Rectangle(0, 0, bm32b.Width, bm32b.Height));
				// Bitmap format has its lines reversed.
				bm32b.RotateFlip(RotateFlipType.Rotate180FlipX);
				bm32bData = ImageUtils.GetImageData(bm32b, out _);
			}
			// BITMAPINFOHEADER struct for DIB.
			int hdrSize = 0x28;
			byte[] fullImage = new byte[hdrSize + bm32bData.Length];
			//Int32 biSize;
			ArrayUtils.WriteIntToByteArray(fullImage, 0x00, 4, true, (uint) hdrSize);
			//Int32 biWidth;
			ArrayUtils.WriteIntToByteArray(fullImage, 0x04, 4, true, (uint) width);
			//Int32 biHeight;
			ArrayUtils.WriteIntToByteArray(fullImage, 0x08, 4, true, (uint) height);
			//Int16 biPlanes;
			ArrayUtils.WriteIntToByteArray(fullImage, 0x0C, 2, true, 1);
			//Int16 biBitCount;
			ArrayUtils.WriteIntToByteArray(fullImage, 0x0E, 2, true, 32);
			//BITMAPCOMPRESSION biCompression = BITMAPCOMPRESSION.BITFIELDS;
			//ArrayUtils.WriteIntToByteArray(fullImage, 0x10, 4, true, 3);
			//Int32 biSizeImage;
			ArrayUtils.WriteIntToByteArray(fullImage, 0x14, 4, true, (uint) bm32bData.Length);
			// These are all 0. Since .net clears new arrays, don't bother writing them.
			//Int32 biXPelsPerMeter = 0;
			//Int32 biYPelsPerMeter = 0;
			//Int32 biClrUsed = 0;
			//Int32 biClrImportant = 0;

			// The aforementioned "BITFIELDS": colour masks applied to the Int32 pixel value to get the R, G and B values.
			/*ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 0, 4, true, 0x00FF0000);
			ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 4, 4, true, 0x0000FF00);
			ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 8, 4, true, 0x000000FF);*/
			Array.Copy(bm32bData, 0, fullImage, hdrSize, bm32bData.Length);
			return fullImage;
		}

		/*[StructLayout(LayoutKind.Sequential)]
		private struct BITMAPV5HEADER {
			public uint bV5Size;
			public int bV5Width;
			public int bV5Height;
			public ushort bV5Planes;
			public ushort bV5BitCount;
			public uint bV5Compression;
			public uint bV5SizeImage;
			public int bV5XPelsPerMeter;
			public int bV5YPelsPerMeter;
			public ushort bV5ClrUsed;
			public ushort bV5ClrImportant;
			public ushort bV5RedMask;
			public ushort bV5GreenMask;
			public ushort bV5BlueMask;
			public ushort bV5AlphaMask;
			public ushort bV5CSType;
			public IntPtr bV5Endpoints;
			public ushort bV5GammaRed;
			public ushort bV5GammaGreen;
			public ushort bV5GammaBlue;
			public ushort bV5Intent;
			public ushort bV5ProfileData;
			public ushort bV5ProfileSize;
			public ushort bV5Reserved;
		}*/
		/*public static Bitmap CF_DIBV5ToBitmap(byte[] data) {
			GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			var bmi = (BITMAPV5HEADER) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(BITMAPV5HEADER));
			Bitmap bitmap = new Bitmap((int) bmi.bV5Width, (int) bmi.bV5Height, -
									   (int) (bmi.bV5SizeImage / bmi.bV5Height), PixelFormat.Format32bppArgb,
									   new IntPtr(handle.AddrOfPinnedObject().ToInt32()
									   + bmi.bV5Size + (bmi.bV5Height - 1)
									   * (int) (bmi.bV5SizeImage / bmi.bV5Height)));
			handle.Free();
			return bitmap;
		}

		public static byte[] BitmapToCF_DIBV5(Bitmap bitmap) {
			BITMAPV5HEADER info = new BITMAPV5HEADER();
			info.bV5Size = (uint) Marshal.SizeOf<BITMAPV5HEADER>();
			info.bV5Width = bitmap.Width;
			info.bV5Height = -bitmap.Height;
			info.bV5Planes = 1; // The docs say this is the only valid value here.
			info.bV5BitCount = 32;
			info.bV5Compression = (uint) BI.BI_BITFIELDS;
			info.bV5SizeImage = (uint)(bitmap.Width * bitmap.Height * 4);
			info.bV5RedMask   = 0xff000000;
			info.bV5GreenMask = 0x00ff0000;
			info.bV5BlueMask  = 0x0000ff00;
			info.bV5AlphaMask = 0x000000ff;
		}*/
		private enum BI : uint {
			BI_RGB = 0,
			BI_RLE8 = 1,
			BI_RLE4 = 2,
			BI_BITFIELDS = 3,
			BI_JPEG = 4,
			BI_PNG = 5
		}
		[StructLayout(LayoutKind.Explicit)]
		private struct BITMAPV5HEADER {
			[FieldOffset(0)]
			public uint bV5Size;
			[FieldOffset(4)]
			public int bV5Width;
			[FieldOffset(8)]
			public int bV5Height;
			[FieldOffset(12)]
			public ushort bV5Planes;
			[FieldOffset(14)]
			public ushort bV5BitCount;
			[FieldOffset(16)]
			public uint bV5Compression;
			[FieldOffset(20)]
			public uint bV5SizeImage;
			[FieldOffset(24)]
			public int bV5XPelsPerMeter;
			[FieldOffset(28)]
			public int bV5YPelsPerMeter;
			[FieldOffset(32)]
			public uint bV5ClrUsed;
			[FieldOffset(36)]
			public uint bV5ClrImportant;
			[FieldOffset(40)]
			public uint bV5RedMask;
			[FieldOffset(44)]
			public uint bV5GreenMask;
			[FieldOffset(48)]
			public uint bV5BlueMask;
			[FieldOffset(52)]
			public uint bV5AlphaMask;
			[FieldOffset(56)]
			public uint bV5CSType;
			[FieldOffset(60)]
			public CIEXYZTRIPLE bV5Endpoints;
			[FieldOffset(96)]
			public uint bV5GammaRed;
			[FieldOffset(100)]
			public uint bV5GammaGreen;
			[FieldOffset(104)]
			public uint bV5GammaBlue;
			[FieldOffset(108)]
			public uint bV5Intent;
			[FieldOffset(112)]
			public uint bV5ProfileData;
			[FieldOffset(116)]
			public uint bV5ProfileSize;
			[FieldOffset(120)]
			public uint bV5Reserved;
		}
		[StructLayout(LayoutKind.Sequential)]
		internal struct CIEXYZ {
			public uint ciexyzX; //FXPT2DOT30
			public uint ciexyzY; //FXPT2DOT30
			public uint ciexyzZ; //FXPT2DOT30
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct CIEXYZTRIPLE {
			public CIEXYZ ciexyzRed;
			public CIEXYZ ciexyzGreen;
			public CIEXYZ ciexyzBlue;
		}

		private static class ImageUtils {
			/// <summary>
			/// Gets the raw bytes from an image.
			/// </summary>
			/// <param name="sourceImage">The image to get the bytes from.</param>
			/// <param name="stride">Stride of the retrieved image data.</param>
			/// <returns>The raw bytes of the image</returns>
			public static Byte[] GetImageData(Bitmap sourceImage, out Int32 stride) {
				BitmapData sourceData = sourceImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly, sourceImage.PixelFormat);
				stride = sourceData.Stride;
				Byte[] data = new Byte[stride * sourceImage.Height];
				Marshal.Copy(sourceData.Scan0, data, 0, data.Length);
				sourceImage.UnlockBits(sourceData);
				return data;
			}
		}

		private static class ArrayUtils {
			public static void WriteIntToByteArray(byte[] data, int startIndex, int bytes, bool littleEndian, uint value) {
				int lastByte = bytes - 1;
				if (data.Length < startIndex + bytes)
					throw new ArgumentOutOfRangeException("startIndex", "Data array is too small to write a " + bytes + "-byte value at offset " + startIndex + ".");
				for (int index = 0; index < bytes; index++) {
					int offs = startIndex + (littleEndian ? index : lastByte - index);
					data[offs] = (byte) (value >> (8 * index) & 0xFF);
				}
			}

			public static uint ReadIntFromByteArray(byte[] data, int startIndex, int bytes, bool littleEndian) {
				int lastByte = bytes - 1;
				if (data.Length < startIndex + bytes)
					throw new ArgumentOutOfRangeException("startIndex", "Data array is too small to read a " + bytes + "-byte value at offset " + startIndex + ".");
				uint value = 0;
				for (int index = 0; index < bytes; index++) {
					int offs = startIndex + (littleEndian ? index : lastByte - index);
					value += (uint) (data[offs] << (8 * index));
				}
				return value;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct BITMAPINFOHEADER {
			public uint biSize;
			public int biWidth;
			public int biHeight;
			public ushort biPlanes;
			public ushort biBitCount;
			public uint biCompression;
			public uint biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public uint biClrUsed;
			public uint biClrImportant;
		}
		[StructLayout(LayoutKind.Sequential)]
		internal struct BITFIELDS {
			public uint BlueMask;
			public uint GreenMask;
			public uint RedMask;
		}
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct BITMAPINFO {
			/// <summary>
			/// A BITMAPINFOHEADER structure that contains information about the dimensions of color format.
			/// </summary>
			public BITMAPINFOHEADER bmiHeader;

			/// <summary>
			/// An array of RGBQUAD. The elements of the array that make up the color table.
			/// </summary>
			[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct)]
			public RGBQUAD[] bmiColors;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct RGBQUAD {
			public byte rgbBlue;
			public byte rgbGreen;
			public byte rgbRed;
			public byte rgbReserved;
		}

		internal class BitmapHelper {
			public const uint BI_BITFIELDS = 3;
			public const uint LCS_WINDOWS_COLOR_SPACE = 2;
			public const uint LCS_GM_IMAGES = 4;
			public const uint CF_DIBV5 = 17;
			public const uint GMEM_MOVEABLE = 0x00000002;
			public const uint GMEM_ZEROINIT = 0x00000040;
			public const uint GMEM_DDESHARE = 0x00002000;
			public const uint GHND = GMEM_MOVEABLE | GMEM_ZEROINIT;

			[DllImport("kernel32.dll")]
			static extern IntPtr GlobalAlloc(uint uFlags, uint dwBytes);

			[DllImport("kernel32.dll")]
			static extern IntPtr GlobalLock(IntPtr hMem);

			[DllImport("kernel32.dll")]
			static extern bool GlobalUnlock(IntPtr hMem);

			const int CBM_INIT = 0x04;//   /* initialize bitmap */

			/*public static IntPtr Create32BppBitmap(Image sourceImage) {
				BITMAPV5HEADER bi = new BITMAPV5HEADER();
				bi.bV5Size = (uint) Marshal.SizeOf(bi);
				bi.bV5Width = sourceImage.Width;
				bi.bV5Height = sourceImage.Height;
				bi.bV5Planes = 1;
				bi.bV5BitCount = 32;
				bi.bV5Compression = BI_BITFIELDS;
				// The following mask specification specifies a supported 32 BPP
				// alpha format for Windows XP.
				bi.bV5RedMask = 0x00FF0000;
				bi.bV5GreenMask = 0x0000FF00;
				bi.bV5BlueMask = 0x000000FF;
				bi.bV5AlphaMask = 0xFF000000;

				IntPtr hdc = User32.GetDC(IntPtr.Zero);
				IntPtr bits = IntPtr.Zero;

				// Create the DIB section with an alpha channel.
				IntPtr hBitmap = Gdi32.CreateDIBSection(hdc, bi, (uint) DIB.DIB_RGB_COLORS,
														out bits, IntPtr.Zero, 0);

				var hMemDC = Gdi32.CreateCompatibleDC(hdc);
				Gdi32.ReleaseDC(IntPtr.Zero, hdc);

				var sourceBits = ((Bitmap) sourceImage).LockBits(
					new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly,
					PixelFormat.Format32bppArgb);

				var stride = sourceImage.Width*4;
				for (int y = 0; y < sourceImage.Height; y++) {
					IntPtr DstDib = (IntPtr) (bits.ToInt32() + (y * stride));
					IntPtr SrcDib = (IntPtr) (sourceBits.Scan0.ToInt32() + ((sourceImage.Height - 1 - y) *
																		   stride));

					for (int x = 0; x < sourceImage.Width; x++) {
						Marshal.WriteInt32(DstDib, Marshal.ReadInt32(SrcDib));
						DstDib = (IntPtr) (DstDib.ToInt32() + 4);
						SrcDib = (IntPtr) (SrcDib.ToInt32() + 4);
					}
				}

				return hBitmap;
			}*/

			public static byte[] CreatePackedDIBV5(Bitmap image) {
				byte[] bmData;
				using (Bitmap bm32b = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppPArgb)) {
					using (Graphics g = Graphics.FromImage(bm32b))
						g.DrawImage(image, new Rectangle(0, 0, bm32b.Width, bm32b.Height));
					// Bitmap format has its lines reversed.
					bm32b.RotateFlip(RotateFlipType.Rotate180FlipX);
					bmData = ImageUtils.GetImageData(bm32b, out _);
				}
				uint bufferLen = (uint) (Marshal.SizeOf(typeof(BITMAPV5HEADER)) +
										 /*(Marshal.SizeOf(typeof(uint))*3) +*/ image.Height*image.Width*4);
				//IntPtr hMem = GlobalAlloc(GHND | GMEM_DDESHARE, bufferLen);
				//IntPtr packedDIBV5 = GlobalLock(hMem);
				BITMAPV5HEADER bmi = new BITMAPV5HEADER();// (BITMAPV5HEADER) Marshal.PtrToStructure(packedDIBV5,
															//				 typeof(BITMAPV5HEADER));
				bmi.bV5Size = (uint) Marshal.SizeOf(typeof(BITMAPV5HEADER));
				bmi.bV5Width = image.Width;
				bmi.bV5Height = image.Height;
				bmi.bV5BitCount = 32;
				bmi.bV5Planes = 1;
				bmi.bV5Compression = 0;// BI_BITFIELDS;
				bmi.bV5XPelsPerMeter = 0;
				bmi.bV5YPelsPerMeter = 0;
				bmi.bV5ClrUsed = 0;
				bmi.bV5ClrImportant = 0;
				bmi.bV5RedMask		= 0x000000FF;
				bmi.bV5GreenMask	= 0x0000FF00;
				bmi.bV5BlueMask		= 0x00FF0000;
				bmi.bV5AlphaMask	= 0xFF000000;
				bmi.bV5CSType = 0x73524742;// LCS_WINDOWS_COLOR_SPACE;
				bmi.bV5GammaBlue = 0;
				bmi.bV5GammaGreen = 0;
				bmi.bV5GammaRed = 0;
				bmi.bV5ProfileData = 0;
				bmi.bV5ProfileSize = 0;
				bmi.bV5Reserved = 0;
				bmi.bV5Intent = 0;// LCS_GM_IMAGES;
				bmi.bV5SizeImage = (uint) (image.Height*image.Width*4);
				bmi.bV5Endpoints.ciexyzBlue.ciexyzX =
					bmi.bV5Endpoints.ciexyzBlue.ciexyzY =
					bmi.bV5Endpoints.ciexyzBlue.ciexyzZ = 0;
				bmi.bV5Endpoints.ciexyzGreen.ciexyzX =
					bmi.bV5Endpoints.ciexyzGreen.ciexyzY =
					bmi.bV5Endpoints.ciexyzGreen.ciexyzZ = 0;
				bmi.bV5Endpoints.ciexyzRed.ciexyzX =
					bmi.bV5Endpoints.ciexyzRed.ciexyzY =
					bmi.bV5Endpoints.ciexyzRed.ciexyzZ = 0;
				//Marshal.StructureToPtr(bmi, packedDIBV5, false);

				BITFIELDS Masks = new BITFIELDS();// (BITFIELDS) Marshal.PtrToStructure(
					//(IntPtr) (packedDIBV5.ToInt32() + bmi.bV5Size), typeof(BITFIELDS));
				Masks.BlueMask = 0x000000FF;
				Masks.GreenMask = 0x0000FF00;
				Masks.RedMask = 0x00FF0000;
				//Marshal.StructureToPtr(Masks, (IntPtr) (packedDIBV5.ToInt32() +
				//										bmi.bV5Size), false);

				//byte[] dibData = new byte[bufferLen];
				byte[] imageData = new byte[bmi.bV5SizeImage];

				using (MemoryStream ms = new MemoryStream()) {
					var writer = new BinaryWriter(ms);
					writer.WriteStruct(bmi);
					//writer.WriteStruct(Masks);
					/*Marshal.Copy(bmData.Scan0, imageData, 0, imageData.Length);
					writer.Write(imageData);*/
					writer.Write(bmData);
					return ms.ToArray();
				}

				/*long offsetBits = bmi.bV5Size + Marshal.SizeOf(typeof(uint))*3;
				IntPtr bits = (IntPtr) (packedDIBV5.ToInt32() + offsetBits);

				for (int y = 0; y < bmData.Height; y++) {
					IntPtr DstDib = (IntPtr) (bits.ToInt32() + (y*bmData.Stride));
					IntPtr SrcDib = (IntPtr) (bmData.Scan0.ToInt32() + ((bmData.Height - 1 - y)*
																		bmData.Stride));

					for (int x = 0; x < bmData.Width; x++) {
						Marshal.WriteInt32(DstDib, Marshal.ReadInt32(SrcDib));
						DstDib = (IntPtr) (DstDib.ToInt32() + 4);
						SrcDib = (IntPtr) (SrcDib.ToInt32() + 4);
					}
				}

				// Create the DIB section with an alpha channel.
				IntPtr hdc = User32.GetDC(IntPtr.Zero);
				//IntPtr hdc = Gdi32.CreateCompatibleDC(IntPtr.Zero);
				GCHandle handle = GCHandle.Alloc(bmi, GCHandleType.Pinned);
				IntPtr hBitmap = Gdi32.CreateDIBitmap(hdc, handle.AddrOfPinnedObject(), CBM_INIT,
													  bits, handle.AddrOfPinnedObject(), (uint) DIB.DIB_RGB_COLORS);
				bm.UnlockBits(bmData);

				GlobalUnlock(hMem);


				return hBitmap;*/
			}
		}
	}
}
