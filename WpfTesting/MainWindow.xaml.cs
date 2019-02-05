using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfTesting.ViewModel;

namespace WpfTesting {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
		}

		private void OnLoaded(object sender, RoutedEventArgs e) {
			var vm = ((MainViewModel) DataContext);
			vm.Categories.CollectionChanged += Categories_CollectionChanged;
		}

		private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

		}

		private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			Console.WriteLine("-----------------------------------");
		}
	}
}
