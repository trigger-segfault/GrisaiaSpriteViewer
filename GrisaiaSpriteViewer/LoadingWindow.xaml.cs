using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Grisaia.Categories;
using Grisaia.Mvvm.Model;
using Grisaia.Mvvm.ViewModel;

namespace Grisaia.SpriteViewer {
	/// <summary>
	///  Interaction logic for LoadingWindow.xaml
	/// </summary>
	public partial class LoadingWindow : Window {
		#region Properties
		
		public LoadingViewModel ViewModel => (LoadingViewModel) DataContext;

		#endregion

		#region Static Constructors

		static LoadingWindow() {
			DataContextProperty.AddOwner(typeof(LoadingWindow),
				new FrameworkPropertyMetadata(
					OnDataContextChanged));
		}

		#endregion

		#region Constructors

		public LoadingWindow() {
			InitializeComponent();
		}

		#endregion

		#region Event Handlers

		private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LoadingWindow window = (LoadingWindow) d;
			window.ViewModel.WindowOwner = window;
		}
		private void OnClosed(object sender, EventArgs e) {
			ViewModel.WindowOwner = null;
		}

		#endregion
	}
}
