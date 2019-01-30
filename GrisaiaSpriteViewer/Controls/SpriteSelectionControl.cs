using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using Grisaia.Categories;
using Grisaia.Categories.Sprites;
using Grisaia.Utils;

namespace Grisaia.SpriteViewer.Controls {
	/// <summary>
	///  The control for managing the combo boxes used for sprite selection.
	/// </summary>
	public class SpriteSelectionControl : Control {
		#region Fields

		/// <summary>
		///  True if the <see cref="OnApplyTemplate"/> has been called.
		/// </summary>
		private bool templateApplied = false;
		/*/// <summary>
		///  Supresses property changed events while another event is taking palce.
		/// </summary>
		private bool suppressEvents = false;*/

		/// <summary>
		///  The stack panel for the primary sprite category combo boxes.
		/// </summary>
		private StackPanel PART_Section1;
		/// <summary>
		///  The stack panel for the secondary sprite category combo boxes.
		/// </summary>
		private StackPanel PART_Section2;
		/// <summary>
		///  The stack panel for the sprite part group combo boxes.
		/// </summary>
		private StackPanel PART_Section3;

		/// <summary>
		///  The separator between <see cref="PART_Section1"/> and <see cref="PART_Section2"/>.
		/// </summary>
		private Rectangle PART_Separator1;
		/// <summary>
		///  The separator between <see cref="PART_Section2"/> and <see cref="PART_Section3"/>.
		/// </summary>
		private Rectangle PART_Separator2;

		/// <summary>
		///  The combo box controls for the sprite categories.
		/// </summary>
		private readonly SpriteCategoryComboBox[] categoryComboBoxes
			= new SpriteCategoryComboBox[SpriteCategoryPool.Count];
		/// <summary>
		///  The combo box controls for the sprite part groups.
		/// </summary>
		private readonly SpritePartGroupComboBox[] groupComboBoxes
			= new SpritePartGroupComboBox[SpriteSelection.PartCount];
		/// <summary>
		///  The current sprite part groups being used for the sprite group part combo boxes.
		/// </summary>
		private readonly ISpritePartGroup[] currentGroups
			= new ISpritePartGroup[SpriteSelection.PartCount];

		#endregion

		#region Dependency Properties

		/// <summary>
		///  The property for the sprite database which is always the first items source category.
		/// </summary>
		public static readonly DependencyProperty SpriteDatabaseProperty =
			DependencyProperty.Register(
				"SpriteDatabase",
				typeof(SpriteDatabase),
				typeof(SpriteSelectionControl),
				new FrameworkPropertyMetadata(
					OnSpriteDatabaseChanged));
		/// <summary>
		///  The property for the categories used for sprite selection.
		/// </summary>
		public static readonly DependencyProperty CategoriesProperty =
			DependencyProperty.Register(
				"Categories",
				typeof(ObservableArray<ISpriteCategory>),
				typeof(SpriteSelectionControl));
		/// <summary>
		///  The property for the groups used for sprite part group part selection.
		/// </summary>
		public static readonly DependencyProperty GroupsProperty =
			DependencyProperty.Register(
				"Groups",
				typeof(IReadOnlyList<ISpritePartGroup>),
				typeof(SpriteSelectionControl),
				new FrameworkPropertyMetadata(
					OnGroupsChanged));
		/// <summary>
		///  The property for the selected sprite part group parts.
		/// </summary>
		public static readonly DependencyProperty GroupPartsProperty =
			DependencyProperty.Register(
				"GroupParts",
				typeof(ObservableArray<ISpritePartGroupPart>),
				typeof(SpriteSelectionControl));

		private static void OnSpriteDatabaseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteSelectionControl control = (SpriteSelectionControl) d;

