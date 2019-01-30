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
using Grisaia.Asmodean;
using Grisaia.Categories;
using Grisaia.SpriteViewer.Model;
using Grisaia.SpriteViewer.ViewModel;

namespace Grisaia.SpriteViewer {
	/// <summary>
	/// Interaction logic for LoadingWindow.xaml
	/// </summary>
	public partial class LoadingWindow : Window {
		#region Fields

		private SpriteSelectionWindow mainWindow;

		private DispatcherTimer timeLabelTimer;
		private bool supressEvents = false;
		private Stopwatch watch;

		#endregion

		#region Properties
		
		public LoadingViewModel ViewModel => (LoadingViewModel) DataContext;

		#endregion

		public LoadingWindow() {
			InitializeComponent();
		}

		private void OnLoaded(object sender, RoutedEventArgs e) {
			GrisaiaModel grisaiaDb = ViewModel.GrisaiaDatabase;

			watch = Stopwatch.StartNew();
			timeLabelTimer = new DispatcherTimer(
				TimeSpan.FromSeconds(1),
				DispatcherPriority.Normal,
				OnUpdateTimer,
				Dispatcher);

			var task = Task.Run(() => {
				grisaiaDb.GameDatabase.LocateGames();
				grisaiaDb.GameDatabase.LoadCache(grisaiaDb.Settings.LoadUpdateArchives, OnLoadCacheProgress);
				grisaiaDb.SpriteDatabase.LoadSprites(grisaiaDb.Settings.SpriteCategoryOrder, OnLoadSpritesProgress);
				Dispatcher.Invoke(() => {
					timeLabelTimer.Stop();
					watch = null;
					var window = new SpriteSelectionWindow();
					Close();
					window.Show();
				});
			}).ConfigureAwait(false);
		}

		private void OnUpdateTimer(object sender, EventArgs e) {
			labelTime.Content = watch.Elapsed.ToString(@"mm\:ss");
		}

		private void OnLoadCacheProgress(LoadCacheProgressArgs e) {
			Dispatcher.Invoke(() => {
				KifintProgressArgs k = e.Kifint;
				if (e.IsDone) {
					labelStatus.Content = "Finishing Loading Archives!";
					labelEntries.Content = $"Entries: -- / --";
					labelGame.Content = $"Game: --";
				}
				else {
					if (e.IsBuilding) {
						if (k.EntryCount == 0 || k.IsDone)
							return; // This is a pointless status update, skip it. Likely no update KIFINT archives.
						string archiveCount = string.Empty;
						//if (k.ArchiveCount > 1)
						//	archiveCount = $" [{(k.ArchiveIndex+1):N0} / {k.ArchiveCount:N0}]";
						labelStatus.Content = $"Building {k.ArchiveType} Cache: \"{k.ArchiveName}\"{archiveCount}";
						labelEntries.Content = $"Entries: {k.EntryIndex:N0} / {k.EntryCount:N0}";
					}
					else {
						labelStatus.Content = $"Loading {k.ArchiveType} Archives...";
						labelEntries.Content = $"Entries: -- / --";
					}
					string gameName = e.CurrentGame.JPName;
					if (ViewModel.Settings.GameNamingScheme.UseEnglishName)
						gameName = e.CurrentGame.ENName;
					string gameCount = string.Empty;
					//if (e.GameCount > 1)
					//	gameCount = $" [{(e.GameIndex+1)} / {e.GameCount}]";
					labelGame.Content = $"Game: {gameName}{gameCount}";
				}
				
				if (e.IsBuilding || e.IsDone)
					minorProgressBar.Value = e.MinorProgress;
				else
					minorProgressBar.Value = 0d;
				majorProgressBar.Value = e.MajorProgress;
				//labelTime.Content = watch.Elapsed.ToString(@"mm\:ss");
			});
			if (e.IsDone) {
				Thread.Sleep(100);
			}
		}
		private void OnLoadSpritesProgress(LoadSpritesProgressArgs e) {
			Dispatcher.Invoke(() => {
				if (e.IsDone) {
					labelStatus.Content = "Finished Categorizing Sprites!";
					labelEntries.Content = $"Entries: -- / --";
					labelGame.Content = $"Game: --";
				}
				else {
					labelStatus.Content = $"Categorizing Sprites: {e.SpriteCount:N0}";
					labelEntries.Content = $"Entries: {e.EntryIndex:N0} / {e.EntryCount:N0}";
					string gameName = e.CurrentGame.JPName;
					if (ViewModel.Settings.GameNamingScheme.UseEnglishName)
						gameName = e.CurrentGame.ENName;
					string gameCount = string.Empty;
					//if (e.GameCount > 1)
					//	gameCount = $" [{(e.GameIndex+1)} / {e.GameCount}]";
					labelGame.Content = $"Game: {gameName}{gameCount}";
				}
				minorProgressBar.Value = e.MinorProgress;
				majorProgressBar.Value = e.MajorProgress;
				//labelTime.Content = watch.Elapsed.ToString(@"mm\:ss");
			});
			if (e.IsDone) {
				Thread.Sleep(200);
			}
		}
	}
}
