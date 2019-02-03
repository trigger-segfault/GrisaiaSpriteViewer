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
using Grisaia.Mvvm.Model;
using Grisaia.Mvvm.ViewModel;
using Grisaia.Mvvm.Services;
using Grisaia.SpriteViewer.Services;

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
			SimpleIoc.Default.Register<IGrisaiaDialogService, GrisaiaDialogService>();
			SimpleIoc.Default.Register<UIService>();
			SimpleIoc.Default.Register<SpriteSelectionViewModel>();
			SimpleIoc.Default.Register<LoadingViewModel>();
			SimpleIoc.Default.Register<IRelayCommandFactory, RelayInfoCommandFactory>();
			SimpleIoc.Default.Register<IClipboardService, ClipboardService>();
		}

		//public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
		public SpriteSelectionViewModel SpriteSelection => ServiceLocator.Current.GetInstance<SpriteSelectionViewModel>();
		public LoadingViewModel Loading => ServiceLocator.Current.GetInstance<LoadingViewModel>();

		public static void Cleanup() {
			// TODO Clear the ViewModels
		}
	}
}