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
			string newPath = Dialogs.ShowFolderBrowser(WindowOwner, "Select an install directory for this game",
														false, CustomInstallDir ?? string.Empty);
			if (newPath != null) {
				CustomInstallDir = newPath;
			}
		}
		private void OnRemoveCustomLocation() {
			CustomInstallDir = null;
		}
		private void OnOpenCustomExecutable() {

		}
		private void OnRemoveCustomExecutable() {
			CustomExecutable = null;
		}
		private void OnRelocateGame() {

		}
	}
}
