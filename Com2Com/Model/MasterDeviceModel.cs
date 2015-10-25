using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com2Com.Model
{
    public class DataReadyEventArgs: EventArgs
    {
        public ProtocolFrame Frame { get; private set; }
        public int UpdatedSlaveIndex { get; private set; }

        public DataReadyEventArgs(ProtocolFrame frame, int updatedSlaveIndex)
        {
            Frame = frame;
            UpdatedSlaveIndex = updatedSlaveIndex; 
        }
    } 

    class MasterDeviceModel : ComDevice
    {

        #region Events
        public event EventHandler<DataReadyEventArgs> DataReady;

        private void OnDataReady(ProtocolFrame frame, int updatedSlaveIndex)
        {
            EventHandler<DataReadyEventArgs> handler = DataReady;
            handler?.Invoke(this, new DataReadyEventArgs(frame, updatedSlaveIndex));
        }

        #endregion

        private ProtocolFrame _lastIncomingFrame;

        private Queue<string> _outgoingCommandsQueue = new Queue<string>();

        private bool _canSend { get; set; } = true;

        public Dictionary<int, SlaveModel> Slaves { get; private set; }

        public MasterDeviceModel()
        {
            Slaves = new Dictionary<int, SlaveModel>() { [255] = new SlaveModel() { SlaveId = 255 } };
            // HACK: DUMMY SLAVE
            Slaves[44]  = new SlaveModel() { SlaveId = 44, AnalogValue = 169, DigitalValue = 200 };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _lastIncomingFrame =  ReadData();
            UpdateSlave(_lastIncomingFrame);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="analogDataChanged"></param>
        /// <param name="digitalDataChanged"></param>
        public void SendMessageToSlave(int slaveId, bool analogDataChanged = false, bool digitalDataChanged = false)
        {

            if (slaveId == 255) // BroadcastId
                _outgoingCommandsQueue.Enqueue("ID");
            else
            {
                if (analogDataChanged)
                    _outgoingCommandsQueue.Enqueue("SA");
                if (digitalDataChanged)
                    _outgoingCommandsQueue.Enqueue("SD");
                else
                    _outgoingCommandsQueue.Enqueue("ID");
            }

            while(_outgoingCommandsQueue.Count != 0)
            {
                ProtocolFrame protocolFrame = ConvertSlaveModelToProtocolFrame(Slaves[slaveId], _outgoingCommandsQueue.Dequeue());
                SendData(protocolFrame);
            }
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
                case "GA":
                    frame = new ProtocolFrame(slave.SlaveId, command.ToCharArray(), null);
                    break;
                case "SD":
                    frame = new ProtocolFrame(slave.SlaveId, command.ToCharArray(), BitConverter.GetBytes(slave.DigitalValue));
                    break;
                case "GD":
                    frame = new ProtocolFrame(slave.SlaveId, command.ToCharArray(), null);
                    break;
                case "ID":
                    frame = new ProtocolFrame(slave.SlaveId, command.ToCharArray(), null);
                    break;
            }
            return frame;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        private void UpdateSlave(ProtocolFrame frame)
        {
            int slaveId = frame.Id;
            if (!Slaves.ContainsKey(slaveId))
                Slaves[slaveId] = new SlaveModel() { SlaveId = slaveId };

            var cmd = frame.Command.ToString();
                switch(cmd)
                {
                    case "SA":
                    case "GA":
                        Slaves[slaveId].AnalogValue = Convert.ToInt16(frame.Data);
                        break;
                    case "SD":
                    case "GD":
                        Slaves[slaveId].DigitalValue = Convert.ToUInt16(frame.Data);
                        break;
                    case "ID":
                        Slaves[slaveId].DigitalValue = Convert.ToUInt16(frame.Data.Take(frame.DataLength / 2));
                        Slaves[slaveId].AnalogValue = Convert.ToInt16(frame.Data.Skip(frame.DataLength / 2));
                        break;
                } 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        public void SendData(ProtocolFrame frame)
        {
            if(Connected)
                _serialPort.Write(frame.ConvertFrameToByteArray(), 0, frame.FrameLength);
            else
            {
                Connect();
                 SendData(frame);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ProtocolFrame ReadData()
        {
            int id = _serialPort.ReadByte();

            int dataLength = _serialPort.ReadByte();

            char[] command = new char[2];
            _serialPort.Read(command, 0, 2);

            byte[] data = new byte[dataLength];
            _serialPort.Read(data, 0, dataLength);

            return new ProtocolFrame(id, command, data);
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
