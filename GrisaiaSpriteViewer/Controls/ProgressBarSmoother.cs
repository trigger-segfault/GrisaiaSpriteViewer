using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Grisaia.SpriteViewer.Controls {
	/// <summary>
	///  A static class for attached smooth value dependency properties.
	/// </summary>
	/// <remarks>
	///  <a href=""https://stackoverflow.com/a/19848684/7517185 />
	/// </remarks>
	public class ProgressBarSmoother {
		#region Dependency Properties

		/// <summary>
		///  The property for settings the value of the progress bar in a smooth motion.
		/// </summary>
		public static readonly DependencyProperty SmoothValueProperty =
			DependencyProperty.RegisterAttached(
				"SmoothValue",
				typeof(double),
				typeof(ProgressBarSmoother),
				new PropertyMetadata(
					0.0,
					OnSmoothValueChanged));
		/// <summary>
		///  The property for the duration of the progress bar animation. The default is 250ms.
		/// </summary>
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.RegisterAttached(
				"Duration",
				typeof(double),
				typeof(ProgressBarSmoother),
				new PropertyMetadata(
					250d));
		/// <summary>
		///  The property for if the progress bar can go smoothly in reverse. Otherwise changes in reverse will be
		///  instant.
		/// </summary>
		public static readonly DependencyProperty AllowReverseProperty =
			DependencyProperty.RegisterAttached(
				"AllowReverse",
				typeof(bool),
				typeof(ProgressBarSmoother),
				new PropertyMetadata(
					false));

		/// <summary>
		///  Raised with the smooth value is changed to apply animation.
		/// </summary>
		private static void OnSmoothValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (!(d is ProgressBar progressBar)) return;
			double oldValue = (double) e.OldValue;
			double newValue = (double) e.NewValue;
			if (newValue < oldValue && !GetAllowReverse(d)) {
				progressBar.Value = newValue;
			}
			else {
				TimeSpan duration = TimeSpan.FromMilliseconds(GetDuration(d));
				DoubleAnimation anim = new DoubleAnimation(oldValue, newValue, duration);
				progressBar?.BeginAnimation(ProgressBar.ValueProperty, anim, HandoffBehavior.Compose);
			}
		}

		/// <summary>
		///  Gets the smooth transition value of the progress bar.
		/// </summary>
		/// <param name="d">The dependency object which should be a progress bar.</param>
		/// <returns>The smooth value of the dependency object.</returns>
		public static double GetSmoothValue(DependencyObject d) {
			return (double) d.GetValue(SmoothValueProperty);
		}
		/// <summary>
		///  Sets the smooth transition value for the progress bar.
		/// </summary>
		/// <param name="d">The dependency object which should be a progress bar.</param>
		/// <param name="value">The new value of the property.</param>
		public static void SetSmoothValue(DependencyObject d, double value) {
			d.SetValue(SmoothValueProperty, value);
		}
		/// <summary>
		///  Gets the smooth transition duration of the progress bar.
		/// </summary>
		/// <param name="d">The dependency object which should be a progress bar.</param>
		/// <returns>The smooth duration of the dependency object.</returns>
		public static double GetDuration(DependencyObject d) {
			return (double) d.GetValue(DurationProperty);
		}
		/// <summary>
		///  Sets the smooth transition duration for the progress bar.
		/// </summary>
		/// <param name="d">The dependency object which should be a progress bar.</param>
		/// <param name="value">The new value of the property.</param>
		public static void SetDuration(DependencyObject d, double value) {
			d.SetValue(DurationProperty, value);
		}
		/// <summary>
		///  Gets if the smooth transition allows reversing animation for the progress bar.
		/// </summary>
		/// <param name="d">The dependency object which should be a progress bar.</param>
		/// <returns>The smooth allowation of reverse animation of the dependency object.</returns>
		public static bool GetAllowReverse(DependencyObject d) {
			return (bool) d.GetValue(AllowReverseProperty);
		}
		/// <summary>
		///  Sets if the smooth transition allows reversing animation for the progress bar.
		/// </summary>
		/// <param name="d">The dependency object which should be a progress bar.</param>
		/// <param name="value">The new value of the property.</param>
		public static void SetAllowReverse(DependencyObject d, TimeSpan value) {
			d.SetValue(AllowReverseProperty, value);
		}

		#endregion
	}
}
