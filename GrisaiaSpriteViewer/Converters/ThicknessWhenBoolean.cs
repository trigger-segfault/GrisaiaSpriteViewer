using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Grisaia.SpriteViewer.Converters {
	public sealed class ThicknessWhenTrue : MarkupExtension, IValueConverter {
		public static readonly ThicknessWhenTrue Instance = new ThicknessWhenTrue();

		private static readonly ThicknessConverter converter = new ThicknessConverter();
		
		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!((bool?) value).HasValue)
				return Binding.DoNothing;
			if (!(bool) value)
				return new Thickness();
			return converter.ConvertFrom(parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	public sealed class ThicknessWhenFalse : MarkupExtension, IValueConverter {
		public static readonly ThicknessWhenFalse Instance = new ThicknessWhenFalse();

		private static readonly ThicknessConverter converter = new ThicknessConverter();

		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!((bool?) value).HasValue)
				return Binding.DoNothing;
			if ((bool) value)
				return new Thickness();
			return converter.ConvertFrom(parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
