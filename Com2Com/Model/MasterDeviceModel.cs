using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Com2Com.Model
{
    public class MessageStatusChangedEventArgs: EventArgs
    {
        public ProtocolFrame Frame { get; private set; }

        public int SlaveId { get; private set; }

        public MessageStatusChangedEventArgs(ProtocolFrame frame, int slaveId = 255)
        {
            Frame = frame;
            SlaveId = slaveId;
        }
    } 

    class MasterDeviceModel : ComDevice
    {

        #region Events

        public event EventHandler< MessageStatusChangedEventArgs> SlaveUpdated;

        private void OnSlaveUpdated(ProtocolFrame frame, int slaveId)
        {
            var handler = SlaveUpdated;
            handler.Invoke(this, new MessageStatusChangedEventArgs(frame, slaveId));
        }

        public event EventHandler<MessageStatusChangedEventArgs> MessageSent;

        private void OnMessageSent(ProtocolFrame frame)
        {
            var handler = MessageSent;
            handler?.Invoke(this, new MessageStatusChangedEventArgs(frame));
        }

        #endregion

        #region Events handlers
        /// <summary>
        /// Updates slaves collection with received data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _webServiceModel_ServerDataReceived(object sender, WebServerSynchronizationEventArgs e)
        {
            var slavesStates = e.SlavesStates;
            if (slavesStates != null && Connected)
            {
                // HACK: (POSITIVE) This if causes that broadcast value don't updates to value saved on server
                if (e.SynchronizationCount <= 1)
                {
                    // Discover slaves in the network
                    SendMessageToSlave(_broadcastId);
                }
                else
                {
                    foreach (SlaveModel slave in slavesStates)
                    {
                        if (Slaves.ContainsKey(slave.SlaveId))
                        {
                            Slaves[slave.SlaveId].DigitalValue = slave.DigitalValue;
                            Slaves[slave.SlaveId].AnalogValue = slave.AnalogValue;
                            // HACK: No check ups for data change
                            SendMessageToSlave(slave.SlaveId, digitalDataChanged: true, analogDataChanged: true);
                        }
                    }
                }
            }
        }
        #endregion
        private readonly int _broadcastId = 255;

        private ProtocolFrame _lastIncomingFrame;

        private WebServiceModel _webService;

        public Dictionary<int, SlaveModel> Slaves { get; private set; }

        public MasterDeviceModel()
        {
            Slaves = new Dictionary<int, SlaveModel>() { [_broadcastId] = new SlaveModel() { SlaveId = _broadcastId } };

            _webService = new WebServiceModel();
            _webService.ServerDataReceived += _webServiceModel_ServerDataReceived;
            _webService.RunWebServerSync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _lastIncomingFrame =  ReadData();
            UpdateSlave(_lastIncomingFrame);
            OnSlaveUpdated(_lastIncomingFrame, _lastIncomingFrame.Id);
            await _webService.PostSlaveListAsync(new List<SlaveModel>() { Slaves[_lastIncomingFrame.Id] });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="analogDataChanged"></param>
        /// <param name="digitalDataChanged"></param>
        public void SendMessageToSlave(int slaveId, bool analogDataChanged = false, bool digitalDataChanged = false)
        {
            string command = "";

            if (analogDataChanged && digitalDataChanged)
                command = ("AS");
            else
            {
                if (analogDataChanged)
                    command = ("SA");
                if (digitalDataChanged)
                    command = ("SD");
            }
            if (!(digitalDataChanged || analogDataChanged))
                command = ("ID");

            ProtocolFrame protocolFrame = ConvertSlaveModelToProtocolFrame(Slaves[slaveId], command);
            SendData(protocolFrame);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slave"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private ProtocolFrame ConvertSlaveModelToProtocolFrame(SlaveModel slave, string command )
        {
            ProtocolFrame frame = null;

            switch (command)
            {
                case "SA":
                    frame = new ProtocolFrame(slave.SlaveId, command.ToCharArray(), BitConverter.GetBytes(slave.AnalogValue));
                    break;
                case "SD":
                    frame = new ProtocolFrame(slave.SlaveId, command.ToCharArray(), BitConverter.GetBytes(slave.DigitalValue));
                    break;
                case "AS":
                    var concatedArray = BitConverter.GetBytes(slave.AnalogValue).Concat(BitConverter.GetBytes(slave.DigitalValue)).ToArray();
                    frame = new ProtocolFrame(slave.SlaveId, command.ToCharArray(), concatedArray);
                    break;
                case "ID":
                    frame = new ProtocolFrame(slave.SlaveId, command.ToCharArray(), null);
                    break;
            }
            return frame;
        }
        /// <summary>
        /// Updates slave state with incoming data
        /// </summary>
        /// <param name="frame"></param>
        private void UpdateSlave(ProtocolFrame frame)
        {
            int slaveId = frame.Id;
            if (!Slaves.ContainsKey(slaveId))
                Slaves[slaveId] = new SlaveModel() { SlaveId = slaveId };

            var cmd = new string(frame.Command);
                switch(cmd)
                {
                    case "SA":
                        Slaves[slaveId].AnalogValue = BitConverter.ToInt16(frame.Data,0);
                        break;
                    case "SD":
                        Slaves[slaveId].DigitalValue = BitConverter.ToUInt16(frame.Data,0);
                        break;
                    case "AS":
                    case "ID":
                        Slaves[slaveId].AnalogValue = BitConverter.ToInt16(frame.Data, 0);
                        Slaves[slaveId].DigitalValue = BitConverter.ToUInt16(frame.Data, frame.DataLength / 2);
                        break;
                } 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        public void SendData(ProtocolFrame frame)
        {
            if (Connected)
            {
                _serialPort.Write(frame.ConvertFrameToByteArray(), 0, frame.FrameLength);
                OnMessageSent(frame);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ProtocolFrame ReadData()
        {
            if(Connected)
            {
                int id = _serialPort.ReadByte();

                int dataLength = _serialPort.ReadByte();

                char[] command = new char[2];
                _serialPort.Read(command, 0, 2);

                byte[] data = new byte[dataLength];
                _serialPort.Read(data, 0, dataLength);

                return new ProtocolFrame(id, command, data);
            }
            throw new ComDeviceException("Attempt to read data from closed serial port.")
            { Cause = "Serial port closed", PortName = _serialPort.PortName };
        }

        #region Dispose Pattern
        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
            {

            }
            else
            {

            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
