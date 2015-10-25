using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using Com2Com.Common;
using Com2Com.View;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;
using Com2Com.Model;
using System.Linq;

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
        
        private ObservableCollection<SlaveModel> _slaves = new ObservableCollection<SlaveModel>() { new SlaveModel() { SlaveId = 44, AnalogValue = 12.5 } };

        public ObservableCollection<SlaveModel> SlavesCollection
        {
            get { return _slaves; }
            private set { Set(nameof(SlavesCollection), ref _slaves, value);}
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MasterDeviceViewModel()
        {
            //Initialization
            SlavesCollection.Add(new SlaveModel() { SlaveId = 33, AnalogValue = 99 });

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
            var slave = _slaves.Where((model) => model.SlaveId == message.SlaveModel.SlaveId).First();
            //SlavesCollection[_slaves.IndexOf(slave)] = slave;
            SlavesCollection.Remove(slave);
            SlavesCollection.Add(message.SlaveModel);
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
            // TODO: Change this behavior
            MessengerInstance.Send(new SlaveDataMessage(_slaves[0]));
            NavigationHelper.NavigateTo<SlavePage>(_slaves[0]);
        }
        private void CreateNavigateToSlaveCommand()
        {
            NavigateToSlave = new RelayCommand(ExecuteNavigateToSlaveCommand);
        }
        #endregion 
    }
}