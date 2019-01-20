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
	/// Interaction logic for LoadingWindow.xaml
	/// </summary>
	public partial class LoadingWindow : Window {

		private MainWindow mainWindow;

		public LoadingWindow() {
			InitializeComponent();
			mainWindow = new MainWindow();
		}

		private void OnLoaded(object sender, RoutedEventArgs e) {
			var task = Task.Run(() => {
				mainWindow.Initialize(UpdateProgress);
				Dispatcher.Invoke(() => {
					Close();
					mainWindow.Show();
				});
			}).ConfigureAwait(false);
		}

		private void UpdateProgress(string status, string game, double progress) {
			Dispatcher.Invoke(() => {
				labelStatus.Content = status;
				if (game != null)
					labelGame.Content = $"Current Game: {game}";
				else
					labelGame.Content = string.Empty;
				progressBar.Value = progress;
			});
		}

		private void OnContentRendered(object sender, EventArgs e) {
		}
	}
}
