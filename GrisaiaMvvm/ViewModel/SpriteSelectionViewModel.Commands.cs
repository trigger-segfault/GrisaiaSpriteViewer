using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Grisaia.Mvvm.Commands;
using Microsoft.Win32;

namespace Grisaia.Mvvm.ViewModel {
	partial class SpriteSelectionViewModel {

		public IRelayCommand ToggleCenterSprite => GetCommand(OnToggleCenterSprite);
		public IRelayCommand SaveSprite => GetCommand(OnSaveSprite, CanExecuteExportSprite);
		public IRelayCommand CopySprite => GetCommand(OnCopySprite, CanExecuteExportSprite);


		private void OnToggleCenterSprite() {
			Centered = !Centered;
		}
		private bool CanExecuteExportSprite() {
			return !SpriteDrawInfo.IsNone;
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
