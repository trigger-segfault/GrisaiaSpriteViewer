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
	/// <summary>
	///  A combo box and label for selecting from a <see cref="ISpriteCategory"/>.
	/// </summary>
	public class SpriteCategoryComboBox : Control {
		#region Fields

		//private TextBlock PART_TextBlock;
		//private ComboBox PART_ComboBox;
		//private Thumb PART_Thumb;
		/// <summary>
		///  Supresses property changed events while another event is taking palce.
		/// </summary>
		private bool supressEvents;

		#endregion

		#region Dependency Properties

		/// <summary>
		///  The property for the sprite category that this combo box makes a selection from.
		/// </summary>
		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register(
				"ItemsSource",
				typeof(ISpriteCategory),
				typeof(SpriteCategoryComboBox),
				new FrameworkPropertyMetadata(
					OnItemsSourceChanged));
		/// <summary>
		///  The property for the selected sprite category.
		/// </summary>
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register(
				"SelectedItem",
				typeof(ISpriteCategory),
				typeof(SpriteCategoryComboBox),
				new FrameworkPropertyMetadata(
					null,
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					OnSelectedItemChanged));
		/// <summary>
		///  The property for the selected sprite category.<para/>
		///  Used to coerce of the <see cref="SelectedItemProperty"/> and only propagate the new value.
		/// </summary>
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

		/// <summary>
		///  Gets or sets the sprite category that this combo box makes a selection from.
		/// </summary>
		public ISpriteCategory ItemsSource {
			get => (ISpriteCategory) GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}
		/// <summary>
		///  Gets or sets the selected sprite category.
		/// </summary>
		public ISpriteCategory SelectedItem {
			get => (ISpriteCategory) GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}
		/// <summary>
		///  Gets or sets the selected sprite category.<para/>
		///  Used to coerce of the <see cref="SelectedItemProperty"/> and only propagate the new value.
		/// </summary>
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
