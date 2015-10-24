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

namespace Com2Com.ViewModel
{
    public class LightSymbol
    {
        private Rect _baseRectangel = new Rect(0,0,20,20);

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

        public bool State { get; set; }

        public bool Contains(Point point)
        {
            return _baseRectangel.Contains(point);
        }
    }

    public class SlaveViewModel : ViewModelBase
    {
        private SlaveModel _slaveModel;

        public ObservableCollection<Rect> DigitalIoCollection { get; private set; }

        private int _slaveId = 0;
        public int SlaveId { get { return _slaveId; } private set { Set(nameof(SlaveId),ref _slaveId, value); } }

        private string _lastMessage = string.Empty;
        public string LastMessage { get { return _lastMessage; } private set { Set(nameof(LastMessage), ref _lastMessage, value); } }

        public SlaveViewModel()
        {
            // Initialization
            InitializeDigitalIoRepresentation();
            // Messaging
            MessengerInstance = Messenger.Default;
            MessengerInstance.Register<SlaveDataMessage>(this, HandleSlaveModelMessage);
            // Commands
            CreateNavigateToMasterPageCommand();
            CreateSendProtocolFrameCommand();
            CreateChangeDigitalIoState();
        }

        private void InitializeDigitalIoRepresentation()
        {
            Point delta = new Point(30, 40);

            int ioAmount = 8;

            DigitalIoCollection = new ObservableCollection<DigitalIoRepresentation>();

            for (int i = 0; i < ioAmount / 2; i++)
            {
                var newSymbol = new DigitalIoRepresentation()
                {
                    X = i * delta.X,
                };
                DigitalIoCollection.Add(newSymbol);
            }
            for (int i = 0; i <ioAmount/2; i++)
            {
                var newSymbol = new DigitalIoRepresentation()
                {
                    X = i * delta.X,
                    Y = delta.Y,
                    State = true,
                };
                DigitalIoCollection.Add(newSymbol);
            }
        }


        #region Messaging
        void HandleSlaveModelMessage(SlaveDataMessage message)
        {
            _slaveModel = message.SlaveModel;
            SlaveId = _slaveModel.SlaveID;
            // ISSUE: There was a problem with reference to the _slaveModel variable ( each time message was send new object was created)
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
        public ICommand SendProtocolFrame { get; set; }
        private void ExecuteSendProtocolFrameCommand()
        {
            MessengerInstance.Send(new SlaveDataMessage(_slaveModel));
        }
        private void CreateSendProtocolFrameCommand()
        {
            SendProtocolFrame = new RelayCommand(ExecuteSendProtocolFrameCommand);
        }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ChangeDigitalIoState { get; private set; }
        private void ExecuteChangeDigitalIoState(MouseEventArgs e)
        {
            var soruce = e.Source as ItemsControl;
            Point position = e.GetPosition(soruce);

            var clicked =
                from symbols in DigitalIoCollection
                where symbols.Contains(position)
                select symbols;

            LastMessage = $"X: {position.X}, Y: {position.Y}";

        }
        private void CreateChangeDigitalIoState()
        {
            ChangeDigitalIoState = new RelayCommand<MouseEventArgs>(ExecuteChangeDigitalIoState);
        }
        #endregion
    }
}
