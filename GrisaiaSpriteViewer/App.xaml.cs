using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Grisaia.Mvvm.ViewModel.Messages;
using Grisaia.SpriteViewer.ViewModel;
using Grisaia.SpriteViewer.Windows;

namespace Grisaia.SpriteViewer {
	/// <summary>
	///  Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		#region Constants

		private static readonly string[] assemblyExtensions = { ".dll", ".exe" };

		#endregion

		#region Properties

		public ViewModelLocator Locator { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs the app and sets up embedded assembly resolving.
		/// </summary>
		public App() {
			AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssemblies;
			// Call this to avoid referencing assemblies before the assembly resolver can be added.
			Initialize();
		}

		/// <summary>
		///  Called to avoid referencing assemblies before the assembly resolver can be added.
		/// </summary>
		private void Initialize() {
			ErrorMessageBox.ProgramName = "Grisaia Extract Sprite Viewer";
			ErrorMessageBox.HyperlinkName = "GitHub Page";
			ErrorMessageBox.HyperlinkUri = new Uri(@"https://github.com/trigger-death/GrisaiaSpriteViewer");
			ErrorMessageBox.GlobalHook(this);
		}

		#endregion

		#region Event Handlers

		private void OnAppStartup(object sender, StartupEventArgs e) {
			Locator = (ViewModelLocator) FindResource("Locator");
			//Locator.Loading.LoadEverything.Execute();
			Locator.Messenger.Send(new OpenLoadingWindowMessage {
				LoadEverything = true,
				OpenSpriteSelectionWindow = true,
				ShowDialog = false,
			});
		}

		/// <summary>
		///  Resolves assemblies that may be embedded in the executable.
		/// </summary>
		private Assembly OnResolveAssemblies(object sender, ResolveEventArgs args) {
			AssemblyName assemblyName = new AssemblyName(args.Name);
			string assemblyPath;

			if (TryResolveAssembly(assemblyName, out Assembly assembly))
				return assembly;
			assemblyPath = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
			if (TryResolveAssembly(assemblyPath, assemblyName, out assembly))
				return assembly;
			assemblyPath = CultureInfo.CurrentCulture.ToString();
			if (TryResolveAssembly(assemblyPath, assemblyName, out assembly))
				return assembly;

			return null;
		}

		#endregion

		#region TryResolveAssembly

		private bool TryResolveAssembly(AssemblyName assemblyName, out Assembly assembly) {
			return TryResolveAssembly(null, assemblyName, out assembly);
		}
		private bool TryResolveAssembly(string path, AssemblyName assemblyName, out Assembly assembly) {
			foreach (string ext in assemblyExtensions) {
				string startPath = Path.Combine(AppContext.BaseDirectory, "bin");
				if (path != null && !Path.IsPathRooted(path))
					startPath = Path.Combine(startPath, path);
					
				path = Path.Combine(startPath, assemblyName.Name + ext);
				if (File.Exists(path)) {
					assembly = Assembly.LoadFile(path);
					return true;
				}
			}
			assembly = null;
			return false;
		}

		#endregion
	}
}
