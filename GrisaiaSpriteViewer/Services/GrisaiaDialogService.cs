using System;
using System.IO;
using System.Media;
using System.Windows;
using Grisaia.Mvvm.Services;
using Grisaia.SpriteViewer.Windows;
using Microsoft.Win32;

namespace Grisaia.SpriteViewer.Services {
	/// <summary>
	///  The dialog service for the Grisaia program.
	/// </summary>
	public class GrisaiaDialogService : IGrisaiaDialogService {
		#region Show Main Windows

		/// <summary>
		///  Shows the settings dialog window.
		/// </summary>
		/// <param name="owner">The optional owner window to use for this window.</param>
		/// <returns>The dialog result of the window.</returns>
		public bool? ShowSettingsDialog(Window owner) {
			Window window = new SettingsDialog();
			if (owner != null)
				window.Owner = owner;
			else
				window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			return window.ShowDialog();
		}
		/// <summary>
		///  Shows the game locations dialog window.
		/// </summary>
		/// <param name="owner">The optional owner window to use for this window.</param>
		/// <returns>The dialog result of the window.</returns>
		public bool? ShowInstallDirsDialog(Window owner) {
			Window window = new InstallDirsDialog();
			if (owner != null)
				window.Owner = owner;
			else
				window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			return window.ShowDialog();
		}

		/// <summary>
		///  Shows the loading window either in normal or dialog mode.
		/// </summary>
		/// <param name="dialog">True if the window should be shown in dialog mode.</param>
		public void ShowLoadingWindow(bool asDialog) {
			Window window = new LoadingWindow();
			if (asDialog)
				window.ShowDialog();
			else
				window.Show();
		}
		/// <summary>
		///  Shows the sprite selection window.
		/// </summary>
		public void ShowSpriteSelectionWindow() {
			Window window = new SpriteSelectionWindow();
			window.Show();
		}
		/// <summary>
		///  Shows the window with information about the program.
		/// </summary>
		public void ShowAboutWindow(Window owner) {
			AboutWindow.Show(owner);
		}
		/// <summary>
		///  Shows the window with the credits for the program.
		/// </summary>
		public void ShowCreditsWindow(Window owner) {
			CreditsWindow.Show(owner);
		}
		/// <summary>
		///  Shows the window that lists all the program hotkeys.
		/// </summary>
		public void ShowHotkeysWindow(Window owner) {

		}

		#endregion

		#region ShowBrowser

		/// <summary>
		///  Shows the save file dialog for PNG images.
		/// </summary>
		/// <param name="owner">The optional owner window to use for this window.</param>
		/// <param name="fileName">The current name of the file to save.</param>
		/// <param name="initialDirectory">The initial directory to save in.</param>
		/// <returns>The selected file path on success, otherwise null.</returns>
		public string ShowSavePngDialog(Window owner, string fileName, string initialDirectory = "") {
			SaveFileDialog dialog = new SaveFileDialog {
				FileName = fileName,
				Filter = "PNG Images|*.png",
				OverwritePrompt = true,
				AddExtension = true,
				InitialDirectory = initialDirectory,
			};
			bool? result = dialog.ShowDialog(owner);
			if (result.HasValue && result.Value)
				return dialog.FileName;
			return null;
		}
		/// <summary>
		///  Shows the open file dialog for EXE and BIN files.
		/// </summary>
		/// <param name="owner">The optional owner window to use for this window.</param>
		/// <param name="fileName">The current name of the file to open.</param>
		/// <param name="initialDirectory">The initial directory to open in.</param>
		/// <returns>The selected file path on success, otherwise null.</returns>
		public string ShowOpenExeDialog(Window owner, string fileName = "", string initialDirectory = "") {
			OpenFileDialog dialog = new OpenFileDialog {
				FileName = fileName,
				Filter = "Executable & Bin files|*.exe;*.bin|All files|*.*",
				AddExtension = true,
				InitialDirectory = initialDirectory,
			};
			bool? result = dialog.ShowDialog(owner);
			if (result.HasValue && result.Value)
				return dialog.FileName;
			return null;
		}

		/// <summary>
		///  Shows the folder browser dialog to select a folder.
		/// </summary>
		/// <param name="owner">The owner window for this dialog.</param>
		/// <param name="description">The description to display.</param>
		/// <param name="showNewFolder">True if the new folder button is present.</param>
		/// <param name="selectedPath">The currently selected path. Use an empty string for nothing.</param>
		/// <returns>The selected path on success, otherwise null.</returns>
		public string ShowFolderBrowser(Window owner, string description, bool showNewFolder, string selectedPath = "") {
			FolderBrowserDialog dialog = new FolderBrowserDialog() {
				Description = description,
				ShowNewFolderButton = showNewFolder,
				SelectedPath = selectedPath,
			};
			bool? result = dialog.ShowDialog(owner);
			if (result ?? false) {
				return dialog.SelectedPath;
			}
			return null;
		}

		#endregion

		#region ShowMessage

		/// <summary>
		///  Shows a message with no icon.
		/// </summary>
		/// <param name="owner">The owner window for this dialog message.</param>
		/// <param name="message">The text message.</param>
		/// <param name="title">The message window title.</param>
		/// <param name="button">The message buttons to display.</param>
		public MessageBoxResult ShowMessage(Window owner, string message, string title, MessageBoxButton button = MessageBoxButton.OK) {
			return MessageBox.Show(owner, message, title, button);
		}
		/// <summary>
		///  Shows a message with an information icon.
		/// </summary>
		/// <param name="owner">The owner window for this dialog message.</param>
		/// <param name="message">The text message.</param>
		/// <param name="title">The message window title.</param>
		/// <param name="button">The message buttons to display.</param>
		public MessageBoxResult ShowInformation(Window owner, string message, string title, MessageBoxButton button = MessageBoxButton.OK) {
			SystemSounds.Asterisk.Play();
			return MessageBox.Show(owner, message, title, button, MessageBoxImage.Information);
		}
		/// <summary>
		///  Shows a message with a question icon.
		/// </summary>
		/// <param name="owner">The owner window for this dialog message.</param>
		/// <param name="message">The text message.</param>
		/// <param name="title">The message window title.</param>
		/// <param name="button">The message buttons to display.</param>
		public MessageBoxResult ShowQuestion(Window owner, string message, string title, MessageBoxButton button = MessageBoxButton.OK) {
			SystemSounds.Asterisk.Play();
			return MessageBox.Show(owner, message, title, button, MessageBoxImage.Question);
		}
		/// <summary>
		///  Shows a message with a warning icon.
		/// </summary>
		/// <param name="owner">The owner window for this dialog message.</param>
		/// <param name="message">The text message.</param>
		/// <param name="title">The message window title.</param>
		/// <param name="button">The message buttons to display.</param>
		public MessageBoxResult ShowWarning(Window owner, string message, string title, MessageBoxButton button = MessageBoxButton.OK) {
			SystemSounds.Exclamation.Play();
			return MessageBox.Show(owner, message, title, button, MessageBoxImage.Warning);
		}
		/// <summary>
		///  Shows a message with an error icon.
		/// </summary>
		/// <param name="owner">The owner window for this dialog message.</param>
		/// <param name="message">The text message.</param>
		/// <param name="title">The message window title.</param>
		/// <param name="button">The message buttons to display.</param>
		public MessageBoxResult ShowError(Window owner, string message, string title, MessageBoxButton button = MessageBoxButton.OK) {
			SystemSounds.Hand.Play();
			return MessageBox.Show(owner, message, title, button, MessageBoxImage.Error);
		}

		#endregion
	}
}
