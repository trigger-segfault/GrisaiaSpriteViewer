using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Grisaia.Mvvm.Services {
	/// <summary>
	///  The dialog service for the Grisaia program.
	/// </summary>
	public interface IGrisaiaDialogService {
		#region Show Main Windows

		/// <summary>
		///  Shows the settings dialog window.
		/// </summary>
		/// <param name="owner">The optional owner window to use for this window.</param>
		/// <returns>The dialog result of the window.</returns>
		bool? ShowSettingsDialog(Window owner);
		/// <summary>
		///  Shows the game locations dialog window.
		/// </summary>
		/// <param name="owner">The optional owner window to use for this window.</param>
		/// <returns>The dialog result of the window.</returns>
		bool? ShowInstallDirsDialog(Window owner);

		/// <summary>
		///  Shows the loading window either in normal or dialog mode.
		/// </summary>
		/// <param name="dialog">True if the window should be shown in dialog mode.</param>
		void ShowLoadingWindow(bool dialog);
		/// <summary>
		///  Shows the sprite selection window.
		/// </summary>
		void ShowSpriteSelectionWindow();
		/// <summary>
		///  Shows the window with information about the program.
		/// </summary>
		void ShowAboutWindow(Window owner);
		/// <summary>
		///  Shows the window with the credits for the program.
		/// </summary>
		void ShowCreditsWindow(Window owner);
		/// <summary>
		///  Shows the window that lists all the program hotkeys.
		/// </summary>
		void ShowHotkeysWindow(Window owner);

		#endregion

		#region ShowBrowser

		/// <summary>
		///  Shows the save file dialog for PNG images.
		/// </summary>
		/// <param name="owner">The optional owner window to use for this window.</param>
		/// <param name="fileName">The current name of the file to save.</param>
		/// <param name="initialDirectory">The initial directory to save in.</param>
		/// <returns>The selected file path on success, otherwise null.</returns>
		string ShowSavePngDialog(Window owner, string fileName, string initialDirectory = "");
		/// <summary>
		///  Shows the open file dialog for EXE and BIN files.
		/// </summary>
		/// <param name="owner">The optional owner window to use for this window.</param>
		/// <param name="fileName">The current name of the file to open.</param>
		/// <param name="initialDirectory">The initial directory to open in.</param>
		/// <returns>The selected file path on success, otherwise null.</returns>
		string ShowOpenExeDialog(Window owner, string fileName = "", string initialDirectory = "");

		/// <summary>
		///  Shows the folder browser dialog to select a folder.
		/// </summary>
		/// <param name="owner">The owner window for this dialog.</param>
		/// <param name="description">The description to display.</param>
		/// <param name="showNewFolder">True if the new folder button is present.</param>
		/// <param name="selectedPath">The currently selected path. Use an empty string for nothing.</param>
		/// <returns>The selected path on success, otherwise null.</returns>
		string ShowFolderBrowser(Window owner, string description, bool showNewFolder, string selectedPath = "");

		#endregion

		#region ShowMessage

		/// <summary>
		///  Shows a message with no icon.
		/// </summary>
		/// <param name="owner">The owner window for this dialog message.</param>
		/// <param name="message">The text message.</param>
		/// <param name="title">The message window title.</param>
		/// <param name="button">The message buttons to display.</param>
		MessageBoxResult ShowMessage(Window owner, string message, string title, MessageBoxButton button = MessageBoxButton.OK);
		/// <summary>
		///  Shows a message with an information icon.
		/// </summary>
		/// <param name="owner">The owner window for this dialog message.</param>
		/// <param name="message">The text message.</param>
		/// <param name="title">The message window title.</param>
		/// <param name="button">The message buttons to display.</param>
		MessageBoxResult ShowInformation(Window owner, string message, string title, MessageBoxButton button = MessageBoxButton.OK);
		/// <summary>
		///  Shows a message with a question icon.
		/// </summary>
		/// <param name="owner">The owner window for this dialog message.</param>
		/// <param name="message">The text message.</param>
		/// <param name="title">The message window title.</param>
		/// <param name="button">The message buttons to display.</param>
		MessageBoxResult ShowQuestion(Window owner, string message, string title, MessageBoxButton button = MessageBoxButton.OK);
		/// <summary>
		///  Shows a message with a warning icon.
		/// </summary>
		/// <param name="owner">The owner window for this dialog message.</param>
		/// <param name="message">The text message.</param>
		/// <param name="title">The message window title.</param>
		/// <param name="button">The message buttons to display.</param>
		MessageBoxResult ShowWarning(Window owner, string message, string title, MessageBoxButton button = MessageBoxButton.OK);
		/// <summary>
		///  Shows a message with an error icon.
		/// </summary>
		/// <param name="owner">The owner window for this dialog message.</param>
		/// <param name="message">The text message.</param>
		/// <param name="title">The message window title.</param>
		/// <param name="button">The message buttons to display.</param>
		MessageBoxResult ShowError(Window owner, string message, string title, MessageBoxButton button = MessageBoxButton.OK);

		#endregion
	}
}
