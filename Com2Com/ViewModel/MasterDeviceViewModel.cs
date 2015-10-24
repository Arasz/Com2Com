using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using Com2Com.Common;
using Com2Com.View;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;

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
    public class MasterDeviceViewModel : ViewModelBase
    {
        private ObservableCollection<string> _slaves = new ObservableCollection<string> { "dummmySlave" };

        public ObservableCollection<string> SlaveCollection
        { get { return _slaves; } }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MasterDeviceViewModel()
        {
            // Messaging
            MessengerInstance = Messenger.Default;

            MessengerInstance.Register<SlaveDataMessage>(this, HandleSlaveDataMessage);
            MessengerInstance.Register<SerialPortSettingsMessage>(this, HandleSerialPortSettingsMessage);

            // Commands
            CreateNavigateToSettingsCommand();
            CreateNavigateToSlaveCommand();
        }

        #region Messages 
        private void HandleSlaveDataMessage(SlaveDataMessage message)
        {
            //TODO: IMPLEMNT HandleSlaveDataMessage
        }

        private void HandleSerialPortSettingsMessage(SerialPortSettingsMessage message)
        {
            //TODO: IMPLEMNT HandleSlaveDataMessage
        }
        #endregion

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
            NavigateToSettings = new RelayCommand(ExecuteNavigateToSettingsCommand);
 
        }
        private void ExecuteNavigateToSettingsCommand()
        {
            NavigationHelper.NavigateTo<SettingsPage>();
        }

        public ICommand NavigateToSlave { get; private set; }
        private void ExecuteNavigateToSlaveCommand()
        {
            MessengerInstance.Send(new SlaveDataMessage(new Model.SlaveModel()));
            NavigationHelper.NavigateTo<SlavePage>(_slaves[0]);
        }
        private void CreateNavigateToSlaveCommand()
        {
            NavigateToSlave = new RelayCommand(ExecuteNavigateToSlaveCommand);
        }
        #endregion 
    }
}