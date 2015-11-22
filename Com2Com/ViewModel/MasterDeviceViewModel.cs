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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Threading;
using System;

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

        #region Events
        private void _masterDeviceModel_SlaveUpdated(object sender, MessageStatusChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(SlavesCollection));
            MessengerInstance.Send(new ProtocolFrameMessage(e.Frame,false),_fromMasterToSlaveChannel);

            // HACK: This line will change slave mode visible in slave view if last slave sent from web server was different from opened slave. Line commented. Slave state will not update in slave view.
            //MessengerInstance.Send(new SlaveDataMessage(_slaves[e.SlaveId],true,true), _fromMasterToSlaveChannel);
            
        }

        private void _masterDeviceModel_MessageSent(object sender, MessageStatusChangedEventArgs e)
        {
            MessengerInstance.Send(new ProtocolFrameMessage(e.Frame,true), _fromMasterToSlaveChannel);
        }


        #endregion

        private MasterDeviceModel _masterDeviceModel;

        private Dictionary<int, SlaveModel> _slaves;

        public ObservableCollection<SlaveModel> SlavesCollection
        {
            get { return new ObservableCollection<SlaveModel>(_slaves.Values); }
        }

        public bool Connected { get { return _masterDeviceModel.Connected; } }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MasterDeviceViewModel()
        {
            //Initialization
            _masterDeviceModel = new MasterDeviceModel();
            _slaves = _masterDeviceModel.Slaves;


            //Events subscription

            _masterDeviceModel.SlaveUpdated += _masterDeviceModel_SlaveUpdated;
            _masterDeviceModel.MessageSent += _masterDeviceModel_MessageSent;

            // Messaging
            MessengerInstance = Messenger.Default;
            MessengerInstance.Register<SlaveDataMessage>(this, _slaveDataToMasterChannel, HandleSlaveDataMessage);
            MessengerInstance.Register<SerialPortSettingsMessage>(this, HandleSerialPortSettingsMessage);

            // Commands
            CreateNavigateToSettingsCommand();
            CreateNavigateToSlaveCommand();
            CreateRefreshSlaveListCommand();

        }

        #region Messages 
        /// <summary>
        /// Tokens which defines channels of communication. Only recipient with correct token will receive message. 
        /// </summary>
        private string _fromMasterToSlaveChannel = "fromMasterToSlaveChannel";
        private string _slaveDataToMasterChannel = "slaveDataToMasterChannel";

        /// <summary>
        /// Receives messages with slave data and pushes it to master model
        /// </summary>
        /// <param name="message">Message with data of changed slave</param>
        private void HandleSlaveDataMessage(SlaveDataMessage message)
        {
            if (Connected)
            {
                int slaveId = message.SlaveModel.SlaveId;
                // TODO: Separate this condition
                if (message.AnalogDataChanged)
                    _slaves[slaveId].AnalogValue = message.SlaveModel.AnalogValue;
                if (message.DigitalDataChanged)
                    _slaves[slaveId].DigitalValue = message.SlaveModel.DigitalValue;

                RaisePropertyChanged(nameof(SlavesCollection));

                _masterDeviceModel.SendMessageToSlave(slaveId, digitalDataChanged: message.DigitalDataChanged, analogDataChanged: message.AnalogDataChanged);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void HandleSerialPortSettingsMessage(SerialPortSettingsMessage message)
        {
            _masterDeviceModel.PortSettings = message.SerialPortSettings;
            try
            {
                _masterDeviceModel.Connect();
            }
            catch(ComDeviceException comex)
            {
                // HACK: Make this in MVVM style
                string mboxText = $"Port name: {comex.PortName}\nMessage: {comex.Message}\nAdditional informations: {comex.Cause}";
                System.Windows.MessageBox.Show(mboxText, "Exception", System.Windows.MessageBoxButton.OK);
            }
            RaisePropertyChanged(nameof(Connected));
        }
        #endregion

        #region Commands
        /// <summary>
        /// Navigate to settings page
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
            try
            {
                _masterDeviceModel.Disconnect();
            }
            catch (ComDeviceException comex)
            {
                // HACK: Make this in MVVM style
                string mboxText = $"Port name: {comex.PortName}\nMessage: {comex.Message}\nAdditional informations: {comex.Cause}";
                System.Windows.MessageBox.Show(mboxText, "Exception", System.Windows.MessageBoxButton.OK);
            }
            NavigationHelper.NavigateTo<SettingsPage>();
        }
        /// <summary>
        /// Navigate to slave page and pass chosen SlaveModel object
        /// </summary>
        public ICommand NavigateToSlave { get; private set; }
        private void ExecuteNavigateToSlaveCommand(MouseButtonEventArgs e)
        {
            var soruce = e.Source as ListView;
            int selectedIndex = soruce.SelectedIndex;
            soruce.SelectedIndex = -1;
            if(selectedIndex > -1)
            {
                MessengerInstance.Send(new SlaveDataMessage(SlavesCollection[selectedIndex]), _fromMasterToSlaveChannel);
                NavigationHelper.NavigateTo<SlavePage>();
            }
        }
        private void CreateNavigateToSlaveCommand()
        {
            NavigateToSlave = new RelayCommand<MouseButtonEventArgs>(ExecuteNavigateToSlaveCommand);
        }
        /// <summary>
        /// Refresh slave list (send broadcast id command )
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