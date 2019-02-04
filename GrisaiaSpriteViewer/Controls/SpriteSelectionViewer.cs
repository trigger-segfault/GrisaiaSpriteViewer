using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Grisaia.Categories.Sprites;
using Grisaia.Mvvm.ViewModel;

namespace Grisaia.SpriteViewer.Controls {
	public class SpriteSelectionViewer : Control {
		#region Dependency Properties
		
		public static readonly DependencyProperty ScaleProperty =
			DependencyProperty.Register(
				"Scale",
				typeof(double),
				typeof(SpriteSelectionViewer),
				new FrameworkPropertyMetadata(
					OnScaleChanged));
		public static readonly DependencyProperty CenteredProperty =
			DependencyProperty.Register(
				"Centered",
				typeof(bool),
				typeof(SpriteSelectionViewer),
				new FrameworkPropertyMetadata(
					OnCenteredChanged));
		public static readonly DependencyProperty ExpandProperty =
			DependencyProperty.Register(
				"Expand",
				typeof(bool),
				typeof(SpriteSelectionViewer),
				new FrameworkPropertyMetadata(
					OnExpandChanged));
		public static readonly DependencyProperty ShowGridLinesProperty =
			DependencyProperty.Register(
				"ShowGridLines",
				typeof(bool),
				typeof(SpriteSelectionViewer));
		public static readonly DependencyProperty SpriteSizeProperty =
			DependencyProperty.Register(
				"SpriteSize",
				typeof(Size),
				typeof(SpriteSelectionViewer),
				new FrameworkPropertyMetadata(
					OnSpriteSizeChanged));
		public static readonly DependencyProperty SpriteOriginProperty =
			DependencyProperty.Register(
				"SpriteOrigin",
				typeof(Point),
				typeof(SpriteSelectionViewer),
				new FrameworkPropertyMetadata(
					OnSpriteOriginChanged));

