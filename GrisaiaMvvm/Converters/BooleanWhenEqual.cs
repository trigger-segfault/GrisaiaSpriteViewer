using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Grisaia.Mvvm.Converters {
	public class TrueWhenEqual : MarkupExtension, IValueConverter {
		public static readonly TrueWhenEqual Instance = new TrueWhenEqual();
		
		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value.Equals(System.Convert.ChangeType(parameter, value.GetType()));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	public class FalseWhenEqual : MarkupExtension, IValueConverter {
		public static readonly FalseWhenEqual Instance = new FalseWhenEqual();
		
		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return !value.Equals(System.Convert.ChangeType(parameter, value.GetType()));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
