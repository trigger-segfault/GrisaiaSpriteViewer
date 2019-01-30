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
using Grisaia.Categories;
using Grisaia.Categories.Sprites;

namespace Grisaia.SpriteViewer.Controls {
	public class SpritePartGroupComboBox : Control {
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
				typeof(ISpritePartGroup),
				typeof(SpritePartGroupComboBox),
				new FrameworkPropertyMetadata(
					OnItemsSourceChanged));
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register(
				"SelectedItem",
				typeof(ISpritePartGroupPart),
				typeof(SpritePartGroupComboBox),
				new FrameworkPropertyMetadata(
					null,
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					OnSelectedItemChanged));
		public static readonly DependencyProperty SelectedItemInternalProperty =
			DependencyProperty.Register(
				"SelectedItemInternal",
				typeof(ISpritePartGroupPart),
				typeof(SpritePartGroupComboBox),
				new FrameworkPropertyMetadata(
					null,
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					OnSelectedItemInternalChanged));

		private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpritePartGroupComboBox control = (SpritePartGroupComboBox) d;
			control.CoerceSelectedItem(control.SelectedItem);
		}
		private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpritePartGroupComboBox control = (SpritePartGroupComboBox) d;
			control.CoerceSelectedItem(control.SelectedItem);
		}
		private static void OnSelectedItemInternalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpritePartGroupComboBox control = (SpritePartGroupComboBox) d;
			control.CoerceSelectedItem(control.SelectedItemInternal);
		}
		private void CoerceSelectedItem(ISpritePartGroupPart groupPart, [CallerMemberName] string callerName = null) {
			if (supressEvents) return;
			string propName = "null";
			if (GetBindingExpression(ItemsSourceProperty) != null)
				propName = GetBindingExpression(ItemsSourceProperty).ResolvedSourcePropertyName;
			Console.WriteLine($"{propName} SpritePartGroupComboBox.{callerName} coerce");
			supressEvents = true;
			ISpritePartGroup source = ItemsSource;
			if (source == null) {
				groupPart = null;
			}
			else if (groupPart != null && source.TryGetValue(groupPart.Id, out ISpritePartGroupPart newGroupPart)) {
				groupPart = newGroupPart;
			}
			else if (source.IsEnabledByDefault && source.GroupParts.Count > 1) {
				groupPart = source.GroupParts[1];
			}
			else {
				groupPart = source.GroupParts[0];
			}
			SelectedItem = groupPart;
			SelectedItemInternal = groupPart;
			supressEvents = false;
		}
		/*public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register(
				"ItemsSource",
				typeof(ISpritePartGroup),
				typeof(SpritePartGroupComboBox),
				new FrameworkPropertyMetadata(
					OnItemsSourceChanged));
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register(
				"SelectedItem",
				typeof(ISpritePartGroupPart),
				typeof(SpritePartGroupComboBox),
				new FrameworkPropertyMetadata(
					null,
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					null,
					OnCoerceSelectedItem));

		private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpritePartGroupComboBox control = (SpritePartGroupComboBox) d;
			if (e.NewValue != null) {
				control.CoerceValue(SelectedItemProperty);
			}
		}
		private static object OnCoerceSelectedItem(DependencyObject d, object baseValue) {
			SpritePartGroupComboBox control = (SpritePartGroupComboBox) d;
			ISpritePartGroupPart groupPart = (ISpritePartGroupPart) baseValue;
			ISpritePartGroup group = control.ItemsSource;
			if (group == null) {
				return null;
			}
			else if (groupPart != null && group.TryGetValue(groupPart.Id, out ISpritePartGroupPart newGroupPart)) {
				return newGroupPart;
			}
			else if (group.IsEnabledByDefault && group.GroupParts.Skip(1).Any()) {
				return group.GroupParts.Skip(1).First();
			}
			return group.GroupParts.FirstOrDefault();
		}*/

		public ISpritePartGroup ItemsSource {
			get => (ISpritePartGroup) GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}
		public ISpritePartGroupPart SelectedItem {
			get => (ISpritePartGroupPart) GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}
		public ISpritePartGroupPart SelectedItemInternal {
			get => (ISpritePartGroupPart) GetValue(SelectedItemInternalProperty);
			set => SetValue(SelectedItemInternalProperty, value);
		}

		#endregion

		#region Static Constructor

		static SpritePartGroupComboBox() {
			DefaultStyleKeyProperty.AddOwner(typeof(SpritePartGroupComboBox),
				new FrameworkPropertyMetadata(typeof(SpritePartGroupComboBox)));
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
