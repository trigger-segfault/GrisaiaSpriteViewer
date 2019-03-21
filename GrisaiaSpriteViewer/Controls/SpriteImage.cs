using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using Grisaia.Categories;
using Grisaia.Categories.Sprites;
using Grisaia.Mvvm.ViewModel;

namespace Grisaia.SpriteViewer.Controls {
	/// <summary>
	///  A control that displays a sprite selection as an image of all selected parts.
	/// </summary>
	public class SpriteImage : Control {
		/*#region Constants

		/// <summary>
		///  The total number of part group part Ids.
		/// </summary>
		private const int PartCount = Categories.Sprites.SpriteSelection.PartCount;
		/// <summary>
		///  True if HG-3 image data is cached in the <see cref="ISpritePart"/>.
		/// </summary>
		private readonly static bool CacheHg3s = true;

		#endregion*/

		/*#region Fields
		
		/// <summary>
		///  The container that holds all images.
		/// </summary>
		private Grid PART_Container;
		/// <summary>
		///  The difference from the total size of all sprites combined. Used for margins.
		/// </summary>
		private Thickness expandShrink;
		/// <summary>
		///  The current parts being displayed.
		/// </summary>
		private readonly ISpritePart[] parts = new ISpritePart[PartCount];
		/// <summary>
		///  The images inside the <see cref="PART_Container"/>.
		/// </summary>
		private readonly Image[] images = new Image[PartCount];

		#endregion*/

		/*#region Dependency Properties

		/// <summary>
		///  The property for the sprite database used to get the sprite parts from.
		/// </summary>
		public static readonly DependencyProperty SpriteDatabaseProperty =
			DependencyProperty.Register(
				"SpriteDatabase",
				typeof(SpriteDatabase),
				typeof(SpriteImage),
				new FrameworkPropertyMetadata(
					OnSpriteDatabaseChanged));
		/// <summary>
		///  The property for the sprite selection that determines what parts to draw.
		/// </summary>
		public static readonly DependencyProperty SpriteSelectionProperty =
			DependencyProperty.Register(
				"SpriteSelection",
				typeof(IReadOnlySpriteSelection),
				typeof(SpriteImage),
				new FrameworkPropertyMetadata(
					OnSpriteSelectionChanged));
		/// <summary>
		///  The property to determine if the image should be expanded to the total size of all sprites combined.
		/// </summary>
		public static readonly DependencyProperty ExpandProperty =
			DependencyProperty.Register(
				"Expand",
				typeof(bool),
				typeof(SpriteImage),
				new FrameworkPropertyMetadata(
					false,
					OnExpandChanged));

		private static void OnSpriteDatabaseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteImage control = (SpriteImage) d;

			if (e.OldValue is SpriteDatabase oldSpriteDb)
				oldSpriteDb.BuildComplete -= control.OnSpriteDatabaseBuildComplete;
			if (e.NewValue is SpriteDatabase newSpriteDb)
				newSpriteDb.BuildComplete += control.OnSpriteDatabaseBuildComplete;

			control.UpdateSelection();
		}
		private static void OnSpriteSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteImage control = (SpriteImage) d;
			IReadOnlySpriteSelection oldValue = (IReadOnlySpriteSelection) e.OldValue;
			IReadOnlySpriteSelection newValue = (IReadOnlySpriteSelection) e.NewValue;

			if (oldValue is INotifyPropertyChanged oldPropChanged)
				oldPropChanged.PropertyChanged -= control.OnSpriteSelectionPropertyChanged;
			if (newValue is INotifyPropertyChanged newPropChanged)
				newPropChanged.PropertyChanged += control.OnSpriteSelectionPropertyChanged;

			if (oldValue?.GroupPartIds is INotifyCollectionChanged oldColChanged)
				oldColChanged.CollectionChanged -= control.OnSpriteSelectionGroupPartsChanged;
			if (newValue?.GroupPartIds is INotifyCollectionChanged newColChanged)
				newColChanged.CollectionChanged += control.OnSpriteSelectionGroupPartsChanged;

			control.UpdateSelection();
		}
		private static void OnExpandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteImage control = (SpriteImage) d;
			control.UpdateMargins();
		}

		/// <summary>
		///  Gets or sets the sprite database used to get the sprite parts from.
		/// </summary>
		public SpriteDatabase SpriteDatabase {
			get => (SpriteDatabase) GetValue(SpriteDatabaseProperty);
			set => SetValue(SpriteDatabaseProperty, value);
		}
		/// <summary>
		///  Gets or sets the sprite selection that determines what parts to draw.
		/// </summary>
		public IReadOnlySpriteSelection SpriteSelection {
			get => (IReadOnlySpriteSelection) GetValue(SpriteSelectionProperty);
			set => SetValue(SpriteSelectionProperty, value);
		}
		/// <summary>
		///  Gets or sets if the image should be expanded to the total size of all sprites combined.
		/// </summary>
		public bool Expand {
			get => (bool) GetValue(ExpandProperty);
			set => SetValue(ExpandProperty, value);
		}

		#endregion*/

		#region Static Constructor

