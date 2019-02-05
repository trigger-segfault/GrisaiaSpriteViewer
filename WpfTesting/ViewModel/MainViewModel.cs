using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using WpfTesting.Model;

namespace WpfTesting.ViewModel {
	/// <summary>
	/// This class contains properties that the main View can data bind to.
	/// <para>
	/// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
	/// </para>
	/// <para>
	/// You can also use Blend to data bind with the tool's support.
	/// </para>
	/// <para>
	/// See http://www.galasoft.ch/mvvm
	/// </para>
	/// </summary>
	public class MainViewModel : ViewModelBase {
		/*/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel() {
			Collection = new ObservableCollection<int>();
			Categories = new ObservableCollection<ComboItem> {
				new ComboItem(0, 5),
			};
			for (int i = 0; i < 4; i++) {
				Categories.Add(Categories[i].Items[0]);
			}
			Action = new RelayCommand(ActionExecute);
			for (int i = 0; i < 4; i++) {
				Collection.Add(i);
			}
			Categories.CollectionChanged += Categories_CollectionChanged;
		}

		private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			Console.WriteLine($"{e.Action} Start={e.NewStartingIndex} Count={e.NewItems.Count}");
		}

		private Random random = new Random();

		public ObservableCollection<ComboItem> Categories { get; }
		public ObservableCollection<int> Collection { get; }
		public RelayCommand Action { get; }

		private void ActionExecute() {
			Collection[random.Next(Collection.Count)]++;
		}*/
		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel() {
			Collection = new ObservableCollection<int>();
			Categories = new ObservableArray<ComboItem>(5);
			this[0] = new ComboItem(0, 5);
			for (int i = 0; i < 4; i++) {
				this[i + 1] = this[i].Items[0];
			}
			ComboItem.TotalItems = 0;
			Categories[0] = new ComboItem(0, 5);
			for (int i = 0; i < 4; i++) {
				Categories[i + 1] = Categories[i].Items[0];
			}
			Action = new RelayCommand(ActionExecute);
			for (int i = 0; i < 4; i++) {
				Collection.Add(i);
			}
			Categories.CollectionChanged += Categories_CollectionChanged;
		}

		private Random random = new Random();

		public ObservableArray<ComboItem> Categories { get; }
		public ObservableCollection<int> Collection { get; }
		public RelayCommand Action { get; }

		private ComboItem c0;
		private ComboItem c1;
		private ComboItem c2;
		private ComboItem c3;
		private ComboItem c4;

		public ComboItem this[int index] {
			get => (ComboItem) typeof(MainViewModel).GetProperty($"Categories{index}").GetValue(this);
			set => typeof(MainViewModel).GetProperty($"Categories{index}").SetValue(this, value);
		}

		public ComboItem Categories0 { get => c0; set => Set(ref c0, value); }
		public ComboItem Categories1 { get => c1; set => Set(ref c1, value); }
		public ComboItem Categories2 { get => c2; set => Set(ref c2, value); }
		public ComboItem Categories3 { get => c3; set => Set(ref c3, value); }
		public ComboItem Categories4 { get => c4; set => Set(ref c4, value); }

		private void ActionExecute() {
			Collection[random.Next(Collection.Count)]++;
		}


		private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			//Console.WriteLine($"{e.Action} Start={e.NewStartingIndex} Count={e.NewItems.Count}");
		}
	}
}
 