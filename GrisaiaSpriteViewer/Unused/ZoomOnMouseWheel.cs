using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Grisaia.SpriteViewer {
	/// <summary>
	/// https://stackoverflow.com/a/46427503/7517185
	/// </summary>
	public class ZoomOnMouseWheel : Behavior<FrameworkElement> {
		public Key? ModifierKey { get; set; } = null;
		public TransformMode TranformMode { get; set; } = TransformMode.Render;

		private Transform _transform;

		protected override void OnAttached() {
			if (TranformMode == TransformMode.Render)
				_transform = AssociatedObject.RenderTransform = new MatrixTransform();
			else
				_transform = AssociatedObject.LayoutTransform = new MatrixTransform();

			AssociatedObject.MouseWheel += AssociatedObject_MouseWheel;
		}

		protected override void OnDetaching() {
			AssociatedObject.MouseWheel -= AssociatedObject_MouseWheel;
		}

		private void AssociatedObject_MouseWheel(object sender, MouseWheelEventArgs e) {
			if ((!ModifierKey.HasValue || !Keyboard.IsKeyDown(ModifierKey.Value)) && ModifierKey.HasValue)
				return;

			if (!(_transform is MatrixTransform transform))
				return;

			Point pos1 = e.GetPosition(AssociatedObject);
			double scale = e.Delta > 0 ? 1.125 : 1 / 1.125;
			Matrix mat = transform.Matrix;
			mat.ScaleAt(scale, scale, pos1.X, pos1.Y);
			transform.Matrix = mat;
			e.Handled = true;
		}
	}

	public enum TransformMode {
		Layout,
		Render,
	}
}
