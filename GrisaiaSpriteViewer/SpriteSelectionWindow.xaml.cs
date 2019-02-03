using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Grisaia.Asmodean;
using Grisaia.Categories;
using Grisaia.Categories.Sprites;
using Grisaia.Mvvm.Utils;
using Grisaia.Mvvm.ViewModel;
using Microsoft.Win32;

namespace Grisaia.SpriteViewer {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class SpriteSelectionWindow : Window {
		#region Fields
		
		//private double scale = 1.0;
		//private bool centered = true;
		//private int currentWidth = 0;
		//private int currentHeight = 0;
		//private Thickness expandShrink;

		//private double savedScale = 1.0;
		//private Vector savedNormalizedScroll = new Vector(-1, -1);

		private bool supressEvents = false;

		#endregion

		#region Properties

		public SpriteSelectionViewModel ViewModel => (SpriteSelectionViewModel) DataContext;

		#endregion
		#region Static Constructor

		static SpriteSelectionWindow() {
			//DataContextProperty.AddOwner(typeof(SpriteSelectionWindow),
			//	new FrameworkPropertyMetadata(
			//		OnDataContextChanged));
		}

		#endregion

		#region Constructors

		public SpriteSelectionWindow() {
			InitializeComponent();
		}

		#endregion


		private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteSelectionWindow window = (SpriteSelectionWindow) d;
			window.ViewModel.WindowOwner = window;
		}
		private void OnClosed(object sender, EventArgs e) {
			ViewModel.WindowOwner = null;
		}

		private void OnLoaded(object sender, RoutedEventArgs e) {
			//comboGame.SelectedIndex = 0;
			ViewModel.PropertyChanged += OnViewModelPropertyChanged;
			OnViewModelPropertyChanged(null, new PropertyChangedEventArgs(nameof(SpriteSelectionViewModel.CurrentParts)));
		}

