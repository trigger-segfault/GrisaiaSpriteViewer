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
		public IRelayCommand ToggleCenterSprite => GetCommand(OnToggleCenterSprite);
		public IRelayCommand ToggleGuideLines => GetCommand(OnToggleGuideLines);
		public IRelayCommand ToggleExpand => GetCommand(OnToggleExpand);
		public IRelayCommand Exit => GetCommand(OnExit);


		private bool CanExecuteExportSprite() {
			return !SpriteDrawInfo.IsNone;
		}

		private void OnExit() {
			UI.Shutdown();
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
			//Window window = Dialogs.CreateInstallDirsDialog();
			//window.Owner = WindowOwner;
			//window.ShowDialog();
		}
		private void OnSaveSprite() {
			if (!SpriteDrawInfo.IsNone) {
				SaveFileDialog dialog = new SaveFileDialog {
					FileName = Path.ChangeExtension(SpriteUniqueId, ".png"),
					Filter = "PNG Images|*.png",
					OverwritePrompt = true,
					AddExtension = true,
					InitialDirectory = Path.Combine(AppContext.BaseDirectory, "saved"),
				};
				if (!Directory.Exists(dialog.InitialDirectory))
					Directory.CreateDirectory(dialog.InitialDirectory);
				bool result = dialog.ShowDialog() ?? false;
				if (!result)
					return;
				using (var bitmap = SpriteDatabase.DrawSprite(SpriteDrawInfo))
					bitmap.Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
			}
		}
		private void OnCopySprite() {
			if (!SpriteDrawInfo.IsNone) {
				using (var bitmaps = SpriteDatabase.DrawSprite(SpriteDrawInfo, Color.Transparent, Color.White)) {
					string tempPath = Path.Combine(AppContext.BaseDirectory, "cache", "clipboard.png");
					bitmaps[0].Save(tempPath, ImageFormat.Png);
					Clipboard.SetImage(bitmaps[0], bitmaps[1], tempPath);
				}
			}
		}
	}
}
