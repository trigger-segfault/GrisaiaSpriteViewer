using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using Grisaia.Mvvm.Commands;
using Microsoft.Win32;

namespace Grisaia.Mvvm.ViewModel {
	partial class SpriteSelectionViewModel {

		public IRelayCommand SaveSprite => GetCommand(OnSaveSprite, CanExecuteExportSprite);
		public IRelayCommand CopySprite => GetCommand(OnCopySprite, CanExecuteExportSprite);
		public IRelayCommand OpenSettings => GetCommand(OnOpenSettings);
		public IRelayCommand OpenInstallDirs => GetCommand(OnOpenInstallDirs);
		public IRelayCommand OpenAbout => GetCommand(OnOpenAbout);
		public IRelayCommand OpenCredits => GetCommand(OnOpenCredits);
		public IRelayCommand OpenHotkeys => GetCommand(OnOpenHotkeys);
		public IRelayCommand ToggleSpritePartList => GetCommand(OnToggleSpritePartList);
		public IRelayCommand ToggleCenterSprite => GetCommand(OnToggleCenterSprite);
		public IRelayCommand ToggleGuideLines => GetCommand(OnToggleGuideLines);
		public IRelayCommand ToggleExpand => GetCommand(OnToggleExpand);
		public IRelayCommand Exit => GetCommand(OnExit);


		private bool CanExecuteExportSprite() {
			return !SpriteDrawInfo.IsNone;
		}

		private void OnOpenAbout() {
			Dialogs.ShowAboutWindow(WindowOwner);
		}
		private void OnOpenCredits() {
			Dialogs.ShowCreditsWindow(WindowOwner);
		}
		private void OnOpenHotkeys() {
			Dialogs.ShowHotkeysWindow(WindowOwner);
		}
		private void OnExit() {
			UI.Shutdown();
		}
		private void OnToggleSpritePartList() {
			ShowSpritePartList = !ShowSpritePartList;
		}
		private void OnToggleCenterSprite() {
			Centered = !Centered;
		}
		private void OnToggleGuideLines() {
			ShowGuideLines = !ShowGuideLines;
		}
		private void OnToggleExpand() {
			Expand = !Expand;
		}
		private void EmptyDelegate() { }
		private void OnOpenSettings() {
			Dialogs.ShowSettingsDialog(WindowOwner);
		}
		private void OnOpenInstallDirs() {
			Dialogs.ShowInstallDirsDialog(WindowOwner);
		}
		private void OnSaveSprite() {
			if (!SpriteDrawInfo.IsNone) {
				string initialDirectory = Path.Combine(AppContext.BaseDirectory, "saved");
				string fileName = SpriteUniqueId + ".png";
				if (!Directory.Exists(initialDirectory))
					Directory.CreateDirectory(initialDirectory);

				string filePath = Dialogs.ShowSavePngDialog(WindowOwner, fileName, initialDirectory);
				if (filePath != null) {
					using (var bitmap = SpriteDatabase.DrawSprite(SpriteDrawInfo))
						bitmap.Save(filePath, ImageFormat.Png);
				}
			}
		}
		private void OnCopySprite() {
			if (!SpriteDrawInfo.IsNone) {
				using (var bitmaps = SpriteDatabase.DrawSprite(SpriteDrawInfo, Color.Transparent, Color.White)) {
					string tempPath = Path.Combine(GrisaiaDatabase.CachePath, "clipboard.png");
					bitmaps[0].Save(tempPath, ImageFormat.Png);
					Clipboard.SetImage(bitmaps[0], bitmaps[1], tempPath);
				}
			}
		}
	}
}