		private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == nameof(SpriteSelectionViewModel.CurrentParts)) {
				UpdatePartChanges();
			}
		}

		private void UpdatePartChanges() {
			//Vector normalized = CalculateNormalizedScrollCenter();
			/*for (int typeId = 0; typeId < DataContext.Cur; typeId++) {
				var part = parts[typeId];
				var image = imagePart[typeId];
				if (currentParts[typeId] != part) {
					currentParts[typeId] = part;
					SetImage(image, part);
				}
			}*/
			//var usedParts = parts.Where(p => p != null);
			/*UpdateExpand();
			if (centered) {
				UpdateCentered();
			}
			else {
				Vector areaCenter = CalculateAreaCenter();
				Vector newScroll = new Vector(
					normalized.X * areaCenter.X,
					normalized.Y * areaCenter.Y);
				transform.ScaleX = scale;
				transform.ScaleY = scale;
				scrollSprite.ScrollToHorizontalOffset(newScroll.X - scrollSprite.ViewportWidth / 2);
				scrollSprite.ScrollToVerticalOffset(newScroll.Y - scrollSprite.ViewportHeight / 2);
			}
			NewLines();*/
			UpdatePartList();
			UpdateStatusBar();
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}

		/*private void UpdateExpand() {
			var usedParts = ViewModel.CurrentParts.Where(p => p != null);
			if (usedParts.Any()) {
				Size totalSize = new Size(
					usedParts.Max(p => p.Hg3.Images[0].TotalWidth),
					usedParts.Max(p => p.Hg3.Images[0].TotalHeight));
				expandShrink = new Thickness();
				if (!menuItemExpand.IsChecked) {
					expandShrink = new Thickness(
						usedParts.Min(p => p.Hg3.Images[0].MarginLeft),
						usedParts.Min(p => p.Hg3.Images[0].MarginTop),
						usedParts.Min(p => totalSize.Width - (p.Hg3.Images[0].MarginLeft + p.Hg3.Images[0].Width)),
						usedParts.Min(p => totalSize.Height - (p.Hg3.Images[0].MarginTop + p.Hg3.Images[0].Height)));
				}
				currentWidth = (int) Math.Round(totalSize.Width - expandShrink.Left - expandShrink.Right);
				currentHeight = (int) Math.Round(totalSize.Height - expandShrink.Top - expandShrink.Bottom);
			}
			else {
				currentWidth = 1;
				currentHeight = 1;
			}
			//gridSprite.Width = currentWidth;
			//gridSprite.Height = currentHeight;
			gridSprite.Margin = CalculateAreaMargins();
			gridLines.Margin = gridSprite.Margin;
		}*/

		private void UpdatePartList() {
			labelPartList.Text =
				string.Join("\n", ViewModel.CurrentParts
				.Where(p => p != null)
				.Select(p => Path.GetFileNameWithoutExtension(p.FileName)));
		}

		private void OnSaveSprite(object sender, RoutedEventArgs e) {
			var usedParts = ViewModel.CurrentParts.Where(p => p != null);
			if (usedParts.Any()) {
				SaveFileDialog dialog = new SaveFileDialog {
					FileName = GetSpriteUniqueFileName(),
					Filter = "PNG Images|*.png",
					OverwritePrompt = true,
					AddExtension = true,
					InitialDirectory = Path.Combine(AppContext.BaseDirectory, "saved"),
				};
				if (!Directory.Exists(dialog.InitialDirectory))
					Directory.CreateDirectory(dialog.InitialDirectory);
				bool result = dialog.ShowDialog() ?? false;
				if (!result)
					return;
				int width = usedParts.Max(p => p.Hg3.Images[0].TotalWidth);
				int height = usedParts.Max(p => p.Hg3.Images[0].TotalHeight);
				using (var bitmap = BuildImage())
					bitmap.Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
			}
		}

		private void OnCopySprite(object sender, RoutedEventArgs e) {
			var usedParts = ViewModel.CurrentParts.Where(p => p != null);
			if (usedParts.Any()) {
				using (var bitmap = BuildImage())
				using (var bitmapNoTr = RemoveTransparency(bitmap)) {
					string tempPath = Path.Combine(AppContext.BaseDirectory, "cache", "clipboard.png");
					bitmap.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);
					ImageClipboard.SetClipboardImage(bitmap, bitmapNoTr, tempPath);
				}
			}
		}

		private System.Drawing.Bitmap RemoveTransparency(System.Drawing.Bitmap bitmap) {
			var newBitmap = new System.Drawing.Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			try {
				using (var g = System.Drawing.Graphics.FromImage(newBitmap)) {
					g.Clear(System.Drawing.Color.White);
					g.DrawImageUnscaled(bitmap, 0, 0);
				}
				return newBitmap;
			}
			catch {
				newBitmap.Dispose();
				throw;
			}
		}

		private System.Drawing.Bitmap BuildImage() {
			return null;
			// TODO: Move to View Model
			/*var usedParts = ViewModel.CurrentParts.Where(p => p != null);
			string dir = Path.Combine(AppContext.BaseDirectory, "cache", ViewModel.CurrentGame.Id);
			var bitmap = new System.Drawing.Bitmap((int) ViewModel.SpriteSize.Width, (int) ViewModel.SpriteSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			try {
				using (var g = System.Drawing.Graphics.FromImage(bitmap)) {
					g.Clear(System.Drawing.Color.Transparent);
					foreach (var part in usedParts) {
						string partFile = Path.Combine(dir, part.Hg3.GetFrameFileName(0, 0));
						using (var partBitmap = System.Drawing.Image.FromFile(partFile))
							g.DrawImageUnscaled(partBitmap,
								part.Hg3.Images[0].MarginLeft - (int) Math.Round(expandShrink.Left),
								part.Hg3.Images[0].MarginTop - (int) Math.Round(expandShrink.Top));
					}
				}
				return bitmap;
			}
			catch {
				// Only dispose on exception
				bitmap.Dispose();
				throw;
			}*/
		}

		private string GetSpriteUniqueId() {
			StringBuilder str = new StringBuilder();
			var selection = ViewModel.SpriteImage.SpriteSelection;
			str.Append(selection.GameId);
			str.Append("-");
			str.Append(selection.CharacterId);
			str.Append("__L");
			str.Append((int) selection.Lighting);
			str.Append("-D");
			str.Append((int) selection.Distance);
			str.Append("-P");
			str.Append((int) selection.Pose);
			str.Append("-B");
			str.Append((int) selection.Blush);

			str.Append("__");
			str.Append(string.Join("-", ViewModel.CurrentParts.Select((p, i) => (p != null ? $"{i}P{p.Id:D2}" : null)).Where(p => p != null)));
			return str.ToString();
		}
		private string GetSpriteUniqueFileName() {
			return GetSpriteUniqueId() + ".png";
		}

		private void UpdateStatusBar() {
			//statusItemDimensions.Content = $"{currentWidth}x{currentHeight}";
			//statusItemScale.Content = $"{scale:P0}";
			statusItemUniqueId.Content = GetSpriteUniqueId();
		}

		/*private void OnZoomImage(object sender, System.Windows.Input.MouseWheelEventArgs e) {
			if (Keyboard.Modifiers!= ModifierKeys.Control)
				return;
			if (centered) {
				centered = false;
				scrollSprite.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
				scrollSprite.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
			}

			Point gridPoint = e.GetPosition(gridSprite);
			Point scrollPoint = e.GetPosition(scrollSprite);

			//Vector beforeOffset = CalculateAreaOffset();
			Vector beforeCenter = new Vector(
				scrollPoint.X - scrollSprite.ViewportWidth  / 2,
				scrollPoint.Y - scrollSprite.ViewportHeight / 2);
			//Vector beforeArea = new Vector(gridPoint.X, gridPoint.Y) + beforeOffset;

			double scaleChange = e.Delta > 0 ? 1.125 : 1 / 1.125;
			// 1% scale is the minimum
			if (scale == 0.01 && scaleChange < 1.0) {
				e.Handled = true;
				return;
			}
			double oldScale = scale;
			scale *= scaleChange;
			if (scale < 0.01) {
				scale = 0.01;
				scaleChange = scale / oldScale;
			}
			else if ((oldScale < 1 && scale > 1) || (oldScale > 1 && scale < 1)) {
				scale = 1;
				scaleChange = scale / oldScale;
			}

			Vector afterOffset = CalculateAreaOffset();
			//Vector afterCenter = beforeCenter * scaleChange;
			Vector afterArea = new Vector(gridPoint.X, gridPoint.Y) * scale + afterOffset;

			transform.ScaleX = scale;
			transform.ScaleY = scale;

			gridSprite.Margin = CalculateAreaMargins();
			gridLines.Margin = gridSprite.Margin;

			//Vector centerChange = afterCenter - beforeCenter;
			Vector newScroll = afterArea - beforeCenter;// - afterCenter + centerChange;
			scrollSprite.ScrollToHorizontalOffset(newScroll.X - scrollSprite.ViewportWidth / 2);
			scrollSprite.ScrollToVerticalOffset(newScroll.Y - scrollSprite.ViewportHeight / 2);

			UpdateStatusBar();
			e.Handled = true;
		}*/

		private void OnPreviewKeyDown(object sender, KeyEventArgs e) {
			/*if (e.Key == Key.B && Keyboard.Modifiers == ModifierKeys.Control) {
				Vector normalizedScroll = CalculateNormalizedScrollCenter();
				centered = !centered;
				Vector newScroll = CalculateAreaCenter();
				if (centered) {
					savedScale = scale;
					savedNormalizedScroll = normalizedScroll;
					scrollSprite.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
					scrollSprite.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
					UpdateCentered();
				}
				else {
					scale = savedScale;
					scrollSprite.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
					scrollSprite.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
					transform.ScaleX = scale;
					transform.ScaleY = scale;
					gridSprite.Margin = CalculateAreaMargins();
					gridLines.Margin = gridSprite.Margin;
					if (savedNormalizedScroll.X >= 0 && savedNormalizedScroll.Y >= 0) {
						Vector areaCenter = CalculateAreaCenter();
						newScroll.X = savedNormalizedScroll.X * areaCenter.X;
						newScroll.Y = savedNormalizedScroll.Y * areaCenter.Y;
					}
					scrollSprite.ScrollToHorizontalOffset(newScroll.X - scrollSprite.ViewportWidth / 2);
					scrollSprite.ScrollToVerticalOffset(newScroll.Y - scrollSprite.ViewportHeight / 2);
					UpdateStatusBar();
				}
			}*/
		}

		/*private Vector CalculateAreaCenter() {
			return new Vector(currentWidth, currentHeight) * scale / 2 + CalculateAreaOffset();
		}
		private Vector CalculateScrollCenter() {
			return new Vector(
				scrollSprite.HorizontalOffset + scrollSprite.ViewportWidth / 2,
				scrollSprite.VerticalOffset + scrollSprite.ViewportHeight / 2);
		}
		private Vector CalculateNormalizedScrollCenter() {
			Vector areaCenter = CalculateAreaCenter();
			Vector scrollCenter = CalculateScrollCenter();
			return new Vector(
				scrollCenter.X / areaCenter.X,
				scrollCenter.Y / areaCenter.Y);
		}


		private void UpdateCentered() {
			if (currentWidth == 0 || currentHeight == 0) {
				scale = 1;
				transform.ScaleX = scale;
				transform.ScaleY = scale;
			}
			else {
				Vector areaOffset = CalculateAreaOffset();
				Vector area = new Vector(
					scrollSprite.ViewportWidth,
					scrollSprite.ViewportHeight) - areaOffset * 2;
				//Thickness margin = 
				double areaRatio = area.X / area.Y;
				double spriteRatio = (double) currentWidth / currentHeight;
				if (areaRatio > spriteRatio) {
					scale = Math.Min(1, area.Y / currentHeight);
				}
				else {
					scale = Math.Min(1, area.X / currentWidth);
				}
				areaOffset.X = (scrollSprite.ViewportWidth - currentWidth * scale) / 2;
				areaOffset.Y = (scrollSprite.ViewportHeight - currentHeight * scale) / 2;
				gridSprite.Margin = new Thickness(areaOffset.X, areaOffset.Y, areaOffset.X, areaOffset.Y);
				gridLines.Margin = gridSprite.Margin;
				transform.ScaleX = scale;
				transform.ScaleY = scale;
			}
			UpdateStatusBar();
		}*/

		/*private void OnScrollSizeChanged(object sender, SizeChangedEventArgs e) {
		}

		private void OnScrollChanged(object sender, ScrollChangedEventArgs e) {
			Thickness margin = new Thickness();
			if (scrollSprite.ComputedHorizontalScrollBarVisibility == Visibility.Visible)
				margin.Right = 1;
			if (scrollSprite.ComputedVerticalScrollBarVisibility == Visibility.Visible)
				margin.Bottom = 1;
			scrollSprite.Padding = margin;
			scrollSprite.Margin = margin;
			gridSprite.Margin = CalculateAreaMargins();
			gridLines.Margin = gridSprite.Margin;
			if (centered)
				UpdateCentered();
			UpdateLines();
		}
		
		private Vector CalculateAreaOffset() {
			if (centered)
				return new Vector(15, 15);
			double viewWidth  = scrollSprite.ViewportWidth;
			double viewHeight = scrollSprite.ViewportHeight;
			double scaledWidth  = currentWidth  * scale;
			double scaledHeight = currentHeight * scale;
			double x, y;
			if (scaledWidth > viewWidth)
				x = viewWidth / 2;
			else
				x = viewWidth - scaledWidth / 2;
			if (scaledHeight > viewHeight)
				y = viewHeight / 2;
			else
				y = viewHeight - scaledHeight / 2;
			//horizontal /= scale;
			//vertical /= scale;
			return new Vector(x, y);
		}
		private Thickness CalculateAreaMargins() {
			var offset = CalculateAreaOffset();
			return new Thickness(offset.X, offset.Y, offset.X, offset.Y);
		}*/

		private void OnExpandChanged(object sender, RoutedEventArgs e) {
			UpdatePartChanges();
		}

		private void OnShowLinesChanged(object sender, RoutedEventArgs e) {
			//NewLines();
		}

		/*private void NewLines() {
			gridLines.Children.Clear();
			if (menuItemShowGuidelines.IsChecked) {
				HashSet<int> centers = new HashSet<int>();
				HashSet<int> baselines = new HashSet<int>();
				foreach (var part in ViewModel.CurrentParts.Where(p => p != null)) {
					var c = part.Hg3.Images[0];
					if (centers.Add(c.Center)) {
						System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle {
							Fill = Brushes.Red,
							HorizontalAlignment = HorizontalAlignment.Left,
							Width = 1,
						};
						gridLines.Children.Add(r);
					}
					if (baselines.Add(c.Baseline)) {
						System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle {
							Fill = Brushes.Blue,
							VerticalAlignment = VerticalAlignment.Top,
							Height = 1,
						};
						gridLines.Children.Add(r);
					}
				}
			}
			UpdateLines();
		}

		private void UpdateLines() {
			if (menuItemShowGuidelines.IsChecked) {
				HashSet<int> centers = new HashSet<int>();
				HashSet<int> baselines = new HashSet<int>();
				int index = 0;
				foreach (var part in ViewModel.CurrentParts.Where(p => p != null)) {
					var c = part.Hg3.Images[0];
					if (centers.Add(c.Center)) {
						System.Windows.Shapes.Rectangle r = (System.Windows.Shapes.Rectangle) gridLines.Children[index++];
						r.Margin = new Thickness((c.Center - expandShrink.Left) * scale, 0, 0, 0);
					}
					if (baselines.Add(c.Baseline)) {
						System.Windows.Shapes.Rectangle r = (System.Windows.Shapes.Rectangle) gridLines.Children[index++];
						r.Margin = new Thickness(0, (c.Baseline - expandShrink.Top) * scale, 0, 0);
					}
				}
			}
		}*/

		private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			Console.WriteLine("-------------------------------------");
		}
	}
}
