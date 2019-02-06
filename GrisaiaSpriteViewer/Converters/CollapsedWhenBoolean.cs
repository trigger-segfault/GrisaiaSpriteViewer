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
	public class CollapsedWhenFalse : MarkupExtension, IValueConverter {
		public static readonly CollapsedWhenFalse Instance = new CollapsedWhenFalse();

		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!((bool?) value).HasValue)
				return Binding.DoNothing;
			return (bool) value ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}

	public class CollapsedWhenTrue : MarkupExtension, IValueConverter {
		public static readonly CollapsedWhenTrue Instance = new CollapsedWhenTrue();

		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!((bool?) value).HasValue)
				return Binding.DoNothing;
			return (bool) value ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
