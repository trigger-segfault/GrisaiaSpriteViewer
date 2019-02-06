using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Grisaia.SpriteViewer.Converters {
	public sealed class ThicknessWhenNonNull : MarkupExtension, IValueConverter {
		public static readonly ThicknessWhenNonNull Instance = new ThicknessWhenNonNull();

		private static readonly ThicknessConverter converter = new ThicknessConverter();
		
		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return new Thickness();
			return converter.ConvertFrom(parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	public sealed class ThicknessWhenNull : MarkupExtension, IValueConverter {
		public static readonly ThicknessWhenNull Instance = new ThicknessWhenNull();

		private static readonly ThicknessConverter converter = new ThicknessConverter();

		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value != null)
				return new Thickness();
			return converter.ConvertFrom(parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
