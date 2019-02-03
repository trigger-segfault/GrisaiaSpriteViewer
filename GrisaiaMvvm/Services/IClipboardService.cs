using System;
using System.Drawing;

namespace Grisaia.Mvvm.Services {
	/// <summary>
	///  The service for managing the clipboard state.
	/// </summary>
	public interface IClipboardService {
		#region Text

		/// <summary>
		///  Gets the text assigned to the clipboard.
		/// </summary>
		/// <returns>A string if the clipboard has text assigned to it.</returns>
		string GetText();
		/// <summary>
		///  Assigns text to the clipboard.
		/// </summary>
		/// <param name="text">The new text to assign to the clipboard.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="text"/> is null.
		/// </exception>
		void SetText(string text);

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
		void SetImage(Bitmap image, Bitmap imageNoTr, string tempFile);

		#endregion
	}
}
