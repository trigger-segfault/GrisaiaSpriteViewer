﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using Grisaia.Mvvm.Commands;

namespace Grisaia.Mvvm {
	/// <summary>
	///  An addition to the <see cref="ViewModelBase"/> class with extra helper functions.
	/// </summary>
	public abstract class ViewModelRelayCommand : ViewModelBase {
		#region Fields
		
		/// <summary>
		///  The list of loaded commands.
		/// </summary>
		private readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();
		
		#endregion

		#region Constructors

		/// <summary>
		///  Constructs the <see cref="ViewModelRelayCommand"/>.
		/// </summary>
		public ViewModelRelayCommand() {
			/*// A really dumb method of making sure all relay commands are initialized
			foreach (PropertyInfo prop in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
				if (typeof(ICommand).IsAssignableFrom(prop.PropertyType))
					prop.GetValue(this);
			}*/
		}

		#endregion

		#region Properties

		/// <summary>
		///  Gets all commands created with <see cref="GetCommand"/>.
		/// </summary>
		public IEnumerable<ICommand> Commands => commands.Values;

		#endregion

		#region GetCommand (Basic)

		/// <summary>
		///  Gets the commnd if one exists, or assigns the passed command.
		/// </summary>
		/// <param name="newCommand">The new command to assign if one does not exist.</param>
		/// <param name="commandName">The name of the command.</param>
		/// <returns>The command from the command list.</returns>
		protected TCommand GetCommand<TCommand>(TCommand newCommand, [CallerMemberName] string commandName = null)
			where TCommand : ICommand
		{
			if (!commands.TryGetValue(commandName, out ICommand command)) {
				command = newCommand;
				commands.Add(commandName, command);
			}
			return (TCommand) command;
		}
		/// <summary>
		///  Gets the commnd if one exists, or assigns calls the specified create command function.
		/// </summary>
		/// <param name="createCommand">The function to create the new command if one does not exist.</param>
		/// <param name="commandName">The name of the command.</param>
		/// <returns>The command from the command list.</returns>
		protected TCommand GetCommand<TCommand>(Func<TCommand> createCommand,
			[CallerMemberName] string commandName = null)
			where TCommand : ICommand
		{
			if (!commands.TryGetValue(commandName, out ICommand command)) {
				command = createCommand();
				commands.Add(commandName, command);
			}
			return (TCommand) command;
		}

		#endregion

		#region GetCommand (IRelayCommand)
		
		/// <summary>
		///  Gets or creates a new command with the specified parameters.
		/// </summary>
		/// <param name="execute">The execute method for the command.</param>
		/// <param name="canExecute">The optional canExecute method for the command.</param>
		/// <param name="commandName">The name of the command to get or set.</param>
		/// <returns>The existing or created command.</returns>
		protected IRelayCommand GetCommand(Action execute, bool keepTargetAlive = false,
			[CallerMemberName] string commandName = null)
		{
			return GetCommand(execute, null, keepTargetAlive, commandName);
		}
		/// <summary>
		///  Gets or creates a new command with the specified parameters.
		/// </summary>
		/// <param name="execute">The execute method for the command.</param>
		/// <param name="canExecute">The optional canExecute method for the command.</param>
		/// <param name="commandName">The name of the command to get or set.</param>
		/// <returns>The existing or created command.</returns>
		protected IRelayCommand GetCommand(Action execute, Func<bool> canExecute, bool keepTargetAlive = false,
			[CallerMemberName] string commandName = null)
		{
			return GetCommand(() => new RelayInfoCommand(execute, canExecute, keepTargetAlive), commandName);
		}

		#endregion

		#region GetCommand (IRelayCommand<T>)
		
		/// <summary>
		///  Gets or creates a new command with the specified parameters.
		/// </summary>
		/// <typeparam name="T">The parameter type of the command.</typeparam>
		/// <param name="execute">The execute method for the command.</param>
		/// <param name="canExecute">The optional canExecute method for the command.</param>
		/// <param name="commandName">The name of the command to get or set.</param>
		/// <returns>The existing or created command.</returns>
		protected IRelayCommand<T> GetCommand<T>(Action<T> execute, bool keepTargetAlive = false,
			[CallerMemberName] string commandName = null)
		{
			return GetCommand(execute, null, keepTargetAlive, commandName);
		}
		/// <summary>
		///  Gets or creates a new command with the specified parameters.
		/// </summary>
		/// <typeparam name="T">The parameter type of the command.</typeparam>
		/// <param name="execute">The execute method for the command.</param>
		/// <param name="canExecute">The optional canExecute method for the command.</param>
		/// <param name="commandName">The name of the command to get or set.</param>
		/// <returns>The existing or created command.</returns>
		protected IRelayCommand<T> GetCommand<T>(Action<T> execute, Func<T, bool> canExecute,
			bool keepTargetAlive = false, [CallerMemberName] string commandName = null)
		{
			return GetCommand(() => new RelayInfoCommand<T>(execute, canExecute, keepTargetAlive), commandName);
		}

		#endregion
	}
}
