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
	public class ZeroWhenFalse : MarkupExtension, IValueConverter {
		public static readonly ZeroWhenFalse Instance = new ZeroWhenFalse();

		private static readonly GridLengthConverter converter = new GridLengthConverter();

		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(GridLength))
				return (bool) value ? converter.ConvertFrom(parameter) : new GridLength(0);
			else
				return (bool) value ? parameter : 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	public class ZeroWhenTrue : MarkupExtension, IValueConverter {
		public static readonly ZeroWhenTrue Instance = new ZeroWhenTrue();

		private static readonly GridLengthConverter converter = new GridLengthConverter();

		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(GridLength))
				return (bool) value ? new GridLength(0) : converter.ConvertFrom(parameter);
			else
				return (bool) value ? 0 : parameter;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
