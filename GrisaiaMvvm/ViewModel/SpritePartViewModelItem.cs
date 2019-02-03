using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Grisaia.Mvvm.ViewModel {
	/// <summary>
	///  An immutable view model for the draw settings of a sprite part.
	/// </summary>
	public class SpritePartViewModelItem {
		#region Constants

		/// <summary>
		///  Represents no sprite part selection.
		/// </summary>
		public static SpritePartViewModelItem None { get; } = new SpritePartViewModelItem();

		#endregion

		#region Properties

		/// <summary>
		///  Gets the image source for the sprite part.
		/// </summary>
		public ImageSource ImageSource { get; }
		/// <summary>
		///  Gets the margins for the sprite part.
		/// </summary>
		public Thickness Margin { get; }
		/// <summary>
		///  Gets the size of the sprite part.
		/// </summary>
		public Size Size { get; }
		/// <summary>
		///  Gets if the sprite part is no selection.
		/// </summary>
		public bool IsNone => ImageSource == null;

		#endregion

		#region Constructors

		public SpritePartViewModelItem() { }

		public SpritePartViewModelItem(ImageSource imageSource,
									   Thickness margin,
									   Size size)
		{
			ImageSource = imageSource;
			Margin = margin;
			Size = size;
		}

		#endregion
	}
}
