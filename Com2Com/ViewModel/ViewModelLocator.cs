/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Com2Com"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support. 
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;

namespace Com2Com.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
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

            SimpleIoc.Default.Register<MasterDeviceViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<SlaveViewModel>();
        }

        public MasterDeviceViewModel MasterDevice
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MasterDeviceViewModel>();
            }
        }

        public SettingsViewModel Settings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        public SlaveViewModel Slave
        {
            get { return ServiceLocator.Current.GetInstance<SlaveViewModel>(); }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
            
        }

        private INavigationService CreateNavigationService()
        {
            // TODO: Implement navigation service based on : http://www.c-sharpcorner.com/UploadFile/3789b7/modern-ui-for-wpf-application-by-example-navigationservice/
            throw new NotImplementedException();
        }
    }
}