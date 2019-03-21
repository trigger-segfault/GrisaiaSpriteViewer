using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;
using TriggersTools.Build;

namespace Grisaia.SpriteViewer.Windows {
	/// <summary>
	///  The window showing information about the program.
	/// </summary>
	public partial class AboutWindow : Window {
		#region Constructors

		/// <summary>
		///  Constructs the about window.
		/// </summary>
		private AboutWindow() {
			InitializeComponent();

			Assembly assembly = Assembly.GetEntryAssembly();
			AssemblyName assemblyName = assembly.GetName();
			DateTime buildDate = assembly.GetBuildTime();

			this.labelVersion.Content = assemblyName.Version.ToString() + " Prerelease";
			this.labelBuildDate.Content = buildDate.ToShortDateString() + " (" + buildDate.ToShortTimeString() + ")";
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
			AboutWindow window = new AboutWindow();
			if (owner == null)
				window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			else
				window.Owner = owner;
			window.ShowDialog();
		}

		#endregion
	}
}
