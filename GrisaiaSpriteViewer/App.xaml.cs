using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Grisaia.SpriteViewer.Windows;

namespace Grisaia.SpriteViewer {
	/// <summary>
	///  Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		/// <summary>
		///  Constructs the app and sets up embedded assembly resolving.
		/// </summary>
		public App() {
			AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssemblies;
			Initialize();
		}

		/// <summary>
		///  Called to avoid referencing assemblies before the assembly resolver can be added.
		/// </summary>
		private void Initialize() {
			ErrorMessageBox.GlobalHook(this);
		}
		
		/// <summary>
		///  Resolves assemblies that may be embedded in the executable.
		/// </summary>
		private Assembly OnResolveAssemblies(object sender, ResolveEventArgs args) {
			AssemblyName assemblyName = new AssemblyName(args.Name);
			string culturePath;

			if (TryResolveAssembly(assemblyName, out Assembly assembly))
				return assembly;
			culturePath = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
			if (TryResolveAssembly(culturePath, assemblyName, out assembly))
				return assembly;
			culturePath = CultureInfo.CurrentCulture.ToString();
			if (TryResolveAssembly(culturePath, assemblyName, out assembly))
				return assembly;

			return null;
		}
		private static readonly string[] assemblyExtensions = { ".dll", ".exe" };
		private bool TryResolveAssembly(AssemblyName assemblyName, out Assembly assembly) {
			return TryResolveAssembly(null, assemblyName, out assembly);
		}
		private bool TryResolveAssembly(string path, AssemblyName assemblyName, out Assembly assembly) {
			foreach (string ext in assemblyExtensions) {
				string startPath = Path.Combine(AppContext.BaseDirectory, "libs");
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
	}
}
