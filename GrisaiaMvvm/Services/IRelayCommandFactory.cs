using System;
using Grisaia.Mvvm.Commands;

namespace Grisaia.Mvvm.Services {
	/// <summary>
	///  A service for creating relay commands to be loaded by the view model.
	/// </summary>
	public interface IRelayCommandFactory {
		/// <summary>
		///  Creates a new <see cref="IRelayCommand"/> with the specified info and functions.
		/// </summary>
		/// <param name="info">The abstract information about the command.</param>
		/// <param name="execute">The execution action.</param>
		/// <param name="canExecute">The optional can execute function.</param>
		/// <returns>The constructed <see cref="IRelayCommand"/>.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="execute"/> is null.
		/// </exception>
		IRelayCommand Create(Action execute, Func<bool> canExecute, bool keepTargetAlive = false);
		/// <summary>
		///  Creates a new <see cref="IRelayCommand"/> with the specified info and functions.
		/// </summary>
		/// <typeparam name="T">The parameter type of the command.</typeparam>
		/// <param name="info">The abstract information about the command.</param>
		/// <param name="execute">The execution action.</param>
		/// <param name="canExecute">The optional can execute function.</param>
		/// <returns>The constructed <see cref="IRelayCommand{T}"/>.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="execute"/> is null.
		/// </exception>
		IRelayCommand<T> Create<T>(Action<T> execute, Func<T, bool> canExecute, bool keepTargetAlive = false);
	}
}
