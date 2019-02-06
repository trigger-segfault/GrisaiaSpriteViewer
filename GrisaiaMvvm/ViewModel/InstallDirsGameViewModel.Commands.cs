using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Grisaia.Categories;
using Grisaia.Mvvm.Commands;

namespace Grisaia.Mvvm.ViewModel {
	partial class InstallDirsGameViewModel {

		public IRelayCommand OpenCustomLocation => GetCommand(OnOpenCustomLocation);
		public IRelayCommand RemoveCustomLocation => GetCommand(OnRemoveCustomLocation, CanExecuteHasCustomLocation);
		public IRelayCommand OpenCustomExecutable => GetCommand(OnOpenCustomExecutable, CanExecuteHasCustomLocation);
		public IRelayCommand RemoveCustomExecutable => GetCommand(OnRemoveCustomExecutable, CanExecuteHasCustomExecutable);
		public IRelayCommand RelocateGame => GetCommand(OnRelocateGame);

		private bool CanExecuteHasCustomLocation() {
			return CustomInstallDir != null;
		}
		private bool CanExecuteHasCustomExecutable() {
			return CustomExecutable != null;
		}
		private void OnOpenCustomLocation() {
			string description = "Select an install directory for this game";
			string initialDirectory = CustomInstallDir ?? string.Empty;
			string filePath = Dialogs.ShowFolderBrowser(WindowOwner, description, false, initialDirectory);
			if (filePath != null) {
				CustomInstallDir = filePath;
			}
		}
		private void OnRemoveCustomLocation() {
			CustomInstallDir = null;
		}
		private void OnOpenCustomExecutable() {
			string initialDirectory = CustomInstallDir;
			string fileName = CustomExecutable ?? string.Empty;
			string filePath = Dialogs.ShowOpenExeDialog(WindowOwner, fileName, initialDirectory);
			if (filePath != null) {
				CustomExecutable = filePath;
			}
		}
		private void OnRemoveCustomExecutable() {
			CustomExecutable = null;
		}
		private void OnRelocateGame() {

		}
	}
}
