using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using Com2Com.Common;
using Com2Com.View;

namespace Com2Com.ViewModel
{
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
    public class MasterViewModel : ViewModelBase
    {


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MasterViewModel()
        {
            CreateNavigateToSettingsCommand();
        }


        #region Commands
        /// <summary>
        /// Navigate to settings page.
        /// </summary>
        public ICommand NavigateToSettings
        {
            get; private set;
        }
        private void CreateNavigateToSettingsCommand()
        {
            NavigateToSettings = new RelayCommand(ExecuteNavigateToSettings);
 
        }
        private void ExecuteNavigateToSettings()
        {
            NavigationHelper.NavigateTo<SettingsPage>();
        }
        #endregion 
    }
}