		private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteSelectionViewer control = (SpriteSelectionViewer) d;
		}
		private static void OnCenteredChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteSelectionViewer control = (SpriteSelectionViewer) d;
			control.CenterToggled();
		}
		private static void OnExpandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteSelectionViewer control = (SpriteSelectionViewer) d;
			if (!control.IsEmpty)
				control.UpdatePartChanges();
			//control.UpdateExpand();
		}
		private static void OnSpriteSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteSelectionViewer control = (SpriteSelectionViewer) d;
			Size previousSize = (Size) e.OldValue;
			if (!control.IsEmpty) {
				control.validSpriteSize = control.SpriteSize;
				if (previousSize.Width == 0 || previousSize.Height == 0)
					control.UpdateEmptyChanged();
				control.UpdatePartChanges();
			}
			else {
				control.UpdateEmptyChanged();
			}
		}
		private static void OnSpriteOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteSelectionViewer control = (SpriteSelectionViewer) d;
			if (!control.IsEmpty)
				control.UpdateLines();
		}

		public double Scale {
			get => (double) GetValue(ScaleProperty);
			set => SetValue(ScaleProperty, value);
		}
		public bool Centered {
			get => (bool) GetValue(CenteredProperty);
			set => SetValue(CenteredProperty, value);
		}
		public bool Expand {
			get => (bool) GetValue(ExpandProperty);
			set => SetValue(ExpandProperty, value);
		}
		public bool ShowGridLines {
			get => (bool) GetValue(ShowGridLinesProperty);
			set => SetValue(ShowGridLinesProperty, value);
		}
		public Size SpriteSize {
			get => (Size) GetValue(SpriteSizeProperty);
			set => SetValue(SpriteSizeProperty, value);
		}
		public Point SpriteOrigin {
			get => (Point) GetValue(SpriteOriginProperty);
			set => SetValue(SpriteOriginProperty, value);
		}

		#endregion

		#region Fields

		private bool templateApplied = false;
		private bool supressEvents = false;

		private double savedScale = 1.0;
		private Vector savedNormalizedScroll = new Vector(-1, -1);
		
		private ScrollViewer PART_ScrollViewer;
		private Grid PART_SpriteArea;
		private Grid PART_SpriteContainer;
		private SpriteImage PART_SpriteImage;
		private ScaleTransform PART_ScaleTransform;
		private Grid PART_GridLines;
		private Rectangle PART_GridLineCenter;
		private Rectangle PART_GridLineBaseline;
		/// <summary>
		///  The last non-empty sprite size. Used for scroll calculations when empty sprite size cannot be used.
		/// </summary>
		private Size validSpriteSize = new Size(1, 1);

		#endregion

		#region Private Properties

		private bool IsEmpty => SpriteSize.Width == 0 || SpriteSize.Height == 0;

		#endregion

		#region Static Constructor

		static SpriteSelectionViewer() {
			DefaultStyleKeyProperty.AddOwner(typeof(SpriteSelectionViewer),
				new FrameworkPropertyMetadata(typeof(SpriteSelectionViewer)));
		}

		#endregion

		#region Control Overrides

		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PART_ScrollViewer	  = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
			PART_SpriteArea		  = GetTemplateChild("PART_SpriteArea") as Grid;
			PART_SpriteContainer  = GetTemplateChild("PART_SpriteContainer") as Grid;
			PART_SpriteImage	  = GetTemplateChild("PART_SpriteImage") as SpriteImage;
			PART_ScaleTransform   = GetTemplateChild("PART_ScaleTransform") as ScaleTransform;
			PART_GridLines		  = GetTemplateChild("PART_GridLines") as Grid;
			PART_GridLineCenter	  = GetTemplateChild("PART_GridLineCenter") as Rectangle;
			PART_GridLineBaseline = GetTemplateChild("PART_GridLineBaseline") as Rectangle;
			
			PART_ScrollViewer.ScrollChanged += OnScrollChanged;
			PART_SpriteArea.MouseWheel += OnSpriteAreaZoom;
			templateApplied = true;

			UpdateEmptyChanged();
			UpdatePartChanges();
		}

		#endregion

		#region Event Handlers

		private void OnSpriteAreaZoom(object sender, MouseWheelEventArgs e) {
			if (Keyboard.Modifiers!= ModifierKeys.Control)
				return;
			if (Centered) {
				supressEvents = true;
				Centered = false;
				supressEvents = false;
				if (!IsEmpty) {
					PART_ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
					PART_ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
				}
			}

			Point spritePoint = e.GetPosition(PART_SpriteImage);
			Point scrollPoint = e.GetPosition(PART_ScrollViewer);

			Vector beforeCenter = new Vector(
				scrollPoint.X - PART_ScrollViewer.ViewportWidth  / 2,
				scrollPoint.Y - PART_ScrollViewer.ViewportHeight / 2);

			double scaleChange = e.Delta > 0 ? 1.125 : 1 / 1.125;
			// 1% scale is the minimum
			if (Scale == 0.01 && scaleChange < 1.0) {
				e.Handled = true;
				return;
			}
			double oldScale = Scale;
			double newScale = oldScale * scaleChange;
			if (newScale < 0.01) {
				newScale = 0.01;
				scaleChange = newScale / oldScale;
			}
			else if ((oldScale < 1 && newScale > 1) || (oldScale > 1 && newScale < 1)) {
				newScale = 1;
				scaleChange = newScale / oldScale;
			}

			supressEvents = true;
			Scale = newScale;
			supressEvents = false;

			Vector afterOffset = CalculateAreaOffset();
			Vector afterArea = new Vector(spritePoint.X, spritePoint.Y) * newScale + afterOffset;

			PART_ScaleTransform.ScaleX = Scale;
			PART_ScaleTransform.ScaleY = Scale;

			PART_SpriteImage.Margin = CalculateAreaMargins();
			PART_GridLines.Margin = PART_SpriteImage.Margin;

			Vector newScroll = afterArea - beforeCenter;
			if (PART_ScrollViewer.ViewportWidth != 0 && PART_ScrollViewer.ViewportHeight != 0) {
				PART_ScrollViewer.ScrollToHorizontalOffset(newScroll.X - PART_ScrollViewer.ViewportWidth / 2);
				PART_ScrollViewer.ScrollToVerticalOffset(newScroll.Y - PART_ScrollViewer.ViewportHeight / 2);
			}

			e.Handled = true;
		}
		private void OnScrollChanged(object sender, ScrollChangedEventArgs e) {
			Thickness margin = new Thickness();
			if (PART_ScrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible)
				margin.Right = 1;
			if (PART_ScrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
				margin.Bottom = 1;
			PART_ScrollViewer.Padding = margin;
			PART_ScrollViewer.Margin = margin;
			PART_SpriteImage.Margin = CalculateAreaMargins();
			PART_GridLines.Margin = PART_SpriteImage.Margin;
			if (Centered)
				UpdateCentered();
			UpdateLines();
		}

		#endregion

		#region Private Methods

		private void CenterToggled() {
			if (!templateApplied || supressEvents) return;

			Vector normalizedScroll = CalculateNormalizedScrollCenter(true);
			Vector newScroll = CalculateAreaCenter();
			if (Centered) {
				savedScale = Scale;
				savedNormalizedScroll = normalizedScroll;
				if (!IsEmpty) {
					PART_ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
					PART_ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
				}
				UpdateCentered();
			}
			else {
				supressEvents = true;
				Scale = savedScale;
				supressEvents = false;
				if (!IsEmpty) {
					PART_ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
					PART_ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
				}
				PART_ScaleTransform.ScaleX = Scale;
				PART_ScaleTransform.ScaleY = Scale;
				PART_SpriteImage.Margin = CalculateAreaMargins();
				PART_GridLines.Margin = PART_SpriteImage.Margin;
				if (savedNormalizedScroll.X >= 0 && savedNormalizedScroll.Y >= 0) {
					Vector areaCenter = CalculateAreaCenter();
					newScroll.X = savedNormalizedScroll.X * areaCenter.X;
					newScroll.Y = savedNormalizedScroll.Y * areaCenter.Y;
				}
				if (PART_ScrollViewer.ViewportWidth != 0 && PART_ScrollViewer.ViewportHeight != 0) {
					PART_ScrollViewer.ScrollToHorizontalOffset(newScroll.X - PART_ScrollViewer.ViewportWidth / 2);
					PART_ScrollViewer.ScrollToVerticalOffset(newScroll.Y - PART_ScrollViewer.ViewportHeight / 2);
				}
			}
		}

		private void UpdatePartChanges() {
			if (!templateApplied) return;

			Vector normalized = CalculateNormalizedScrollCenter();
			UpdateExpand();
			if (Centered) {
				UpdateCentered();
			}
			else {
				Vector areaCenter = CalculateAreaCenter();
				Vector newScroll = new Vector(
					normalized.X * areaCenter.X,
					normalized.Y * areaCenter.Y);
				PART_ScaleTransform.ScaleX = Scale;
				PART_ScaleTransform.ScaleY = Scale;
				if (PART_ScrollViewer.ViewportWidth != 0 && PART_ScrollViewer.ViewportHeight != 0) {
					PART_ScrollViewer.ScrollToHorizontalOffset(newScroll.X - PART_ScrollViewer.ViewportWidth / 2);
					PART_ScrollViewer.ScrollToVerticalOffset(newScroll.Y - PART_ScrollViewer.ViewportHeight / 2);
				}
			}
			UpdateLines();
		}


		private Vector CalculateAreaCenter(bool noCentered = false) {
			return new Vector(validSpriteSize.Width, validSpriteSize.Height) * Scale / 2 +
				CalculateAreaOffset(noCentered);
		}
		private Vector CalculateScrollCenter() {
			return new Vector(
				PART_ScrollViewer.HorizontalOffset + PART_ScrollViewer.ViewportWidth / 2,
				PART_ScrollViewer.VerticalOffset + PART_ScrollViewer.ViewportHeight / 2);
		}
		private Vector CalculateNormalizedScrollCenter(bool noCentered = false) {
			Vector areaCenter = CalculateAreaCenter(noCentered);
			Vector scrollCenter = CalculateScrollCenter();
			return new Vector(
				scrollCenter.X / areaCenter.X,
				scrollCenter.Y / areaCenter.Y);
		}

		private Vector CalculateAreaOffset(bool noCentered = false) {
			if (Centered && !noCentered)
				return new Vector(15, 15);
			double viewWidth = PART_ScrollViewer.ViewportWidth;
			double viewHeight = PART_ScrollViewer.ViewportHeight;
			double scaledWidth = validSpriteSize.Width  * Scale;
			double scaledHeight = validSpriteSize.Height * Scale;
			double x, y;
			if (scaledWidth > viewWidth)
				x = viewWidth / 2;
			else
				x = viewWidth - scaledWidth / 2;
			if (scaledHeight > viewHeight)
				y = viewHeight / 2;
			else
				y = viewHeight - scaledHeight / 2;
			return new Vector(x, y);
		}
		private Thickness CalculateAreaMargins() {
			var offset = CalculateAreaOffset();
			return new Thickness(offset.X, offset.Y, offset.X, offset.Y);
		}


		private void UpdateCentered() {
			if (IsEmpty) {
				supressEvents = true;
				Scale = 1;
				supressEvents = false;
				PART_ScaleTransform.ScaleX = Scale;
				PART_ScaleTransform.ScaleY = Scale;
				PART_SpriteImage.Margin = new Thickness();
				PART_GridLines.Margin = PART_SpriteImage.Margin;
			}
			else {
				Vector areaOffset = CalculateAreaOffset();
				Vector area = new Vector(
					PART_ScrollViewer.ViewportWidth,
					PART_ScrollViewer.ViewportHeight) - areaOffset * 2;
				double areaRatio = area.X / area.Y;
				double spriteRatio = validSpriteSize.Width / validSpriteSize.Height;
				supressEvents = true;
				if (areaRatio > spriteRatio) {
					Scale = Math.Min(1, area.Y / validSpriteSize.Height);
				}
				else {
					Scale = Math.Min(1, area.X / validSpriteSize.Width);
				}
				supressEvents = false;
				areaOffset.X = (PART_ScrollViewer.ViewportWidth - validSpriteSize.Width * Scale) / 2;
				areaOffset.Y = (PART_ScrollViewer.ViewportHeight - validSpriteSize.Height * Scale) / 2;
				PART_SpriteImage.Margin = new Thickness(areaOffset.X, areaOffset.Y, areaOffset.X, areaOffset.Y);
				PART_GridLines.Margin = PART_SpriteImage.Margin;
				PART_ScaleTransform.ScaleX = Scale;
				PART_ScaleTransform.ScaleY = Scale;
			}
		}

		private void UpdateExpand() {
			if (!templateApplied) return;
			
			PART_SpriteImage.Margin = CalculateAreaMargins();
			PART_GridLines.Margin = PART_SpriteImage.Margin;
			UpdateLines();
		}

		private void UpdateLines() {
			if (!templateApplied) return;

			PART_GridLineCenter.Margin = new Thickness(SpriteOrigin.X * Scale, 0, 0, 0);
			PART_GridLineBaseline.Margin = new Thickness(0, SpriteOrigin.Y * Scale, 0, 0);
		}

		private void UpdateEmptyChanged() {
			if (!templateApplied) return;
			if (IsEmpty) {
				PART_SpriteImage.Width = 0;
				PART_SpriteImage.Height = 0;
				PART_GridLines.Visibility = Visibility.Hidden;
			}
			else {
				PART_SpriteImage.Width = double.NaN;
				PART_SpriteImage.Height = double.NaN;
				PART_GridLines.Visibility = Visibility.Visible;
			}
			if (IsEmpty || Centered) {
				PART_ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
				PART_ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
				if (Centered)
					UpdateCentered();
			}
			else {
				PART_ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
				PART_ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
			}
		}

		#endregion
	}
}
