using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WpfTesting.Model;

namespace WpfTesting.Controls {
	public class SpriteCategoryComboBox : Control {
		#region Fields

		//private TextBlock PART_TextBlock;
		//private ComboBox PART_ComboBox;
		//private Thumb PART_Thumb;
		private bool supressEvents;

		#endregion

		#region Dependency Properties

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register(
				"ItemsSource",
				typeof(ComboItem),
				typeof(SpriteCategoryComboBox),
				new FrameworkPropertyMetadata(
					null,
					OnItemsSourceChanged));
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register(
				"SelectedItem",
				typeof(ComboItem),
				typeof(SpriteCategoryComboBox),
				new FrameworkPropertyMetadata(
					null,
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					OnSelectedItemChanged,
					null));
		public static readonly DependencyProperty SelectedItemBindingProperty =
			DependencyProperty.Register(
				"SelectedItemBinding",
				typeof(ComboItem),
				typeof(SpriteCategoryComboBox),
				new FrameworkPropertyMetadata(
					null,
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					OnSelectedItemBindingChanged,
					null));

		private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteCategoryComboBox control = (SpriteCategoryComboBox) d;

			if (control.supressEvents) return;
			control.supressEvents = true;
			string propName = string.Empty;
			if (control.GetBindingExpression(ItemsSourceProperty) != null)
				propName = control.GetBindingExpression(ItemsSourceProperty).ResolvedSourcePropertyName;
			if (control.ItemsSource != null) {
				Console.WriteLine($"{propName} ItemsSource Changed: Coerce");
				control.SelectedItem = (ComboItem) OnCoerceSelectedItem(control, control.SelectedItem);
			}
			else {
				Console.WriteLine($"{propName} ItemsSource Changed: null");
				control.SelectedItem = null;
			}
			control.SelectedItemBinding = control.SelectedItem;
			control.supressEvents = false;
		}
		private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteCategoryComboBox control = (SpriteCategoryComboBox) d;

			if (control.supressEvents) return;
			control.supressEvents = true;
			string propName = string.Empty;
			if (control.GetBindingExpression(ItemsSourceProperty) != null)
				propName = control.GetBindingExpression(ItemsSourceProperty).ResolvedSourcePropertyName;
			if (control.ItemsSource != null) {
				Console.WriteLine($"{propName} SelectedItem Changed: Coerce");
				control.SelectedItemBinding = (ComboItem) OnCoerceSelectedItem(control, control.SelectedItemBinding);
			}
			else {
				Console.WriteLine($"{propName} SelectedItem Changed: null");
				control.SelectedItemBinding = null;
			}
			control.SelectedItemBinding = control.SelectedItem;
			control.supressEvents = false;
		}
		private static void OnSelectedItemBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteCategoryComboBox control = (SpriteCategoryComboBox) d;

			if (control.supressEvents) return;
			control.supressEvents = true;
			string propName = string.Empty;
			if (control.GetBindingExpression(ItemsSourceProperty) != null)
				propName = control.GetBindingExpression(ItemsSourceProperty).ResolvedSourcePropertyName;
			if (control.ItemsSource != null) {
				Console.WriteLine($"{propName} SelectedItemBinding Changed: Coerce");
				control.SelectedItem = (ComboItem) OnCoerceSelectedItem(control, control.SelectedItem);
				control.SelectedItem = control.SelectedItemBinding;
			}
			else {
				Console.WriteLine($"{propName} SelectedItemBinding Changed: null");
				control.SelectedItem = null;
			}
			control.SelectedItem = control.SelectedItemBinding;
			control.supressEvents = false;
		}
		private static object OnCoerceSelectedItem(DependencyObject d, object baseValue) {
			SpriteCategoryComboBox control = (SpriteCategoryComboBox) d;
			/*ComboItem item = (ComboItem) baseValue;
			if (item == null || !control.ItemsSource.Items.Contains(item)) {
				if (control.ItemsSource == null)
					return null;
				else
					return ((ComboItem) control.ItemsSource).Items.FirstOrDefault();
			}
			else {
				return baseValue;
			}*/
			string propName = string.Empty;
			if (control.GetBindingExpression(SelectedItemProperty) != null)
				propName = control.GetBindingExpression(SelectedItemProperty).ResolvedSourcePropertyName;
			ComboItem item = (ComboItem) baseValue;
			ComboItem source = control.ItemsSource;
			if (source == null) {
				Console.WriteLine($"{propName} Coerce: null");
				return null;
			}
			else if (item != null && source.Items.Contains(item)) {
				Console.WriteLine($"{propName} Coerce: Find");
				return source.Items.First(i => i.Id == item.Id);
			}
			Console.WriteLine($"{propName} Coerce: FirstOrDefault");
			return source.Items.FirstOrDefault();
		}

		public ComboItem ItemsSource {
			get => (ComboItem) GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}
		public ComboItem SelectedItem {
			get => (ComboItem) GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}
		public ComboItem SelectedItemBinding {
			get => (ComboItem) GetValue(SelectedItemBindingProperty);
			set => SetValue(SelectedItemBindingProperty, value);
		}

		#endregion

		#region Static Constructor

		static SpriteCategoryComboBox() {
			DefaultStyleKeyProperty.AddOwner(typeof(SpriteCategoryComboBox),
				new FrameworkPropertyMetadata(typeof(SpriteCategoryComboBox)));
			/*ItemsSourceProperty.AddOwner(
				typeof(SpriteCategoryComboBox),
				new FrameworkPropertyMetadata(
					null,
					OnItemsSourceChanged));
			SelectedItemProperty.AddOwner(
				typeof(SpriteCategoryComboBox),
				new FrameworkPropertyMetadata(
					null,
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					OnSelectedItemChanged,
					OnCoerceSelectedItem));*/
		}

		#endregion

		#region Constructors


		#endregion

		#region Control Overrides

		/*public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PART_ComboBox = GetTemplateChild("PART_ComboBox") as ComboBox;
			PART_TextBlock = GetTemplateChild("PART_TextBlock") as TextBlock;
		}*/

		#endregion
	}
}
