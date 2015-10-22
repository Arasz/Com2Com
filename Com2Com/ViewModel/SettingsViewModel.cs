using Com2Com.Common;
using Com2Com.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Com2Com.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            CreateNavigateToMainPageCommand();
        }

        #region Commands
        public ICommand NavigateToMainPage { get; private set; }

        private void CreateNavigateToMainPageCommand()
        {
            NavigateToMainPage = new RelayCommand(ExecuteNavigateToMainPageCommand);
        }

        private void ExecuteNavigateToMainPageCommand()
        {
            NavigationHelper.NavigateTo<MasterDevicePage>();
        }
        #endregion
    }
}