			if (e.OldValue is SpriteDatabase oldSpriteDb)
				oldSpriteDb.BuildComplete -= control.OnSpriteDatabaseBuildComplete;
			if (e.NewValue is SpriteDatabase newSpriteDb)
				newSpriteDb.BuildComplete += control.OnSpriteDatabaseBuildComplete;
		}
		private static void OnGroupsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteSelectionControl control = (SpriteSelectionControl) d;
			control.UpdateGroups();
		}

		/// <summary>
		///  Gets or sets the sprite database which is always the first items source category.
		/// </summary>
		public SpriteDatabase SpriteDatabase {
			get => (SpriteDatabase) GetValue(SpriteDatabaseProperty);
			set => SetValue(SpriteDatabaseProperty, value);
		}
		/// <summary>
		///  Gets or sets the categories used for sprite selection.
		/// </summary>
		public ObservableArray<ISpriteCategory> Categories {
			get => (ObservableArray<ISpriteCategory>) GetValue(CategoriesProperty);
			set => SetValue(CategoriesProperty, value);
		}
		/// <summary>
		///  Gets or sets the groups used for sprite part group part selection.
		/// </summary>
		public IReadOnlyList<ISpritePartGroup> Groups {
			get => (IReadOnlyList<ISpritePartGroup>) GetValue(GroupsProperty);
			set => SetValue(GroupsProperty, value);
		}
		/// <summary>
		///  Gets or sets the selected sprite part group parts.
		/// </summary>
		public ObservableArray<ISpritePartGroupPart> GroupParts {
			get => (ObservableArray<ISpritePartGroupPart>) GetValue(GroupPartsProperty);
			set => SetValue(GroupPartsProperty, value);
		}

		#endregion

		#region Static Constructor

		static SpriteSelectionControl() {
			DefaultStyleKeyProperty.AddOwner(typeof(SpriteSelectionControl),
				new FrameworkPropertyMetadata(typeof(SpriteSelectionControl)));
		}

		#endregion

		#region Event Handlers
		
		private void OnSpriteDatabaseBuildComplete(object sender, EventArgs e) {

		}

		#endregion

		#region Control Overrides

		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PART_Section1 = GetTemplateChild("PART_Section1") as StackPanel;
			PART_Section2 = GetTemplateChild("PART_Section2") as StackPanel;
			PART_Section3 = GetTemplateChild("PART_Section3") as StackPanel;
			PART_Separator1 = GetTemplateChild("PART_Separator1") as Rectangle;
			PART_Separator2 = GetTemplateChild("PART_Separator2") as Rectangle;
			
			Grid[] grids = AddGrids(PART_Section2, SpriteCategoryPool.Count - 2);
			for (int i = 0; i < SpriteCategoryPool.Count; i++) {
				SpriteCategoryComboBox comboBox = new SpriteCategoryComboBox();

				Binding itemsSourceBinding, selectedItemBinding;
				itemsSourceBinding = new Binding() {
					Source = this,
					Path = new PropertyPath(i == 0 ? nameof(SpriteDatabase) : $"{nameof(Categories)}[{(i - 1)}]"),
				};
				selectedItemBinding = new Binding() {
					Source = this,
					Path = new PropertyPath($"{nameof(Categories)}[{i}]"),
					Mode = BindingMode.TwoWay,
				};
				comboBox.SetBinding(SpriteCategoryComboBox.ItemsSourceProperty, itemsSourceBinding);
				comboBox.SetBinding(SpriteCategoryComboBox.SelectedItemProperty, selectedItemBinding);

				categoryComboBoxes[i] = comboBox;
				if (i < 2) {
					PART_Section1.Children.Add(comboBox);
				}
				else {
					if (i % 2 == 1)
						Grid.SetColumn(comboBox, 2);
					grids[(i - 2) / 2].Children.Add(comboBox);
				}
			}
			grids = AddGrids(PART_Section3, SpriteSelection.PartCount);
			for (int i = 0; i < SpriteSelection.PartCount; i++) {
				SpritePartGroupComboBox comboBox = new SpritePartGroupComboBox {
					Visibility = Visibility.Collapsed,
				};

				groupComboBoxes[i] = comboBox;
				if (i % 2 == 1)
					Grid.SetColumn(comboBox, 2);
				grids[i / 2].Children.Add(comboBox);
			}

			templateApplied = true;
			UpdateGroups();
		}

		#endregion

		#region Private Methods

		/// <summary>
		///  Adds the grids to a section that split the combo boxes into columns.
		/// </summary>
		/// <param name="container">The stack panel section container.</param>
		/// <param name="count">The number of combo boxes that need room.</param>
		/// <returns>An array of the created grids.</returns>
		private Grid[] AddGrids(StackPanel container, int count) {
			Grid[] grids = new Grid[(count + 1) / 2];
			for (int i = 0; i < grids.Length; i++) {
				Grid grid = new Grid();
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4, GridUnitType.Pixel) });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
				grids[i] = grid;
				container.Children.Add(grid);
			}
			return grids;
		}
		/// <summary>
		///  Updates the combo boxes for the sprite part groups when the groups have changed.
		/// </summary>
		private void UpdateGroups() {
			if (!templateApplied || Groups == null) return;
			//if (suppressEvents) return;
			//suppressEvents = true;
			Console.WriteLine($"SpriteSelectionControl.UpdateGroups");
			for (int i = 0; i < SpriteSelection.PartCount; i++) {
				SpritePartGroupComboBox comboBox = groupComboBoxes[i];
				ISpritePartGroup group = Groups[i];
				ISpritePartGroup currentGroup = currentGroups[i];

				Binding itemsSourceBinding, selectedItemBinding;
				if (group == null && currentGroup != null) {
					Console.WriteLine($"[{i}] SpriteSelectionControl.RemoveGroup");
					comboBox.Visibility = Visibility.Collapsed;
					itemsSourceBinding = null;
					selectedItemBinding = null;
					comboBox.ItemsSource = null;
					comboBox.SelectedItem = null;
				}
				else if (group != null && currentGroup == null) {
					Console.WriteLine($"[{i}] SpriteSelectionControl.AddGroup");
					comboBox.Visibility = Visibility.Visible;
					itemsSourceBinding = new Binding() {
						Source = this,
						Path = new PropertyPath($"{nameof(Groups)}[{i}]"),
					};
					selectedItemBinding = new Binding() {
						Source = this,
						Path = new PropertyPath($"{nameof(GroupParts)}[{i}]"),
						Mode = BindingMode.TwoWay,
					};
					comboBox.SetBinding(SpritePartGroupComboBox.ItemsSourceProperty, itemsSourceBinding);
					comboBox.SetBinding(SpritePartGroupComboBox.SelectedItemProperty, selectedItemBinding);
				}
				currentGroups[i] = group;
			}
			//suppressEvents = false;
		}

		#endregion
	}
}
