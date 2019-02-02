﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Grisaia.Mvvm.Converters {
	public class InverseBoolean : MarkupExtension, IValueConverter {
		public static readonly InverseBoolean Instance = new InverseBoolean();

		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return !(bool) value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return !(bool) value;
		}
	}
}
