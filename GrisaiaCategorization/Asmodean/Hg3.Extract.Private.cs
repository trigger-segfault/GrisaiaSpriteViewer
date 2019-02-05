using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Grisaia.Extensions;

namespace Grisaia.Asmodean {
	partial class Hg3 {
		#region Extract
		
		private static Hg3 Extract(Stream stream, string fileName, string directory, bool saveFrames, bool expand) {
			BinaryReader reader = new BinaryReader(stream);
			HG3HDR hdr = reader.ReadStruct<HG3HDR>();
			
			if (hdr.Signature != "HG-3")
				throw new UnexpectedFileTypeException(fileName, "HG-3");

			//int backtrack = Marshal.SizeOf<HG3TAG>() - 1;
			List<KeyValuePair<HG3STDINFO, List<long>>> imageOffsets = new List<KeyValuePair<HG3STDINFO, List<long>>>();
			
			for (int i = 0; ; i++) {

				// NEW-NEW METHOD: We now know the next offset ahead
				// of time from the HG3OFFSET we're going to read.
				// Usually skips 0 bytes, otherwise usually 1-7 bytes.
				long startPosition = stream.Position;
				HG3OFFSET offset = reader.ReadStruct<HG3OFFSET>();
				
				HG3TAG tag = reader.ReadStruct<HG3TAG>();
				if (!HG3STDINFO.HasTagSignature(tag.Signature))
					throw new Exception("Expected \"stdinfo\" tag!");
				
				// NEW METHOD: Keep searching for the next stdinfo
				// This way we don't miss any images
				/*int offset = 0;
				while (!tag.Signature.StartsWith("stdinfo")) {
					if (stream.IsEndOfStream())
						break;
					stream.Position -= backtrack;
					tag = reader.ReadStruct<HG3TAG>();
					offset++;
				}
				if (stream.IsEndOfStream())
					break;*/

				// OLD METHOD: Missed entries in a few files
				//if (!tag.signature.StartsWith(StdInfoSignature))
				//	break;

				HG3STDINFO stdInfo = reader.ReadStruct<HG3STDINFO>();

				List<long> frameOffsets = new List<long>();
				imageOffsets.Add(new KeyValuePair<HG3STDINFO, List<long>>(stdInfo, frameOffsets));

				while (tag.OffsetNext != 0) {
					tag = reader.ReadStruct<HG3TAG>();
					
					string signature = tag.Signature;
					if (HG3IMG.HasTagSignature(signature)) { // "img####"
						frameOffsets.Add(stream.Position);
						// Skip this tag
						stream.Position += tag.Length;
					}
					/*else if (HG3ATS.HasTagSignature(signature)) { // "ats####"
						// Skip this tag
						stream.Position += tag.Length;
					}
					else if (HG3CPTYPE.HasTagSignature(signature)) { // "cptype"
						// Skip this tag
						stream.Position += tag.Length;
					}
					else if (HG3IMG_AL.HasTagSignature(signature)) { // "img_al"
						// Skip this tag
						stream.Position += tag.Length;
					}
					else if (HG3IMG_JPG.HasTagSignature(signature)) { // "img_jpg"
						// Skip this tag
						stream.Position += tag.Length;
					}
					else if (HG3IMGMODE.HasTagSignature(signature)) { // "imgmode"
						// Skip this tag
						stream.Position += tag.Length;
					}*/
					else {
						// Skip this unknown tag
						stream.Position += tag.Length;
					}
				}

				if (offset.OffsetNext == 0)
					break; // End of stream

				stream.Position = startPosition + offset.OffsetNext;
			}

			HG3STDINFO[] stdInfos = imageOffsets.Select(p => p.Key).ToArray();
			long[][] allFrameOffsets = imageOffsets.Select(p => p.Value.ToArray()).ToArray();
			Hg3 hg3 = new Hg3(Path.GetFileName(fileName), hdr, stdInfos, allFrameOffsets, saveFrames && expand);
			// Save any frames after we've located them all.
			// This way we truely know if something is an animation.
			if (saveFrames) {
				for (int imgIndex = 0; imgIndex < hg3.Count; imgIndex++) {
					HG3STDINFO stdInfo = stdInfos[imgIndex];
					Hg3Image hg3Image = hg3.Images[imgIndex];
					for (int frmIndex = 0; frmIndex < hg3Image.FrameCount; frmIndex++) {
						stream.Position = hg3Image.FrameOffsets[frmIndex];
						HG3IMG imghdr = reader.ReadStruct<HG3IMG>();
						string pngFile = hg3.GetFrameFilePath(directory, imgIndex, frmIndex);
						ExtractBitmap(reader, stdInfo, imghdr, expand, pngFile);
					}
				}
			}
			
			return hg3;
		}

