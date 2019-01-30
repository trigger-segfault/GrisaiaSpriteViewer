using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Grisaia.Categories.Sprites;

namespace Grisaia.SpriteViewer.Controls {
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
				typeof(ISpriteCategory),
				typeof(SpriteCategoryComboBox),
				new FrameworkPropertyMetadata(
					OnItemsSourceChanged));
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register(
				"SelectedItem",
				typeof(ISpriteCategory),
				typeof(SpriteCategoryComboBox),
				new FrameworkPropertyMetadata(
					null,
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					OnSelectedItemChanged));
		public static readonly DependencyProperty SelectedItemInternalProperty =
			DependencyProperty.Register(
				"SelectedItemInternal",
				typeof(ISpriteCategory),
				typeof(SpriteCategoryComboBox),
				new FrameworkPropertyMetadata(
					null,
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					OnSelectedItemInternalChanged));

		private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteCategoryComboBox control = (SpriteCategoryComboBox) d;
			control.CoerceSelectedItem(control.SelectedItem);
		}
		private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteCategoryComboBox control = (SpriteCategoryComboBox) d;
			control.CoerceSelectedItem(control.SelectedItem);
		}
		private static void OnSelectedItemInternalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteCategoryComboBox control = (SpriteCategoryComboBox) d;
			control.CoerceSelectedItem(control.SelectedItemInternal);
		}
		private void CoerceSelectedItem(ISpriteCategory category, [CallerMemberName] string callerName = null) {
			if (supressEvents) return;
			string propName = "null";
			if (GetBindingExpression(ItemsSourceProperty) != null)
				propName = GetBindingExpression(ItemsSourceProperty).ResolvedSourcePropertyName;
			Console.WriteLine($"{propName} SpriteCategoryComboBox.{callerName} coerce");
			supressEvents = true;
			ISpriteCategory source = ItemsSource;
			if (source == null) {
				category = null;
			}
			else if (category != null && source.TryGetValue(category.Id, out ISpriteCategory newCategory)) {
				category = newCategory;
			}
			else {
				category = (ISpriteCategory) source.List.FirstOrDefault();
			}
			SelectedItem = category;
			SelectedItemInternal = category;
			supressEvents = false;
		}

		public ISpriteCategory ItemsSource {
			get => (ISpriteCategory) GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}
		public ISpriteCategory SelectedItem {
			get => (ISpriteCategory) GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}
		public ISpriteCategory SelectedItemInternal {
			get => (ISpriteCategory) GetValue(SelectedItemInternalProperty);
			set => SetValue(SelectedItemInternalProperty, value);
		}

		#endregion

		#region Static Constructor

		static SpriteCategoryComboBox() {
			DefaultStyleKeyProperty.AddOwner(typeof(SpriteCategoryComboBox),
				new FrameworkPropertyMetadata(typeof(SpriteCategoryComboBox)));
		}

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
