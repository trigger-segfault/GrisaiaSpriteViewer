using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Categories;
using Grisaia.Mvvm.Commands;
using Grisaia.Mvvm.Model;
using Grisaia.Mvvm.Services;

namespace Grisaia.Mvvm.ViewModel {
	partial class InstallDirsViewModel {
		public IRelayCommand OK => GetCommand(OnOK);
		public IRelayCommand Cancel => GetCommand(OnCancel);


		private void OnOK() {
			Dictionary<string, GameInstallInfo> newGames = new Dictionary<string, GameInstallInfo>();
			foreach (InstallDirsGameViewModel game in games) {
				newGames.Add(game.GameId, game.CustomInstall);
			}
			Settings.CustomGameInstalls = new ReadOnlyDictionary<string, GameInstallInfo>(newGames);
			GrisaiaDatabase.SaveSettings();
		}
		private void OnCancel() {
			//WindowOwner.Close();
		}
	}
}