		#endregion

		#region ExtractBitmap

		/// <summary>
		///  Extracts the bitmap from the HG-3 file.
		/// </summary>
		/// <param name="reader">The binary reader for the file.</param>
		/// <param name="std">The HG3STDINFO containing image dimensions, etc.</param>
		/// <param name="img">The image header used to process the image.</param>
		/// <param name="expand">True if the image should be expanded to its full size.</param>
		/// <param name="pngFile">The path to the PNG file to save to.</param>
		private static void ExtractBitmap(BinaryReader reader, HG3STDINFO std, HG3IMG img, bool expand, string pngFile) {
			int depthBytes = (std.DepthBits + 7) / 8;
			int stride = (std.Width * depthBytes + 3) / 4 * 4;

			byte[] bufferTmp = reader.ReadBytes(img.DataLength);
			byte[] cmdBufferTmp = reader.ReadBytes(img.CmdLength);
			byte[] rgbaBuffer;

			// Perform heavy processing that's faster in native code
			ProcessImageNative(
				bufferTmp,
				img.DataLength,
				img.OriginalDataLength,
				cmdBufferTmp,
				img.CmdLength,
				img.OriginalCmdLength,
				out IntPtr pRgbaBuffer,
				out int rgbaLength,
				std.Width,
				std.Height,
				depthBytes);

			try {
				// Vertically flip the buffer so its in the correct setup to load into Bitmap
				rgbaBuffer = new byte[rgbaLength];
				for (int y = 0; y < std.Height; y++) {
					int src = y * stride;
					int dst = (std.Height - (y + 1)) * stride;
					Marshal.Copy(pRgbaBuffer + src, rgbaBuffer, dst, stride);
				}
			} finally {
				Marshal.FreeHGlobal(pRgbaBuffer);
			}

			WriteBitmap(rgbaBuffer, std, expand, pngFile);
		}
		/// <summary>
		///  Writes the bitmap buffer to <paramref name="pngFile"/> and optional performs expansion if
		///  <paramref name="expand"/> is true.
		/// </summary>
		/// <param name="buffer">The buffer to the image bits.</param>
		/// <param name="std">The HG3STDINFO containing image dimensions, etc.</param>
		/// <param name="expand">True if the image should be expanded to its full size.</param>
		/// <param name="pngFile">The path to the PNG file to save to.</param>
		private static void WriteBitmap(byte[] buffer, HG3STDINFO std, bool expand, string pngFile) {
			GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			try {
				IntPtr scan0 = handle.AddrOfPinnedObject();
				int depthBytes = (std.DepthBits + 7) / 8;
				int stride = (std.Width * depthBytes + 3) / 4 * 4;
				PixelFormat format;
				switch (std.DepthBits) {
				case 32: format = PixelFormat.Format32bppArgb; break;
				case 24: format = PixelFormat.Format24bppRgb; break;
				default: throw new Exception($"Unsupported depth bits {std.DepthBits}!");
				}
				// Do expansion here, and up to 32 bits if not 32 bits already.
				if (expand) {
					using (var bitmap = new Bitmap(std.Width, std.Height, stride, format, scan0))
					using (var bitmap32 = new Bitmap(std.TotalWidth, std.TotalHeight, PixelFormat.Format32bppArgb))
					using (Graphics g = Graphics.FromImage(bitmap32)) {
						g.DrawImageUnscaled(bitmap, std.OffsetX, std.OffsetY);
						bitmap32.Save(pngFile, ImageFormat.Png);
					}
				}
				else {
					using (var bitmap = new Bitmap(std.Width, std.Height, stride, format, scan0))
						bitmap.Save(pngFile, ImageFormat.Png);
				}

			} finally {
				// Thing to note that gave me headaches earlier:
				// Once this handle is freed, the bitmap loaded from
				// scan0 will be invalidated after garbage collection.
				handle.Free();
			}
		}

		#endregion
	}
}
