using System;
using System.Drawing;
using System.Windows;
using Grisaia.Mvvm.Services;
using Grisaia.SpriteViewer.Utils;

namespace Grisaia.SpriteViewer.Services {
	/// <summary>
	///  The service for managing the clipboard state.
	/// </summary>
	public class ClipboardService : IClipboardService {
		#region Text

		/// <summary>
		///  Gets the text assigned to the clipboard.
		/// </summary>
		/// <returns>A string if the clipboard has text assigned to it.</returns>
		public string GetText() => Clipboard.GetText();
		/// <summary>
		///  Assigns text to the clipboard.
		/// </summary>
		/// <param name="text">The new text to assign to the clipboard.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="text"/> is null.
		/// </exception>
		public void SetText(string text) => Clipboard.SetText(text);

		#endregion

		#region Image

		/// <summary>
		///  Copies the given image to the clipboard as PNG, DIB and standard Bitmap format.
		/// </summary>
		/// <param name="image">Image to put on the clipboard.</param>
		/// <param name="imageNoTr">Optional specifically nontransparent version of the image to put on the clipboard.</param>
		/// <param name="tempFile">The path to a temporary file that can be pasted.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="image"/> is null.
		/// </exception>
		public void SetImage(Bitmap image, Bitmap imageNoTr, string tempFile) {
			ImageClipboard.SetClipboardImage(image, imageNoTr, tempFile);
		}

		#endregion
	}
}
