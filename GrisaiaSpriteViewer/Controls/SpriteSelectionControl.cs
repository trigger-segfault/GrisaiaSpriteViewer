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
	public class SpriteSelectionControl : Control {
		#region Fields

		private bool templateApplied = false;
		private bool suppressEvents = false;
		
		private StackPanel PART_Section1;
		private StackPanel PART_Section2;
		private StackPanel PART_Section3;

		private Rectangle PART_Separator1;
		private Rectangle PART_Separator2;

		private readonly SpriteCategoryComboBox[] categoryComboBoxes
			= new SpriteCategoryComboBox[SpriteCategoryPool.Count];
		private readonly SpritePartGroupComboBox[] groupComboBoxes
			= new SpritePartGroupComboBox[SpriteSelection.PartCount];
		private readonly ISpritePartGroup[] currentGroups
			= new ISpritePartGroup[SpriteSelection.PartCount];

		//private readonly List<SpriteCategoryComboBox> categoryComboBoxes = new List<SpriteCategoryComboBox>();
		//private readonly List<SpritePartGroupComboBox> groupComboBoxes = new List<SpritePartGroupComboBox>();

		#endregion

		#region Dependency Properties

		public static readonly DependencyProperty SpriteDatabaseProperty =
			DependencyProperty.Register(
				"SpriteDatabase",
				typeof(SpriteDatabase),
				typeof(SpriteSelectionControl),
				new FrameworkPropertyMetadata(
					OnSpriteDatabaseChanged));
		public static readonly DependencyProperty CategoriesProperty =
			DependencyProperty.Register(
				"Categories",
				typeof(ObservableArray<ISpriteCategory>),
				typeof(SpriteSelectionControl),
				new FrameworkPropertyMetadata(
					OnCategoriesChanged));
		public static readonly DependencyProperty GroupsProperty =
			DependencyProperty.Register(
				"Groups",
				typeof(IReadOnlyList<ISpritePartGroup>),
				typeof(SpriteSelectionControl),
				new FrameworkPropertyMetadata(
					//null,
					//FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					OnGroupsChanged));
		public static readonly DependencyProperty GroupPartsProperty =
			DependencyProperty.Register(
				"GroupParts",
				typeof(ObservableArray<ISpritePartGroupPart>),
				typeof(SpriteSelectionControl),
				new FrameworkPropertyMetadata(
					OnGroupPartsChanged));

		private static void OnSpriteDatabaseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteSelectionControl control = (SpriteSelectionControl) d;

			if (e.OldValue is SpriteDatabase oldSpriteDb)
				oldSpriteDb.BuildComplete -= control.OnSpriteDatabaseBuildComplete;
			if (e.NewValue is SpriteDatabase newSpriteDb)
				newSpriteDb.BuildComplete += control.OnSpriteDatabaseBuildComplete;
		}
		private static void OnCategoriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteSelectionControl control = (SpriteSelectionControl) d;
			/*if (e.OldValue is ObservableArray<ISpriteCategory> oldCategories)
				oldCategories.CollectionChanged -= control.OnCategoriesCollectionChanged;
			if (e.NewValue is ObservableArray<ISpriteCategory> newCategories)
				newCategories.CollectionChanged += control.OnCategoriesCollectionChanged;
			control.OnCategoriesCollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));*/
		}
		private static void OnGroupsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteSelectionControl control = (SpriteSelectionControl) d;
			control.UpdateGroups();
		}
		private static void OnGroupPartsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteSelectionControl control = (SpriteSelectionControl) d;
			/*if (e.OldValue is ObservableArray<ISpritePartGroupPart> newGroups)
				newGroups.CollectionChanged -= control.OnGroupPartsCollectionChanged;
			if (e.NewValue is ObservableArray<ISpritePartGroupPart> oldGroups)
				oldGroups.CollectionChanged += control.OnGroupPartsCollectionChanged;*/
		}
		private static void OnPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteSelectionControl control = (SpriteSelectionControl) d;
			control.UpdatePadding();
		}

		public SpriteDatabase SpriteDatabase {
			get => (SpriteDatabase) GetValue(SpriteDatabaseProperty);
			set => SetValue(SpriteDatabaseProperty, value);
		}
		public ObservableArray<ISpriteCategory> Categories {
			get => (ObservableArray<ISpriteCategory>) GetValue(CategoriesProperty);
			set => SetValue(CategoriesProperty, value);
		}
		public IReadOnlyList<ISpritePartGroup> Groups {
			get => (IReadOnlyList<ISpritePartGroup>) GetValue(GroupsProperty);
			set => SetValue(GroupsProperty, value);
		}
		public ObservableArray<ISpritePartGroupPart> GroupParts {
			get => (ObservableArray<ISpritePartGroupPart>) GetValue(GroupPartsProperty);
			set => SetValue(GroupPartsProperty, value);
		}

		#endregion

		#region Static Constructor

		static SpriteSelectionControl() {
			DefaultStyleKeyProperty.AddOwner(typeof(SpriteSelectionControl),
				new FrameworkPropertyMetadata(typeof(SpriteSelectionControl)));
			PaddingProperty.AddOwner(typeof(SpriteSelectionControl),
				new FrameworkPropertyMetadata(OnPaddingChanged));
		}

		#endregion

		#region Constructors

		public SpriteSelectionControl() {
			//GroupPartsInternal = new ObservableArray<ISpritePartGroupPart>(SpriteSelection.PartCount);
		}

		#endregion

		#region Event Handlers

		/*private void OnCategoriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
		}*/
		/*private void OnGroupsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			Console.WriteLine($"SpriteSelectionControl.OnGroupsCollectionChanged");
			for (int i = 0; i < SpriteSelection.PartCount; i++) {
				SpritePartGroupComboBox comboBox = groupComboBoxes[i];
				ISpritePartGroup group = Groups[i];
				ISpritePartGroup currentGroup = currentGroups[i];

				Binding itemsSourceBinding, selectedItemBinding;
				if (group == null && currentGroup != null) {
					Console.WriteLine($"RemoveGroupBinding[{i}]");
					comboBox.Visibility = Visibility.Collapsed;
					itemsSourceBinding = null;
					selectedItemBinding = null;
					comboBox.SetBinding(SpritePartGroupComboBox.ItemsSourceProperty, itemsSourceBinding);
					comboBox.SetBinding(SpritePartGroupComboBox.SelectedItemProperty, selectedItemBinding);
				}
				else if (group != null && currentGroup == null) {
					Console.WriteLine($"AddGroupBinding[{i}]");
					comboBox.Visibility = Visibility.Visible;
					itemsSourceBinding = new Binding() {
						Source = this,
						Path = new PropertyPath($"Groups[{i}]"),
					};
					selectedItemBinding = new Binding() {
						Source = this,
						Path = new PropertyPath($"GroupPartsInternal[{i}]"),
						Mode = BindingMode.TwoWay,
					};
					comboBox.SetBinding(SpritePartGroupComboBox.ItemsSourceProperty, itemsSourceBinding);
					comboBox.SetBinding(SpritePartGroupComboBox.SelectedItemProperty, selectedItemBinding);
				}
				currentGroups[i] = group;
			}
		}*/
		/*private void OnGroupPartsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (suppressEvents) return;
			suppressEvents = true;
			Console.WriteLine($"SpriteSelectionControl.OnGroupPartsCollectionChanged");
			//GroupParts = Array.AsReadOnly(GroupPartsInternal.ToArray());
			suppressEvents = false;
		}*/
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
					Path = new PropertyPath(i == 0 ? "SpriteDatabase" : $"Categories[{(i - 1)}]"),
				};
				selectedItemBinding = new Binding() {
					Source = this,
					Path = new PropertyPath($"Categories[{i}]"),
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
			if (Groups != null)
				UpdateGroups();
			UpdatePadding();
		}

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

		#endregion

		#region Private Methods

		private void UpdateGroups() {
			if (!templateApplied) return;
			if (suppressEvents) return;
			suppressEvents = true;
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
						Path = new PropertyPath($"Groups[{i}]"),
					};
					selectedItemBinding = new Binding() {
						Source = this,
						Path = new PropertyPath($"GroupParts[{i}]"),
						Mode = BindingMode.TwoWay,
					};
					comboBox.SetBinding(SpritePartGroupComboBox.ItemsSourceProperty, itemsSourceBinding);
					comboBox.SetBinding(SpritePartGroupComboBox.SelectedItemProperty, selectedItemBinding);
				}
				currentGroups[i] = group;
			}
			suppressEvents = false;
		}

		private void UpdatePadding() {
			if (!templateApplied) return;
			if (PART_Separator1 != null) {
				Thickness margin = PART_Separator1.Margin;
				margin.Left = -Padding.Left;
				margin.Right = -Padding.Right;
				PART_Separator1.Margin = margin;
				PART_Separator2.Margin = margin;
			}
		}

		#endregion
	}
}
