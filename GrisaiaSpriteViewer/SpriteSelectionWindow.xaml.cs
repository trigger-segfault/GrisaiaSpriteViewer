using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Grisaia.Asmodean;
using Grisaia.Categories;
using Grisaia.Categories.Sprites;
using Grisaia.Mvvm.ViewModel;
using Microsoft.Win32;

namespace Grisaia.SpriteViewer {
	/// <summary>
	///  Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class SpriteSelectionWindow : Window {
		#region Fields
		
		//private double scale = 1.0;
		//private bool centered = true;
		//private int currentWidth = 0;
		//private int currentHeight = 0;
		//private Thickness expandShrink;

		//private double savedScale = 1.0;
		//private Vector savedNormalizedScroll = new Vector(-1, -1);

		private bool supressEvents = false;

		#endregion

		#region Properties

		public SpriteSelectionViewModel ViewModel => (SpriteSelectionViewModel) DataContext;

		#endregion

		#region Static Constructor

		static SpriteSelectionWindow() {
			DataContextProperty.AddOwner(typeof(SpriteSelectionWindow),
				new FrameworkPropertyMetadata(
					OnDataContextChanged));
		}

		#endregion

		#region Constructors

		public SpriteSelectionWindow() {
			InitializeComponent();
		}

		#endregion

		#region Event Handlers

		private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpriteSelectionWindow window = (SpriteSelectionWindow) d;
			window.ViewModel.WindowOwner = window;
		}
		private void OnClosed(object sender, EventArgs e) {
			ViewModel.WindowOwner = null;
		}
		private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			Console.WriteLine("-------------------------------------");
		}

		#endregion
	}
}
