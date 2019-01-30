/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Grisaia.SpriteViewer"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System;
using System.IO;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Grisaia.Categories;
using Grisaia.Categories.Sprites;
using Grisaia.SpriteViewer.Model;

namespace Grisaia.SpriteViewer.ViewModel {
	/// <summary>
	/// This class contains static references to all the view models in the
	/// application and provides an entry point for the bindings.
	/// </summary>
	public class ViewModelLocator {
		/// <summary>
		/// Initializes a new instance of the ViewModelLocator class.
		/// </summary>
		public ViewModelLocator() {
			// Prevents the designer from complaining about a service being added multiple times
			SimpleIoc.Default.Reset();

			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			////if (ViewModelBase.IsInDesignModeStatic)
			////{
			////    // Create design time view services and models
			////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
			////}
			////else
			////{
			////    // Create run time view services and models
			////    SimpleIoc.Default.Register<IDataService, DataService>();
			////}
			
			SimpleIoc.Default.Register<GrisaiaModel>();
			//SimpleIoc.Default.Register<SpriteViewerSettings>();
			//SimpleIoc.Default.Register<SpriteDatabase>(true);
			//SimpleIoc.Default.Register<MainViewModel>();
			SimpleIoc.Default.Register<SpriteViewModel>();
			//SimpleIoc.Default.Register<LoadingViewModel>(true);
		}

		public string DataPath {
			get {
				if (ViewModelBase.IsInDesignModeStatic)
					return Path.Combine(DummyCategorizationContext.BaseDirectory, "data");
				else
					return Path.Combine(AppContext.BaseDirectory, "data");
			}
		}

		//public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
		public SpriteViewModel Sprite => ServiceLocator.Current.GetInstance<SpriteViewModel>();

		public static void Cleanup() {
			// TODO Clear the ViewModels
		}
	}
}