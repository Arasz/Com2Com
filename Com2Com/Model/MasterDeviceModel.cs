using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com2Com.Model
{
    public class FrameEventArgs: EventArgs
    {
        public ProtocolFrame Frame { get; private set; }

        public FrameEventArgs(ProtocolFrame frame)
        {
            Frame = frame;
        }
    } 

    class MasterDeviceModel : ComDevice
    {

        #region Events

        public event EventHandler< FrameEventArgs> SlaveUpdated;

        private void OnSlaveUpdated(ProtocolFrame frame)
        {
            var handler = SlaveUpdated;
            handler.Invoke(this, new FrameEventArgs(frame));
        }

        public event EventHandler<FrameEventArgs> MessageSent;

        private void OnMessageSent(ProtocolFrame frame)
        {
            var handler = MessageSent;
            handler?.Invoke(this, new FrameEventArgs(frame));
        }

        #endregion

        private ProtocolFrame _lastIncomingFrame;

        private Queue<string> _outgoingCommandsQueue = new Queue<string>();


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
            OnSlaveUpdated(_lastIncomingFrame);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="analogDataChanged"></param>
        /// <param name="digitalDataChanged"></param>
        public void SendMessageToSlave(int slaveId, bool analogDataChanged = false, bool digitalDataChanged = false)
        {
            // TODO: Delete queue
            if (analogDataChanged && digitalDataChanged)
                _outgoingCommandsQueue.Enqueue("AS");
            else
            {
                if (analogDataChanged)
                    _outgoingCommandsQueue.Enqueue("SA");
                if (digitalDataChanged)
                    _outgoingCommandsQueue.Enqueue("SD");
            }
            if (!(digitalDataChanged || analogDataChanged))
                _outgoingCommandsQueue.Enqueue("ID");

            while (_outgoingCommandsQueue.Count != 0)
            {
                ProtocolFrame protocolFrame = ConvertSlaveModelToProtocolFrame(Slaves[slaveId], _outgoingCommandsQueue.Dequeue());
                SendData(protocolFrame);
                OnMessageSent(protocolFrame);
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
                        Slaves[slaveId].AnalogValue = Convert.ToInt16(frame.Data);
                        break;
                    case "SD":
                        Slaves[slaveId].DigitalValue = Convert.ToUInt16(frame.Data);
                        break;
                    case "AS":
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
