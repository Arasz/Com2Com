using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using Com2Com.Model;
using Com2Com.Common;
using Com2Com.View;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace Com2Com.ViewModel
{
    public class LightSymbol : INotifyPropertyChanged
    {
        private Rect _baseRectangel = new Rect(0,0,20,20);

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public double X
        {
            get { return _baseRectangel.X; }
            set { _baseRectangel.X = value; }
        }
        public double Y
        {
            get { return _baseRectangel.Y; }
            set { _baseRectangel.Y = value; }
        }
        public double Width
        {
            get { return _baseRectangel.Width; }
            set { _baseRectangel.Width = value; }
        }
        public double Height
        {
            get { return _baseRectangel.Height; }
            set { _baseRectangel.Height = value; }
        }

        private bool _state = false;
        public bool State { get { return _state; } set { _state = value; OnPropertyChanged(nameof(State)); } }

        public bool Contains(Point point)
        {
            return _baseRectangel.Contains(point);
        }
    }

    public class SlaveViewModel : ViewModelBase
    {
        public ObservableCollection<LightSymbol> DigitalIoCollection { get; private set; }
         
        private SlaveModel _inputSlaveModel;

        private double _analogValue = 0;
        /// <summary>
        /// Slave analog i/o value
        /// </summary>
        public double AnalogValue
        {
            get { return (double)_analogValue; }
            set { Set(nameof(AnalogValue), ref _analogValue, value); }
        }

        private int _slaveId = 0;
        /// <summary>
        /// Slave id
        /// </summary>
        public int SlaveId
        {
            get { return _slaveId; }
            private set { Set(nameof(SlaveId),ref _slaveId, value); }
        }

        private string _lastMessage = string.Empty;
        /// <summary>
        /// Last sent/received message
        /// </summary>
        public string LastMessage
        {
            get { return _lastMessage; }
            private set { Set(nameof(LastMessage), ref _lastMessage, value); }
        }

        public SlaveViewModel()
        {

            // Initialization
            InitializeLightSymbols();
            // Messaging
            MessengerInstance = Messenger.Default;
            MessengerInstance.Register<SlaveDataMessage>(this, _inToken, HandleSlaveModelMessage);
            MessengerInstance.Register<ProtocolFrameMessage>(this, _inToken, HandleProtocolFrameMessage);
            // Commands
            CreateNavigateToMasterPageCommand();
            CreateSendSlaveModelCommand();
            CreateChangeDigitalIoState();
            // Events
        }

        /// <summary>
        /// Creates representation of digital i/o
        /// </summary>
        private void InitializeLightSymbols()
        {
            Point delta = new Point(30, 40);

            int ioAmount = 8;

            DigitalIoCollection = new ObservableCollection<LightSymbol>();

            for (int i = 0; i < ioAmount / 2; i++)
            {
                var newSymbol = new LightSymbol()
                {
                    X = i * delta.X,
                };
                DigitalIoCollection.Add(newSymbol);
            }
            for (int i = 0; i <ioAmount/2; i++)
            {
                var newSymbol = new LightSymbol()
                {
                    X = i * delta.X,
                    Y = delta.Y,
                    State = true,
                };
                DigitalIoCollection.Add(newSymbol);
            }
        }

        /// <summary>
        /// Updates ViewModel with received slaveModel data
        /// </summary>
        private void UpdateViewModel(SlaveModel slaveModel)
        {
            uint mask = 0x01;
            for (int i = 0; i < DigitalIoCollection.Count; i++)
            {
                DigitalIoCollection[i].State = Convert.ToBoolean((slaveModel.DigitalValue>>i) & mask);
            }
            AnalogValue = slaveModel.AnalogValue;
            SlaveId = slaveModel.SlaveId;
        }

        /// <summary>
        /// Creates new slave model with user data
        /// </summary>
        private SlaveModel CreateSlaveModel()
        {
            uint digitalValue = 0x00;
            SlaveModel slaveModel = new SlaveModel();
            for (int i = 0; i < DigitalIoCollection.Count; i++)
            {
                digitalValue = digitalValue | (Convert.ToUInt32(DigitalIoCollection[i].State)<<i);
            }
            slaveModel.DigitalValue = (ushort)digitalValue;
            slaveModel.AnalogValue = Convert.ToInt16(AnalogValue);
            slaveModel.SlaveId = SlaveId;
            return slaveModel;
        }

        /// <summary>
        /// Checks if slave model changed and stores information about this change in message
        /// </summary>
        /// <param name="message">Message with slave model and data about change</param>
        /// <returns></returns>
        private void CheckSlaveModelChanged(SlaveDataMessage message)
        {
            if (message.SlaveModel.DigitalValue != _inputSlaveModel.DigitalValue)
                message.DigitalDataChanged = true;
            if (message.SlaveModel.AnalogValue != _inputSlaveModel.AnalogValue)
                message.AnalogDataChanged = true;
        }


        #region Messaging

        /// <summary>
        /// Token which defines channel of communication. Only recipient which correct token will receive message.
        /// </summary>
        private string _inToken = "fromMasterToSlave";
        private string _outToken = "fromSlaveToMaster";

        /// <summary>
        /// Processes message with slave data
        /// </summary>
        /// <param name="message">Slave model</param>
        private void HandleSlaveModelMessage(SlaveDataMessage message)
        {
            _inputSlaveModel = message.SlaveModel;
            UpdateViewModel(message.SlaveModel);
        }

        /// <summary>
        /// Processes message with used protocol frame
        /// </summary>
        /// <param name="message">Protocol frame</param>
        private void HandleProtocolFrameMessage(ProtocolFrameMessage message)
        {
            LastMessage += Environment.NewLine + message.Frame.ToString();
        }
        #endregion

        #region Commands
        /// <summary>
        /// 
        /// </summary>
        public ICommand NavigateToMasterPage { get; private set; }
        private void ExecuteNavigateToMasterPageCommand()
        {
            NavigationHelper.NavigateTo<MasterDevicePage>();
        }
        private void CreateNavigateToMasterPageCommand()
        {
            NavigateToMasterPage = new RelayCommand(ExecuteNavigateToMasterPageCommand);
        }
        /// <summary>
        /// 
        /// </summary>
        public ICommand SendSlaveModel { get; private set; }
        private void ExecuteSendSlaveModelCommand()
        { 
            SlaveModel slaveModel = CreateSlaveModel();
            SlaveDataMessage message = new SlaveDataMessage(slaveModel);
            CheckSlaveModelChanged(message);
            MessengerInstance.Send(message,_outToken);
        }
        private void CreateSendSlaveModelCommand()
        {
            SendSlaveModel = new RelayCommand(ExecuteSendSlaveModelCommand);
        }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ChangeDigitalIoState { get; private set; }
        private void ExecuteChangeDigitalIoState(MouseEventArgs e)
        {
            var soruce = e.Source as ItemsControl;
            Point mousePosition = e.GetPosition(soruce);

            int index = -1;
            foreach (LightSymbol symbol in DigitalIoCollection)
            {
                if(symbol.Contains(mousePosition))
                {
                    index = DigitalIoCollection.IndexOf(symbol);
                    break;
                }
            }

            if(index>-1)
                DigitalIoCollection[index].State = !DigitalIoCollection[index].State;
        }
        private void CreateChangeDigitalIoState()
        {
            ChangeDigitalIoState = new RelayCommand<MouseEventArgs>(ExecuteChangeDigitalIoState);
        }
        #endregion

        #region Events


        #endregion
    }
}
