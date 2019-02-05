using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Grisaia.SpriteViewer.Converters {
	public class EnumToName : MarkupExtension, IValueConverter {
		public static readonly EnumToName Instance = new EnumToName();
		
		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			FieldInfo field = value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(f => f.GetValue(null).Equals(value));
			return field?.GetCustomAttribute<NameAttribute>()?.Name ?? value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	public class EnumToDescription : MarkupExtension, IValueConverter {
		public static readonly EnumToDescription Instance = new EnumToDescription();

		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			FieldInfo field = value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(f => f.GetValue(null).Equals(value));
			return field?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
