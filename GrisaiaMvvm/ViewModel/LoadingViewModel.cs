using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Grisaia.Asmodean;
using Grisaia.Categories;
using Grisaia.Mvvm.Commands;
using Grisaia.Mvvm.Model;
using Grisaia.Mvvm.Services;
using Grisaia.Mvvm.ViewModel.Messages;

namespace Grisaia.Mvvm.ViewModel {
	public sealed class LoadingViewModel : ViewModelWindow {
		#region Constants

		private const string NoValue = "--";
		private const string NoValueOutOfValue = NoValue + " / " + NoValue;

		#endregion

		#region Fields

		private UITimer timeLabelTimer;
		private bool supressEvents = false;
		private Stopwatch watch;
		private TimeSpan ellapsed;
		private string mainStatus;
		private string entriesStatus;
		private string gameStatus;
		private double minorProgress;
		private double majorProgress;
		private OpenLoadingWindowAction loadedAction;

		#endregion

		#region Properties

		/// <summary>
		///  Gets the database for all Grisaia databases.
		/// </summary>
		public GrisaiaModel GrisaiaDatabase { get; }
		/// <summary>
		///  Gets the database for all Grisaia games.
		/// </summary>
		public GameDatabase GameDatabase => GrisaiaDatabase.GameDatabase;
		/// <summary>
		///  Gets the database for all known Grisaia characters.
		/// </summary>
		public CharacterDatabase CharacterDatabase => GrisaiaDatabase.CharacterDatabase;
		/// <summary>
		///  Gets the database for all located character sprites.
		/// </summary>
		public SpriteDatabase SpriteDatabase => GrisaiaDatabase.SpriteDatabase;
		/// <summary>
		///  Gets the program settings
		/// </summary>
		public SpriteViewerSettings Settings => GrisaiaDatabase.Settings;

		public IGrisaiaDialogService Dialogs { get; }
		public UIService UI { get; }

		public TimeSpan Ellapsed {
			get => ellapsed;
			set => Set(ref ellapsed, value);
		}
		public string MainStatus {
			get => mainStatus;
			set => Set(ref mainStatus, value);
		}
		public string EntriesStatus {
			get => entriesStatus;
			set => Set(ref entriesStatus, value);
		}
		public string GameStatus {
			get => gameStatus;
			set => Set(ref gameStatus, value);
		}
		public double MinorProgress {
			get => minorProgress;
			set => Set(ref minorProgress, value);
		}
		public double MajorProgress {
			get => majorProgress;
			set => Set(ref majorProgress, value);
		}

		#endregion

		#region Constructors

		public LoadingViewModel(IRelayCommandFactory relayFactory,
								GrisaiaModel grisaiaDb,
								IGrisaiaDialogService dialogs,
								UIService ui)
			: base(relayFactory)
		{
			Title = "Grisaia Extract Sprite Viewer";
			GrisaiaDatabase = grisaiaDb;
			Dialogs = dialogs;
			UI = ui;
			MessengerInstance.Register<OpenLoadingWindowMessage>(this, OnOpenLoadingWindow);
			if (IsInDesignMode) {
				Ellapsed = TimeSpan.FromSeconds(21);
				MainStatus = "Building Image Cache: \"image.int\"";
				EntriesStatus = "14000 / 74868";
				GameStatus = "Grisaia no Kajitsu";
				MinorProgress = 14000d / 74868d;
				MajorProgress = 1d / 8d;
			}
			else {
				Ellapsed = TimeSpan.Zero;
				MainStatus = "Preparing Grisaia Sprite Viewer...";
				EntriesStatus = NoValueOutOfValue;
				GameStatus = NoValue;
			}
		}

		#endregion

		#region Loaded Override