		static SpriteImage() {
			DefaultStyleKeyProperty.AddOwner(typeof(SpriteImage),
				new FrameworkPropertyMetadata(typeof(SpriteImage)));
		}

		#endregion

		#region Constructors


		#endregion

		/*#region Control Overrides

		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PART_Container = GetTemplateChild("PART_Container") as Grid;
			for (int i = 0; i < PartCount; i++) {
				Image image = images[i] = new Image {
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Top,
				};
				image.SetBinding(Image.VisibilityProperty, new Binding {
					Path = new PropertyPath(nameof(SpritePartViewModelItem.ImageSource)),
					Converter = CollapsedWhenNull.Instance,
				});
				image.SetBinding(Image.SourceProperty, new Binding {
					Path = new PropertyPath(nameof(SpritePartViewModelItem.ImageSource)),
				});
				image.SetBinding(Image.MarginProperty, new Binding {
					Path = new PropertyPath(nameof(SpritePartViewModelItem.Margin)),
				});
				image.SetBinding(Image.WidthProperty, new Binding {
					Path = new PropertyPath(nameof(SpritePartViewModelItem.Width)),
				});
				image.SetBinding(Image.HeightProperty, new Binding {
					Path = new PropertyPath(nameof(SpritePartViewModelItem.Height)),
				});
				PART_Container.Children.Add(images[i]);
			}
			UpdateSelection();
		}

		#endregion*/

		/*#region Event Handlers

		private void OnSpriteDatabaseBuildComplete(object sender, EventArgs e) {
			UpdateSelection();
		}
		private void OnSpriteSelectionPropertyChanged(object sender, PropertyChangedEventArgs e) {
			UpdateSelection();
		}
		private void OnSpriteSelectionGroupPartsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateSelection();
		}

		#endregion*/

		/*#region Private Methods

		/// <summary>
		///  Updates the currently drawn sprite parts and the margins for the images.
		/// </summary>
		private void UpdateSelection() {
			if (PART_Container == null)
				return;

			GameInfo game = null;
			ISpritePart[] newParts;
			if (SpriteDatabase == null || SpriteSelection == null)
				newParts= new ISpritePart[PartCount];
			else
				newParts = SpriteDatabase.GetSpriteParts(SpriteSelection, out game, out _);

			for (int i = 0; i < PartCount; i++) {
				if (parts[i] != newParts[i]) {
					parts[i] = newParts[i];
					UpdateImage(i, game);
				}
			}
			UpdateMargins();
		}
		/// <summary>
		///  Updates the margin boundaries of all the sprite images.
		/// </summary>
		private void UpdateMargins() {
			if (PART_Container == null)
				return;

			var usedParts = parts.Where(p => p != null);
			Size totalSize = new Size();
			expandShrink = new Thickness();
			if (usedParts.Any()) {
				totalSize = new Size(
					usedParts.Max(p => p.Hg3.Images[0].TotalWidth),
					usedParts.Max(p => p.Hg3.Images[0].TotalHeight));
				if (!Expand) {
					expandShrink = new Thickness(
						usedParts.Min(p => p.Hg3.Images[0].MarginLeft),
						usedParts.Min(p => p.Hg3.Images[0].MarginTop),
						usedParts.Min(p => totalSize.Width - (p.Hg3.Images[0].MarginLeft + p.Hg3.Images[0].Width)),
						usedParts.Min(p => totalSize.Height - (p.Hg3.Images[0].MarginTop + p.Hg3.Images[0].Height)));
				}
			}
			for (int typeId = 0; typeId < PartCount; typeId++) {
				ISpritePart part = parts[typeId];
				Image image = images[typeId];
				if (part != null) {
					Hg3Image h = part.Hg3.Images[0];
					image.Width = h.Width;
					image.Height = h.Height;
					image.Margin = new Thickness(
						h.MarginLeft - expandShrink.Left,
						h.MarginTop - expandShrink.Top,
						h.MarginRight - expandShrink.Right,
						h.MarginBottom - expandShrink.Bottom);
				}
			}
		}
		/// <summary>
		///  Updates the image sprite part at the specified index.
		/// </summary>
		/// <param name="index">The index of image sprite part to update.</param>
		/// <param name="game">The current game, used to get the cache directory location.</param>
		private void UpdateImage(int index, GameInfo game) {
			ISpritePart part = parts[index];
			Image image = images[index];
			if (part == null) {
				image.Width = 0;
				image.Height = 0;
				image.Margin = new Thickness();
				image.Source = null;
				image.Visibility = Visibility.Collapsed;
				return;
			}
			
			string cachePath = game.CachePath;

			BitmapImage source = new BitmapImage();
			source.BeginInit();
			if (ViewModelBase.IsInDesignModeStatic)
				source.StreamSource = Embedded.Open(typeof(GrisaiaDatabase).Assembly, Embedded.Combine("Grisaia.data.dummy", part.Hg3.GetFrameFileName(0, 0)));
			else
				source.UriSource = new Uri(part.Hg3.GetFrameFilePath(cachePath, 0, 0));
			source.EndInit();
			
			image.Source = source;
			image.Visibility = Visibility.Visible;
		}

		#endregion*/
	}
}
