using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using Com2Com.Common;
using Com2Com.View;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;
using Com2Com.Model;
using System.Linq;
using System.Windows.Controls;

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

        private MasterDeviceModel _masterDeviceModel;

        private ObservableCollection<SlaveModel> _slaves;

        public ObservableCollection<SlaveModel> SlavesCollection
        {
            get { return _slaves; }
            private set { Set(nameof(SlavesCollection), ref _slaves, value);}
        }

        public bool Connected { get { return _masterDeviceModel.Connected; } }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MasterDeviceViewModel()
        {
            //Initialization
            _masterDeviceModel = new MasterDeviceModel();
            _slaves = new ObservableCollection<SlaveModel>() { new SlaveModel() { SlaveId = 44, AnalogValue = 169, DigitalValue = 200 } };
            SlavesCollection.Add(new SlaveModel() { SlaveId = 33, AnalogValue = 99 });

            // Messaging
            MessengerInstance = Messenger.Default;

            MessengerInstance.Register<SlaveDataMessage>(this,_inToken, HandleSlaveDataMessage);
            MessengerInstance.Register<SerialPortSettingsMessage>(this, HandleSerialPortSettingsMessage);

            // Commands
            CreateNavigateToSettingsCommand();
            CreateNavigateToSlaveCommand();
            CreateRefreshSlaveListCommand();
        }

        #region Messages 
        /// <summary>
        /// Token which defines channel of communication. Only recipient which correct token will receive message.
        /// </summary>
        private string _outToken = "fromMasterToSlave";
        private string _inToken = "fromSlaveToMaster";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void HandleSlaveDataMessage(SlaveDataMessage message)
        {
            if(message.DataChanged)
            {
                var slave = _slaves.Where((model) => model.SlaveId == message.SlaveModel.SlaveId).First();
                //SlavesCollection[_slaves.IndexOf(slave)] = slave;
                SlavesCollection.Remove(slave);
                SlavesCollection.Add(message.SlaveModel);
                // HACK: Think about changing the way this functionality is implemented

                _masterDeviceModel.SendMessageToSlave(slave.SlaveId, message.DataChanged, message.DataChanged);
            }
        }

        private void HandleSerialPortSettingsMessage(SerialPortSettingsMessage message)
        {
            _masterDeviceModel.PortSettings = message.SerialPortSettings;
            _masterDeviceModel.Connect();
            RaisePropertyChanged(nameof(Connected));
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
        /// <summary>
        /// 
        /// </summary>
        public ICommand NavigateToSlave { get; private set; }
        private void ExecuteNavigateToSlaveCommand(MouseButtonEventArgs e)
        {
            var soruce = e.Source as ListView;
            // TODO: What if there is no slave ?
            int selectedIndex = soruce.SelectedIndex;
            MessengerInstance.Send(new SlaveDataMessage(_slaves[selectedIndex]),_outToken);

            NavigationHelper.NavigateTo<SlavePage>();
        }
        private void CreateNavigateToSlaveCommand()
        {
            NavigateToSlave = new RelayCommand<MouseButtonEventArgs>(ExecuteNavigateToSlaveCommand);
        }
        /// <summary>
        /// 
        /// </summary>
        public ICommand RefreshSlaveList { get; private set; }
        private void ExecuteRefreshSlaveListCommand()
        {
            _masterDeviceModel.SendMessageToSlave(ProtocolFrame.BroadcastId);
        }
        private void CreateRefreshSlaveListCommand()
        {
            RefreshSlaveList = new RelayCommand(ExecuteRefreshSlaveListCommand);
        }
        #endregion 
    }
}