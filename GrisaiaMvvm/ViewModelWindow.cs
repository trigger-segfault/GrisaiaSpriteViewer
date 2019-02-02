using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Grisaia.Mvvm {
	/// <summary>
	///  An addition to the <see cref="ViewModelBase"/> class with extra helper functions.
	/// </summary>
	public abstract class ViewModelWindow : ViewModelRelayCommand {
		#region Fields

		/// <summary>
		///  The window owning this view model.
		/// </summary>
		private Window windowOwner;
		/// <summary>
		///  The title to display for the window.
		/// </summary>
		private string title = "Grisaia Sprite Viewer";

		#endregion

		#region Constructors

		public ViewModelWindow() { }

		#endregion

		#region Properties

		/// <summary>
		///  Gets or sets the window owning this view model.
		/// </summary>
		public Window WindowOwner {
			get => windowOwner;
			set => Set(ref windowOwner, value);
		}
		/// <summary>
		///  Gets or sets the title to display for the window.
		/// </summary>
		public string Title {
			get => title;
			set => Set(ref title, value);
		}

		#endregion
	}
}
