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
        public Dictionary<int,SlaveModel> Slaves { get; private set; }

        private ProtocolFrame _lastIncomingFrame;

        public MasterDeviceModel()
        {
            Slaves = new Dictionary<int, SlaveModel>();
        }

        protected override void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _lastIncomingFrame =  ReadData();
        }

        private void UpdateSlave(ProtocolFrame frame)
        {
            int slaveId = frame.Id;
            if (Slaves.ContainsKey(slaveId))
            {

            }

            
        }

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
    }
}
