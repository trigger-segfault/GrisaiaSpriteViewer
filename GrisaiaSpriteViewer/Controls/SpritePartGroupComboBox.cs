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
	/// <summary>
	///  A combo box and label for selecting from a <see cref="ISpritePartGroup"/>.
	/// </summary>
	public class SpritePartGroupComboBox : Control {
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
		///  The property for the sprite part group that this combo box makes a selection from.
		/// </summary>
		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register(
				"ItemsSource",
				typeof(ISpritePartGroup),
				typeof(SpritePartGroupComboBox),
				new FrameworkPropertyMetadata(
					OnItemsSourceChanged));
		/// <summary>
		///  The property for the selected sprite part group part.
		/// </summary>
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register(
				"SelectedItem",
				typeof(ISpritePartGroupPart),
				typeof(SpritePartGroupComboBox),
				new FrameworkPropertyMetadata(
					null,
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					OnSelectedItemChanged));
		/// <summary>
		///  The property for the selected sprite part group part.<para/>
		///  Used to coerce of the <see cref="SelectedItemProperty"/> and only propagate the new value.
		/// </summary>
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

		/// <summary>
		///  Gets or sets the sprite part group that this combo box makes a selection from.
		/// </summary>
		public ISpritePartGroup ItemsSource {
			get => (ISpritePartGroup) GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}
		/// <summary>
		///  Gets or sets the selected sprite part group part.
		/// </summary>
		public ISpritePartGroupPart SelectedItem {
			get => (ISpritePartGroupPart) GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}
		/// <summary>
		///  Gets or sets the selected sprite part group part.<para/>
		///  Used to coerce of the <see cref="SelectedItemProperty"/> and only propagate the new value.
		/// </summary>
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
