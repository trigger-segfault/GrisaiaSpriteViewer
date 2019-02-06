using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using Grisaia.Categories;
using Grisaia.Mvvm.Services;

namespace Grisaia.Mvvm.ViewModel {
	public sealed partial class InstallDirsGameViewModel : ViewModelRelayCommand {
		#region Fields

		/// <summary>
		///  The custom install info for the game.
		/// </summary>
		private GameInstallInfo customInstall;

		private bool? customInstallValidated;

		#endregion

		#region Properties

		/// <summary>
		///  Gets the game info associated with this game install.
		/// </summary>
		public GameInfo GameInfo { get; }
		/// <summary>
		///  Gets the game Id associated with this game install.
		/// </summary>
		public string GameId => GameInfo.Id;
		/// <summary>
		///  Gets the optional custom installation directory for the game.
		/// </summary>
		public string LocatedInstallDir => GameInfo.LocatedInstall.Directory;
		/// <summary>
		///  Gets the optional custom installation directory for the game.
		/// </summary>
		public string CustomInstallDir {
			get => customInstall.Directory;
			set {
				if (string.IsNullOrWhiteSpace(value))
					value = null;
				if (customInstall.Directory != value) {
					customInstall.Directory = value;
					RaisePropertyChanged();
					RemoveCustomLocation.RaiseCanExecuteChanged();
					OpenCustomExecutable.RaiseCanExecuteChanged();
					Validate();
				}
			}
		}
		/// <summary>
		///  Gets the optional custom executable name for the game.
		/// </summary>
		public string CustomExecutable {
			get => customInstall.Executable;
			set {
				if (string.IsNullOrWhiteSpace(value))
					value = null;
				if (customInstall.Executable != value) {
					customInstall.Executable = value;
					RaisePropertyChanged();
					RemoveCustomExecutable.RaiseCanExecuteChanged();
					Validate();
				}
			}
		}
		/// <summary>
		///  Gets or sets if the custom installation location has been validated as usable.
		/// </summary>
		public bool? IsCustomInstallValidated {
			get => customInstallValidated;
			set {
				if (Set(ref customInstallValidated, value)) {

				}
			}
		}
		/// <summary>
		///  Gets the custom game installation information.
		/// </summary>
		public GameInstallInfo CustomInstall => customInstall;

		public IGrisaiaDialogService Dialogs => ViewModel.Dialogs;

		public Window WindowOwner => ViewModel.WindowOwner;

		public InstallDirsViewModel ViewModel { get; }

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs the game install item view model with the specified game and uses the existin custom install
		///  information.
		/// </summary>
		/// <param name="game">The game info to use.</param>
		public InstallDirsGameViewModel(IRelayCommandFactory relayFactory,
										InstallDirsViewModel viewModel,
										GameInfo game)
			: base(relayFactory)
		{
			GameInfo = game;
			ViewModel = viewModel;
			customInstall = game.CustomInstall;
			Validate();
		}

		#endregion

		#region Validate

		public void Validate() {
			if (customInstall.Directory == null)
				IsCustomInstallValidated = null;
			else
				IsCustomInstallValidated = GameInfo.ValidateCustomInstall(customInstall);
		}

		#endregion
	}
}
