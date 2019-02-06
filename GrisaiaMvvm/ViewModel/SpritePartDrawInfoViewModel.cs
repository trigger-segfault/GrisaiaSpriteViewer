using System.Windows;
using System.Windows.Media.Imaging;

namespace Grisaia.Mvvm.ViewModel {
	/// <summary>
	///  An immutable view model for the draw settings of a sprite part.
	/// </summary>
	public sealed class SpritePartDrawInfoViewModel {
		#region Constants

		/// <summary>
		///  Represents no sprite part selection.
		/// </summary>
		public static SpritePartDrawInfoViewModel None { get; } = new SpritePartDrawInfoViewModel();

		#endregion

		#region Properties

		/// <summary>
		///  Gets the image source for the sprite part.
		/// </summary>
		public BitmapImage Source { get; }
		/// <summary>
		///  Gets the margins for the sprite part.
		/// </summary>
		public Thickness Margin { get; }
		/// <summary>
		///  Gets the width of the sprite part.
		/// </summary>
		public double Width { get; }
		/// <summary>
		///  Gets the height of the sprite part.
		/// </summary>
		public double Height { get; }
		/// <summary>
		///  Gets if the sprite part is no selection.
		/// </summary>
		public bool IsNone => Source == null;

		#endregion

		#region Constructors

		private SpritePartDrawInfoViewModel() { }

		public SpritePartDrawInfoViewModel(BitmapImage source,
										   Thickness margin,
										   double width,
										   double height)
		{
			Source = source;
			Margin = margin;
			Width = width;
			Height = height;
		}

		#endregion
	}
}
