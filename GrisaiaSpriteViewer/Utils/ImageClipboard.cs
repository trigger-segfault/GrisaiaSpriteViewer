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

namespace Grisaia.SpriteViewer.Utils {
	/// <summary>
	///  https://stackoverflow.com/a/46424800/7517185
	/// </summary>
	public static class ImageClipboard {
		private static class DataFormatsEx {
			public static readonly string DibV5 = "Format17";
			public static readonly string Png = "PNG";
		}

		/// <summary>
		///  Copies the given image to the clipboard as PNG, DIB and standard Bitmap format.
		/// </summary>
		/// <param name="image">Image to put on the clipboard.</param>
		/// <param name="imageNoTr">Optional specifically nontransparent version of the image to put on the clipboard.</param>
		/// <param name="data">Clipboard data object to put the image into. Might already contain other stuff. Leave null to create a new one.</param>
		public static void SetClipboardImage(Bitmap image, Bitmap imageNoTr, string tempFile, DataObject data = null) {
			Clipboard.Clear();
			if (data == null)
				data = new DataObject();
			if (imageNoTr == null)
				imageNoTr = image;
			using (MemoryStream pngMemStream = new MemoryStream())
			using (MemoryStream dibMemStream = new MemoryStream())
			using (MemoryStream dibV5MemStream = new MemoryStream()) {
				// As standard bitmap, without transparency support
				data.SetData(DataFormats.Bitmap, true, imageNoTr);
				// As PNG. Gimp will prefer this over Dib and Bitmap
				image.Save(pngMemStream, ImageFormat.Png);
				data.SetData(DataFormatsEx.Png, false, pngMemStream);
				
				// As DIB. This is (incorrectly) accepted as ARGB by many applications.
				byte[] dibImageData = GetDibImageData(image);
				WriteDibToStream(dibMemStream, image, dibImageData);
				data.SetData(DataFormats.Dib, false, dibMemStream);
				
				WriteDibV5ToStream(dibV5MemStream, image, dibImageData);
				data.SetData(DataFormatsEx.DibV5, false, dibV5MemStream);

				// FileDrop for additional support
				if (tempFile != null)
					data.SetFileDropList(new StringCollection { tempFile });

				// The 'copy=true' argument means the MemoryStreams can be safely disposed after the operation.
				Clipboard.SetDataObject(data, true);
			}
		}

		private static void WriteDibToStream(Stream stream, Bitmap image, byte[] imageData) {
			BITMAPINFOHEADER bmi = new BITMAPINFOHEADER {
				biSize = Marshal.SizeOf<BITMAPINFOHEADER>(),
				biWidth = image.Width,
				biHeight = image.Height,
				biPlanes = 1,
				biBitCount = 32,
				//biCompression = 3,
				biSizeImage = imageData.Length,
			};
			/*BITMAPINFOHEADER bmi = new BITMAPINFOHEADER();
			bmi.biSize = Marshal.SizeOf<BITMAPINFOHEADER>();
			bmi.biWidth = image.Width;
			bmi.biHeight = image.Height;
			bmi.biPlanes = 1;
			bmi.biBitCount = 32;
			//bmi.biCompression = 3;
			bmi.biSizeImage = imageData.Length;*/

			BinaryWriter writer = new BinaryWriter(stream);
			writer.WriteStruct(bmi);
			/*writer.Write(0x000001E0);
			writer.Write(0x000001E0);
			writer.Write(0x00FF0000);
			writer.Write(imageData.Skip(12).ToArray());*/
			writer.Write(imageData);
		}

