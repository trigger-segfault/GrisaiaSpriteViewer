using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Grisaia.Categories;

namespace Grisaia.SpriteViewer.Controls {
	public class GameInstallControl : Control {
		#region Dependency Properties
		
		public static readonly DependencyProperty GameInfoProperty =
			DependencyProperty.Register(
				"GameInfo",
				typeof(GameInfo),
				typeof(GameInstallControl));
		public static readonly DependencyProperty LocatedInstallDirProperty =
			DependencyProperty.Register(
				"LocatedInstallDir",
				typeof(string),
				typeof(GameInstallControl));
		public static readonly DependencyProperty CustomInstallDirProperty =
			DependencyProperty.Register(
				"CustomInstallDir",
				typeof(string),
				typeof(GameInstallControl));
		public static readonly DependencyProperty CustomExecutableProperty =
			DependencyProperty.Register(
				"CustomExecutable",
				typeof(string),
				typeof(GameInstallControl));
		public static readonly DependencyProperty IsCustomInstallValidatedProperty =
			DependencyProperty.Register(
				"IsCustomInstallValidated",
				typeof(bool?),
				typeof(GameInstallControl));

		public GameInfo GameInfo {
			get => (GameInfo) GetValue(GameInfoProperty);
			set => SetValue(GameInfoProperty, value);
		}
		public string LocatedInstallDir {
			get => (string) GetValue(LocatedInstallDirProperty);
			set => SetValue(LocatedInstallDirProperty, value);
		}
		public string CustomInstallDir {
			get => (string) GetValue(CustomInstallDirProperty);
			set => SetValue(CustomInstallDirProperty, value);
		}
		public string CustomExecutable {
			get => (string) GetValue(CustomExecutableProperty);
			set => SetValue(CustomExecutableProperty, value);
		}
		public bool? IsCustomInstallValidated {
			get => (bool?) GetValue(IsCustomInstallValidatedProperty);
			set => SetValue(IsCustomInstallValidatedProperty, value);
		}

		#endregion

		#region Static Constructor

		static GameInstallControl() {
			DefaultStyleKeyProperty.AddOwner(typeof(GameInstallControl),
				new FrameworkPropertyMetadata(typeof(GameInstallControl)));
		}

		#endregion
	}
}
