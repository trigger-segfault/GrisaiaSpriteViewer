﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Grisaia.Utils;

namespace Grisaia.Extensions {
	/// <summary>
	///  Extensions for the <see cref="Stream"/> class.
	/// </summary>
	public static class StreamExtensions {
		#region IsEndOfStream

		/// <summary>
		///  Determins if the stream pointer is at or passed the length of the stream. If this is a
		///  <see cref="NetworkStream"/>, then <see cref="NetworkStream.DataAvailable"/> will be called.
		/// </summary>
		/// <param name="stream">The stream to check for the end of.</param>
		/// <returns>True if the end of the stream has been reached.</returns>
		/// 
		/// <exception cref="NotSupportedException">
		///  The stream does not support seeking.
		/// </exception>
		/// <exception cref="IOException">
		///  An I/O error occurred. Or the underlying <see cref="Socket"/> is closed.
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		///  The stream is closed.
		/// </exception>
		/// <exception cref="SocketException">
		///  Use the <see cref="SocketException.ErrorCode"/> property to obtain the specific error code, and
		///  description refer to the Windows Sockets version 2 API error code documentation in MSDN for a detailed of
		///  the error.
		/// </exception>
		public static bool IsEndOfStream(this Stream stream) {
			if (stream is NetworkStream netStream)
				return !netStream.DataAvailable;
			return stream.Position >= stream.Length;
		}

		#endregion

		#region ReadToEnd

		/// <summary>
		///  Reads the remaining bytes in the stream and advances the current position.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <returns>A byte array with the remaining bytes.</returns>
		/// 
		/// <exception cref="NotSupportedException">
		///  The stream does not support reading.
		/// </exception>
		/// <exception cref="IOException">
		///  An I/O error occurred.
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		///  The stream is closed.
		/// </exception>
		public static byte[] ReadToEnd(this Stream stream) {
			using (MemoryStream memoryStream = new MemoryStream()) {
				stream.CopyTo(memoryStream);
				return memoryStream.ToArray();
			}
		}

		#endregion

		#region Skip

		/// <summary>
		///  Skips <paramref name="count"/> number of bytes in the stream.
		/// </summary>
		/// <param name="stream">The stream to skip through.</param>
		/// <param name="count">The number of bytes to skip.</param>
		/// <returns>The number of bytes, actually skipped.</returns>
		/// 
		/// <remarks>
		///  This method utilizes <see cref="Stream.Seek(long, SeekOrigin)"/> if it's <see cref="Stream.CanSeek"/> is
		///  true. Otherwise it reads bytes to a buffer of size 4096 until <paramref name="count"/> is reached.
		/// </remarks>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="stream"/> is null.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///  <paramref name="count"/> is negative.
		/// </exception>
		/// <exception cref="IOException">
		///  An I/O error occurred.
		/// </exception>
		/// <exception cref="NotSupportedException">
		///  The stream does not support seeking or reading.
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		///  The stream is closed.
		/// </exception>
		public static int Skip(this Stream stream, int count) {
			return stream.Skip(count, 4096);
		}
		/// <summary>
		///  Skips <paramref name="count"/> number of bytes in the stream.
		/// </summary>
		/// <param name="stream">The stream to skip through.</param>
		/// <param name="count">The number of bytes to skip.</param>
		/// <param name="bufferSize">
		/// The size of the buffer to read bytes into at a time when <see cref="Stream.CanSeek"/> is false.
		/// </param>
		/// <returns>The number of bytes, actually skipped.</returns>
		/// 
		/// <remarks>
		///  This method utilizes <see cref="Stream.Seek(long, SeekOrigin)"/> if it's <see cref="Stream.CanSeek"/> is
		///  true. Otherwise it reads bytes to a buffer of size <paramref name="bufferSize"/> until
		///  <paramref name="count"/> is reached.
		/// </remarks>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="stream"/> is null.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///  <paramref name="count"/> is less than zero or <paramref name="bufferSize"/> is less than 1.
		/// </exception>
		/// <exception cref="IOException">
		///  An I/O error occurred.
		/// </exception>
		/// <exception cref="NotSupportedException">
		///  The stream does not support seeking or reading.
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		///  The stream is closed.
		/// </exception>
		public static int Skip(this Stream stream, int count, int bufferSize) {
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));
			if (count < 0)
				throw ArgumentOutOfRangeUtils.OutsideMin(nameof(count), count, 0, true);
			if (bufferSize < 1)
				throw ArgumentOutOfRangeUtils.OutsideMin(nameof(bufferSize), bufferSize, 1, true);

			if (stream.CanSeek) {
				// Use a faster method of skipping if possible
				long oldPosition = stream.Position;
				return (int) (stream.Seek(count, SeekOrigin.Current) - oldPosition);
			}
			else {
				bufferSize = Math.Min(count, bufferSize);
				byte[] buffer = new byte[bufferSize];
				int totalRead = 0;
				while (totalRead < count) {
					int read = stream.Read(buffer, 0, Math.Min(count, bufferSize));
					if (read == 0)
						return totalRead;
					totalRead += read;
				}
				return totalRead;
			}
		}

		#endregion
	}
}