		public override void Loaded() {
			watch = Stopwatch.StartNew();
			Ellapsed = TimeSpan.Zero;
			MainStatus = "Preparing Grisaia Sprite Viewer...";
			EntriesStatus = NoValueOutOfValue;
			GameStatus = NoValue;
			MinorProgress = 0d;
			MajorProgress = 0d;
			timeLabelTimer = UI.StartTimer(TimeSpan.FromSeconds(1), true, OnUpdateTimer);

			var task = Task.Run(() => {
				switch (loadedAction) {
				case OpenLoadingWindowAction.Startup:
					GameDatabase.LocateGames(Settings.CustomGameInstalls);
					GameDatabase.LoadCache(Settings.LoadUpdateArchives, OnLoadCacheProgress);
					goto case OpenLoadingWindowAction.ReloadSprites;
				case OpenLoadingWindowAction.ReloadGames:
					GameDatabase.ReloadCache(Settings.LoadUpdateArchives, OnLoadCacheProgress);
					goto case OpenLoadingWindowAction.ReloadSprites;
				case OpenLoadingWindowAction.ReloadSprites:
					SpriteDatabase.LoadSprites(Settings.SpriteCategoryOrder, OnLoadSpritesProgress);
					break;
				}
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
				UI.Invoke(() => {
					timeLabelTimer.Stop();
					watch = null;
					if (loadedAction == OpenLoadingWindowAction.Startup)
						Dialogs.ShowSpriteSelectionWindow();
					WindowOwner.Close();
					timeLabelTimer?.Stop();
					timeLabelTimer = null;
				});
			}).ConfigureAwait(false);
		}

		#endregion

		#region Event Handlers

		private void OnOpenLoadingWindow(OpenLoadingWindowMessage msg) {
			loadedAction = msg.Action;
			bool dialog = msg.Action != OpenLoadingWindowAction.Startup;
			Dialogs.ShowLoadingWindow(dialog);
		}

		private void OnUpdateTimer() {
			Ellapsed = watch.Elapsed;
			RaisePropertyChanged(nameof(Ellapsed));
		}

		private void OnLoadCacheProgress(LoadCacheProgressArgs e) {
			UI.Invoke(() => {
				KifintProgressArgs k = e.Kifint;
				if (e.IsDone) {
					MainStatus = "Finishing Loading Archives!";
					EntriesStatus = NoValueOutOfValue;
					GameStatus = NoValue;
				}
				else {
					if (e.IsBuilding) {
						if (k.EntryCount == 0 || k.IsDone)
							return; // This is a pointless status update, skip it. Likely no update KIFINT archives.
						string archiveCount = string.Empty;
						//if (k.ArchiveCount > 1)
						//	archiveCount = $" [{(k.ArchiveIndex+1):N0} / {k.ArchiveCount:N0}]";
						MainStatus = $"Building {k.ArchiveType} Cache: \"{k.ArchiveName}\"{archiveCount}";
						EntriesStatus = $"{k.EntryIndex:N0} / {k.EntryCount:N0}";
					}
					else {
						MainStatus = $"Loading {k.ArchiveType} Archives...";
						EntriesStatus = NoValueOutOfValue;
					}
					string gameName = e.CurrentGame.JPName;
					if (Settings.GameNamingScheme.EnglishName)
						gameName = e.CurrentGame.ENName;
					string gameCount = string.Empty;
					//if (e.GameCount > 1)
					//	gameCount = $" [{(e.GameIndex+1)} / {e.GameCount}]";
					GameStatus = $"{gameName}{gameCount}";
				}

				if (e.IsBuilding || e.IsDone)
					MinorProgress = e.MinorProgress;
				else
					MinorProgress = 0d;
				MajorProgress = e.MajorProgress;
				//labelTime.Content = watch.Elapsed.ToString(@"mm\:ss");
			});
			if (e.IsDone) {
				Thread.Sleep(100);
			}
		}
		private void OnLoadSpritesProgress(LoadSpritesProgressArgs e) {
			UI.Invoke(() => {
				if (e.IsDone) {
					MainStatus = "Finished Categorizing Sprites!";
					EntriesStatus = NoValueOutOfValue;
					GameStatus = NoValue;
				}
				else {
					MainStatus = $"Categorizing Sprites: {e.SpriteCount:N0}";
					EntriesStatus = $"{e.EntryIndex:N0} / {e.EntryCount:N0}";
					string gameName = e.CurrentGame.JPName;
					if (Settings.GameNamingScheme.EnglishName)
						gameName = e.CurrentGame.ENName;
					string gameCount = string.Empty;
					//if (e.GameCount > 1)
					//	gameCount = $" [{(e.GameIndex+1)} / {e.GameCount}]";
					GameStatus = $"{gameName}{gameCount}";
				}
				MinorProgress = e.MinorProgress;
				MajorProgress = e.MajorProgress;
			});
			if (e.IsDone) {
				Thread.Sleep(200);
			}
		}

		#endregion
	}
}
