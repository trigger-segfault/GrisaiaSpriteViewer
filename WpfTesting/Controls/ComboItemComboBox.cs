using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfTesting.Model;

namespace WpfTesting.Controls {
	public class ComboItemComboBox : ComboBox {


		static ComboItemComboBox() {
			SelectedItemProperty.AddOwner(typeof(ComboItemComboBox),
				new FrameworkPropertyMetadata(null, OnCoerceSelectedItem));
			ItemsSourceProperty.AddOwner(typeof(ComboItemComboBox),
				new FrameworkPropertyMetadata(OnItemsSourceChanged));
		}

		private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ComboItemComboBox control = (ComboItemComboBox) d;
			if (e.NewValue != null) {
				d.CoerceValue(SelectedItemProperty);
			}
		}
		private static object OnCoerceSelectedItem(DependencyObject d, object baseValue) {
			ComboItemComboBox control = (ComboItemComboBox) d;
			ComboItem item = (ComboItem) baseValue;
			if (item == null) {
				if (control.ItemsSource == null)
					return null;
				else
					return ((IEnumerable<ComboItem>) control.ItemsSource).FirstOrDefault();
			}
			else {
				return baseValue;
			}
		}
	}
}
