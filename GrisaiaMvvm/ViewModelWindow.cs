using System.Windows;
using GalaSoft.MvvmLight;
using Grisaia.Mvvm.Services;

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
		private string title = "Grisaia Extract Sprite Viewer";

		#endregion
		
		#region Constructors

		/// <summary>
		///  Constructs the <see cref="ViewModelWindow"/>.
		/// </summary>
		/// <param name="relayFactory">The factory used to create relay commands.</param>
		public ViewModelWindow(IRelayCommandFactory relayFactory) : base(relayFactory) { }

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

		#region Virtual Methods

		/// <summary>
		///  Called when the window loads the view model.
		/// </summary>
		public virtual void Loaded() { }

		#endregion
	}
}