		private static void WriteDibV5ToStream(Stream stream, Bitmap image, byte[] imageData) {
			BITMAPV5HEADER bmi = new BITMAPV5HEADER {
				bV5Size = Marshal.SizeOf<BITMAPV5HEADER>(),
				bV5Width = image.Width,
				bV5Height = image.Height,
				bV5Planes = 1,
				bV5BitCount = 32,
				//bV5Compression = 3,
				bV5SizeImage = imageData.Length,
				bV5RedMask   = 0x00FF0000,
				bV5GreenMask = 0x0000FF00,
				bV5BlueMask  = 0x000000FF,
				bV5AlphaMask = 0xFF000000,
				bV5CSType = 0x73524742,// LCS_WINDOWS_COLOR_SPACE;
				//bV5CSType = 0x206E6957,// GLITCHED
				bV5Intent = 4,
			};
			/*BITMAPV5HEADER bmi = new BITMAPV5HEADER();
			bmi.bV5Size = Marshal.SizeOf<BITMAPV5HEADER>();
			bmi.bV5Width = image.Width;
			bmi.bV5Height = image.Height;
			bmi.bV5Planes = 1;
			bmi.bV5BitCount = 32;
			//bmi.bV5Compression = 3;
			bmi.bV5SizeImage = imageData.Length;
			bmi.bV5RedMask   = 0x00FF0000;
			bmi.bV5GreenMask = 0x0000FF00;
			bmi.bV5BlueMask  = 0x000000FF;
			bmi.bV5AlphaMask = 0xFF000000;
			bmi.bV5CSType = 0x73524742;// LCS_WINDOWS_COLOR_SPACE;
			//bmi.bV5CSType = 0x206E6957;// GLITCHED
			bmi.bV5Intent = 4;*/

			BinaryWriter writer = new BinaryWriter(stream);
			writer.WriteStruct(bmi);
			/*writer.Write(0x000001E0);
			writer.Write(0x000001E0);
			writer.Write(0x00FF0000);*/
			writer.Write(imageData);
		}

		private static byte[] GetDibImageData(Bitmap image) {
			// PixelFormat.Format32bppPArgb:
			// Makes things work with Discord
			// Otherwise we get a black background
			using (Bitmap bmpPremult = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppPArgb)) {
				using (Graphics gr = Graphics.FromImage(bmpPremult))
					gr.DrawImage(image, new Rectangle(0, 0, bmpPremult.Width, bmpPremult.Height));
				// Bitmap format has its lines reversed.
				bmpPremult.RotateFlip(RotateFlipType.Rotate180FlipX);

				Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
				BitmapData bmpData = bmpPremult.LockBits(rect, ImageLockMode.ReadOnly, bmpPremult.PixelFormat);
				try {
					byte[] imageData = new byte[bmpData.Stride * image.Height];
					Marshal.Copy(bmpData.Scan0, imageData, 0, imageData.Length);
					return imageData;
				}
				finally {
					bmpPremult.UnlockBits(bmpData);
				}
			}
		}
		
		private enum BI : uint {
			BI_RGB = 0,
			BI_RLE8 = 1,
			BI_RLE4 = 2,
			BI_BITFIELDS = 3,
			BI_JPEG = 4,
			BI_PNG = 5
		}
		[StructLayout(LayoutKind.Sequential)]
		private struct BITMAPV5HEADER {
			public int bV5Size;
			public int bV5Width;
			public int bV5Height;
			public ushort bV5Planes;
			public ushort bV5BitCount;
			public uint bV5Compression;
			public int bV5SizeImage;
			public int bV5XPelsPerMeter;
			public int bV5YPelsPerMeter;
			public uint bV5ClrUsed;
			public uint bV5ClrImportant;
			public uint bV5RedMask;
			public uint bV5GreenMask;
			public uint bV5BlueMask;
			public uint bV5AlphaMask;
			public uint bV5CSType;
			public CIEXYZTRIPLE bV5Endpoints;
			public uint bV5GammaRed;
			public uint bV5GammaGreen;
			public uint bV5GammaBlue;
			public uint bV5Intent;
			public uint bV5ProfileData;
			public uint bV5ProfileSize;
			public uint bV5Reserved;
		}
		[StructLayout(LayoutKind.Sequential)]
		private struct CIEXYZ {
			public uint ciexyzX; //FXPT2DOT30
			public uint ciexyzY; //FXPT2DOT30
			public uint ciexyzZ; //FXPT2DOT30
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct CIEXYZTRIPLE {
			public CIEXYZ ciexyzRed;
			public CIEXYZ ciexyzGreen;
			public CIEXYZ ciexyzBlue;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct BITMAPINFOHEADER {
			public int biSize;
			public int biWidth;
			public int biHeight;
			public ushort biPlanes;
			public ushort biBitCount;
			public uint biCompression;
			public int biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public uint biClrUsed;
			public uint biClrImportant;
		}
		/*[StructLayout(LayoutKind.Sequential)]
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

			const int CBM_INIT = 0x04;// initialize bitmap
			
		}*/
	}
}
