using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Grisaia.SpriteViewer {
	/// <summary>
	/// Interaction logic for InstallDirDialog.xaml
	/// </summary>
	public partial class InstallDirsDialog : Window {
		#region Properties

		//public InstallDirViewModel ViewModel => (InstallDirViewModel) DataContext;

		#endregion

		#region Static Constructor

		static InstallDirsDialog() {
			//DataContextProperty.AddOwner(typeof(InstallDirDialog),
			//	new FrameworkPropertyMetadata(
			//		OnDataContextChanged));
		}

		#endregion

		#region Constructors

		public InstallDirsDialog() {
			InitializeComponent();
		}

		#endregion

		#region Event Handlers

		private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			InstallDirsDialog window = (InstallDirsDialog) d;
			//window.ViewModel.WindowOwner = window;
		}
		private void OnClosed(object sender, EventArgs e) {
			//ViewModel.WindowOwner = null;
		}
		private void OnOK(object sender, RoutedEventArgs e) {
			DialogResult = true;
		}
		private void OnCancel(object sender, RoutedEventArgs e) {
			Close();
		}

		#endregion
	}
}
