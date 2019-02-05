using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Grisaia.Mvvm.Commands {
	/// <summary>
	///  An interface for working with any type of RelayCommand.
	/// </summary>
	/// <typeparam name="T">The type of the parameter that is passed to the command.</typeparam>
	public interface IRelayCommand<T> : IRelayCommandBase {
		/// <summary>
		///  Executes the method with a parameter.
		/// </summary>
		/// <param name="parameter">The command parameter.</param>
		void Execute(T parameter);
	}
}
