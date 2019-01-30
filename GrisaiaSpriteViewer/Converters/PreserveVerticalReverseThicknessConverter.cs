using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Grisaia.SpriteViewer.Converters {
	public sealed class PreserveVerticalReverseThicknessConverter : MarkupExtension, IValueConverter {
		public static PreserveVerticalReverseThicknessConverter Instance = new PreserveVerticalReverseThicknessConverter();

		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is Thickness padding) {
				int[] parts = new int[1] { 0 };
				if (parameter is string topBottomPaddingStr) {
					parts = topBottomPaddingStr.Split(',').Select(p => int.Parse(p)).ToArray();
					if (parts.Length < 1 || parts.Length > 2)
						throw new ArgumentException("Parameter must be one or two integer values!");
				}
				return new Thickness {
					Left = -padding.Left,
					Right = -padding.Right,
					Top = parts[0],
					Bottom = (parts.Length == 1 ? parts[0] : parts[1]),
				};
			}
			throw new ArgumentException("Value to convert must be padding!");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
