using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Grisaia.Mvvm.Extensions;

namespace Grisaia.Mvvm.Controls {
	public class ImageButton : Button {
		#region Dependency Properties

		/// <summary>
		///  The property for the button's image.
		/// </summary>
		public static readonly DependencyProperty IconSourceProperty =
			DependencyProperty.RegisterAttached("IconSource", typeof(ImageSource), typeof(ImageButton),
				new FrameworkPropertyMetadata(OnSourceChanged));

		/// <summary>
		///  Gets or sets the source of the button's image.
		/// </summary>
		[Category("Common")]
		public ImageSource IconSource {
			get => (ImageSource) GetValue(IconSourceProperty);
			set => SetValue(IconSourceProperty, value);
		}

		/// <summary>
		///  Called when the source property for the button is changed.
		/// </summary>
		private static void OnSourceChanged(object sender, DependencyPropertyChangedEventArgs e) {
			ImageButton button = (ImageButton) sender;
			button.image.Source = button.IconSource;

			button.CoerceValue(ContentProperty);
		}

		private static object CoerceContent(DependencyObject d, object value) {
			ImageButton button = (ImageButton) d;

			if (button.IsValueUnsetAndNull(ContentProperty, value)) {
				return button.image;
			}
			return value;
		}

		#endregion

		#region Fields

		/// <summary>The image that contains the buttons's icon.</summary>
		private Image image;

		#endregion

		#region Static Constructor

		/// <summary>
		///  Initializes the image buttons default style.
		/// </summary>
		static ImageButton() {
			ContentProperty.OverrideMetadata(typeof(ImageButton),
				new FrameworkPropertyMetadata(null, CoerceContent));
		}

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs an empty buttons.
		/// </summary>
		public ImageButton() {
			image = new Image() {
				Stretch = Stretch.None,
			};
		}

		#endregion
	}
}
