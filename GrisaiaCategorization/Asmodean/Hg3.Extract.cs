using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Grisaia.Extensions;

namespace Grisaia.Asmodean {
	partial class Hg3 {
		public static Hg3 Extract(string hg3File, string directory, bool saveFrames, bool expand) {
			using (var stream = File.OpenRead(hg3File))
				return Extract(stream, directory, hg3File, saveFrames, expand);
		}

		public static Hg3 Extract(Stream stream, string directory, string fileName, bool saveFrames, bool expand) {
			BinaryReader reader = new BinaryReader(stream);
			HG3HDR hdr = reader.ReadStruct<HG3HDR>();

			if (hdr.Signature != "HG-3")
				throw new UnexpectedFileTypeException(fileName, "HG-3");
			
			int backtrack = Marshal.SizeOf<HG3TAG>() - 1;
			List<KeyValuePair<HG3STDINFO, List<long>>> imageOffsets = new List<KeyValuePair<HG3STDINFO, List<long>>>();
			
			for (int i = 0; true; i++) {
				HG3TAG tag = reader.ReadStruct<HG3TAG>();

				// NEW METHOD: Keep searching for the next stdinfo
				// This way we don't miss any images
				while (!tag.Signature.StartsWith("stdinfo")) {
					if (stream.IsEndOfStream())
						break;
					stream.Position -= backtrack;
					tag = reader.ReadStruct<HG3TAG>();
				}
				if (stream.IsEndOfStream())
					break;

				// OLD METHOD: Missed entries in a few files
				//if (!tag.signature.StartsWith(StdInfoSignature))
				//	break;

				HG3STDINFO stdInfo = reader.ReadStruct<HG3STDINFO>();

				List<long> frameOffsets = new List<long>();
				imageOffsets.Add(new KeyValuePair<HG3STDINFO, List<long>>(stdInfo, frameOffsets));
				
				while (tag.OffsetNext != 0) {
					tag = reader.ReadStruct<HG3TAG>();
					if (Regex.IsMatch(tag.Signature, @"img\d+")) {
						if (saveFrames)
							frameOffsets.Add(stream.Position);
						// Skip this tag
						stream.Position += tag.Length;
					}
					/*else if (tag.Signature.StartsWith("img_al")) {
						// Skip this tag
						stream.Position += tag.Length;
					}
					else if (tag.Signature.StartsWith("img_jpg")) {
						// Skip this tag
						stream.Position += tag.Length;
					}
					else if (tag.Signature.StartsWith("imgmode")) {
						// Skip this tag
						stream.Position += tag.Length;
					}*/
					else {
						// Skip this tag
						stream.Position += tag.Length;
					}
				}

				stream.Position += 8;
			}

			// Save any frames after we've located them all.
			// This way we truely know if something is an animation.
			if (saveFrames) {
				bool isAnimation = (imageOffsets.Count != 1 || imageOffsets.First().Value.Count != 1);
				string pngFile = Path.Combine(directory, Path.ChangeExtension(fileName, ".png"));
				int imgIndex = 0;
				foreach (var pair in imageOffsets) {
					HG3STDINFO stdInfo = pair.Key;
					List<long> frameOffsets = pair.Value;
					int frmIndex = 0;
					foreach (long offset in frameOffsets) {
						stream.Position = offset;
						HG3IMG imghdr = reader.ReadStruct<HG3IMG>();
						if (isAnimation)
							pngFile = Path.Combine(directory, GetFrameFileName(fileName, imgIndex, frmIndex));
						ProcessImage(reader, stdInfo, imghdr, expand, pngFile);
						frmIndex++;
					}
					imgIndex++;
				}
			}

			HG3STDINFO[] stdInfos = imageOffsets.Select(p => p.Key).ToArray();
			return new Hg3(Path.GetFileName(fileName), stdInfos, saveFrames && expand);
		}

		private static void ProcessImage(BinaryReader reader, HG3STDINFO std, HG3IMG img, bool expand, string pngFile) {
			int depthBytes = (std.DepthBits + 7) / 8;
			int stride = (std.Width * depthBytes + 3) / 4 * 4;

			byte[] bufferTmp = reader.ReadBytes(img.DataLength);
			byte[] cmdBufferTmp = reader.ReadBytes(img.CmdLength);
			byte[] rgbaBuffer;

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
				handle.Free();
			}
		}
	}
}
