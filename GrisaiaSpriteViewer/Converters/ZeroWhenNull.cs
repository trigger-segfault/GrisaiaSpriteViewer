using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Grisaia.SpriteViewer.Converters {
	public class ZeroWhenNonNull : MarkupExtension, IValueConverter {
		public static readonly ZeroWhenNonNull Instance = new ZeroWhenNonNull();

		private static readonly GridLengthConverter converter = new GridLengthConverter();

		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(GridLength))
				return value == null ? converter.ConvertFrom(parameter) : new GridLength(0);
			else
				return value == null ? parameter : 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	public class ZeroWhenNull : MarkupExtension, IValueConverter {
		public static readonly ZeroWhenNull Instance = new ZeroWhenNull();

		private static readonly GridLengthConverter converter = new GridLengthConverter();

		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(GridLength))
				return value == null ? new GridLength(0) : converter.ConvertFrom(parameter);
			else
				return value == null ? 0 : parameter;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
