using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Grisaia.Mvvm.Commands;
using Grisaia.Mvvm.Input;

namespace Grisaia.SpriteViewer.Commands {
	public interface IRelayInfoCommand : IRelayCommandBase, INotifyPropertyChanged {

		#region Properties

		/// <summary>
		///  Gets or sets the UI specific info for the command.
		/// </summary>
		RelayInfo Info { get; set; }
		/// <summary>
		///  Gets the display text for the command.
		/// </summary>
		string Text { get; }
		/// <summary>
		///  Gets the display icon for the command.
		/// </summary>
		ImageSource Icon { get; }
		/// <summary>
		///  Gets the input gesture for the command.
		/// </summary>
		AnyKeyGesture InputGesture { get; }

		#endregion
	}
}
