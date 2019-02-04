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
using Grisaia.Mvvm.ViewModel;

namespace Grisaia.SpriteViewer {
	/// <summary>
	///  Interaction logic for SettingsDialog.xaml
	/// </summary>
	public partial class SettingsDialog : Window {
		#region Properties

		public SettingsViewModel ViewModel => (SettingsViewModel) DataContext;

		#endregion

		#region Static Constructor

		static SettingsDialog() {
			DataContextProperty.AddOwner(typeof(SettingsDialog),
				new FrameworkPropertyMetadata(
					OnDataContextChanged));
		}

		#endregion

		#region Constructors

		public SettingsDialog() {
			InitializeComponent();
		}

		#endregion

		#region Event Handlers

		private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SettingsDialog window = (SettingsDialog) d;
			window.ViewModel.WindowOwner = window;
		}
		private void OnClosed(object sender, EventArgs e) {
			ViewModel.WindowOwner = null;
		}

		#endregion

		private void OnOK(object sender, RoutedEventArgs e) {
			DialogResult = true;
		}

		private void OnCancel(object sender, RoutedEventArgs e) {
			Close();
		}

		private void FocusCharacterNameTypeList(object sender, RoutedEventArgs e) {
			listBoxCharacterNameType.Focus();
		}
		private void FocusPrimary(object sender, RoutedEventArgs e) {
			listBoxPrimary.Focus();
		}
		private void FocusSecondary(object sender, RoutedEventArgs e) {
			listBoxSecondary.Focus();
		}
	}
}
