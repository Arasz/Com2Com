using Com2Com.Common;
using Com2Com.Model;
using Com2Com.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Com2Com.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {

        private SettingsModel _settingsModel;

        public SettingsModel SettingsModel
        {
            get
            {
                return _settingsModel;
            }

        }

        public ObservableCollection<string> PortNames { get; private set; }

        public string PortName
        {
            get { return _settingsModel.PortName; }
            set { _settingsModel.PortName = value; }
        }
        public string Parity
        {
            get { return _settingsModel.Parity.ToString(); }
            set { Set(nameof(Parity), ref _settingsModel.Parity, (Parity)Enum.Parse(typeof(Parity),value)); }
        }
        public string DataBits
        {
            get { return _settingsModel.DataBits.ToString(); }
            set { Set(nameof(DataBits), ref _settingsModel.DataBits, (DataBits)Enum.Parse(typeof(DataBits), value)); }
        }
        public string StopBits
        {
            get { return _settingsModel.StopBits.ToString(); }
            set { Set(nameof(StopBits), ref _settingsModel.StopBits, (StopBits)Enum.Parse(typeof(StopBits), value)); }
        }

        public int BaudRate
        {
            get { return _settingsModel.BaudRate; }
            set { Set(nameof(BaudRate), ref _settingsModel.BaudRate, value); }
        }
        public ObservableCollection<string> ParityCollection { get; private set; }

        public ObservableCollection<string> StopBitsCollection { get; private set; }

        public ObservableCollection<string> DataBitsCollection { get; private set; }

        public SettingsViewModel()
        {
            _settingsModel = new SettingsModel();
            PortNames = new ObservableCollection<string>(SerialPort.GetPortNames());
            ParityCollection = new ObservableCollection<string>(Enum.GetNames(typeof(Parity)));
            StopBitsCollection = new ObservableCollection<string>(Enum.GetNames(typeof(StopBits)));
            DataBitsCollection = new ObservableCollection<string>(Enum.GetNames(typeof(DataBits)));

            CreateNavigateToMainPageCommand();
        }

        #region Commands
        public ICommand NavigateToMainPage { get; private set; }

        private void CreateNavigateToMainPageCommand()
        {
            NavigateToMainPage = new RelayCommand<SettingsModel>(ExecuteNavigateToMainPageCommand);
        }

        private void ExecuteNavigateToMainPageCommand(SettingsModel portSettings)
        {
            NavigationHelper.NavigateTo<MasterDevicePage>(portSettings);
        }
        #endregion
    }
}

