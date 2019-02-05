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
	public class CollapsedWhenNonNull : MarkupExtension, IValueConverter {
		public static readonly CollapsedWhenNonNull Instance = new CollapsedWhenNonNull();

		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value == null ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}

	public class CollapsedWhenNull : MarkupExtension, IValueConverter {
		public static readonly CollapsedWhenNull Instance = new CollapsedWhenNull();

		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value == null ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
