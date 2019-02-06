using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Grisaia.SpriteViewer.Windows {
	/// <summary>
	/// Interaction logic for CreditsWindow.xaml
	/// </summary>
	public partial class CreditsWindow : Window {
		#region Constructors

		/// <summary>
		///  Constructs the credits window.
		/// </summary>
		private CreditsWindow() {
			InitializeComponent();
		}

		#endregion
		
		#region Event Handlers

		private void OnRequestNavigate(object sender, RequestNavigateEventArgs e) {
			Process.Start((sender as Hyperlink).NavigateUri.ToString());
		}

		#endregion

		#region Show

		/// <summary>
		///  Shows the about window as a dialog with the optional window owner.
		/// </summary>
		/// <param name="owner">The optional owner of the window.</param>
		public static void Show(Window owner) {
			CreditsWindow window = new CreditsWindow();
			if (owner == null)
				window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			else
				window.Owner = owner;
			window.ShowDialog();
		}

		#endregion
	}
}
