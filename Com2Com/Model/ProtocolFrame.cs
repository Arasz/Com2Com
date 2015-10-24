using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com2Com.Model
{
    public class ProtocolFrame
    {
        /// <summary>
        /// Slave id number
        /// </summary>
        public byte Id = 0;
        /// <summary>
        /// Data length in bytes
        /// </summary>
        public byte DataLength = 1;

        /// <summary>
        /// Command
        /// </summary>
        public char[] Command = new char[2];

        /// <summary>
        /// Data array
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// CRC - Cyclic Redundancy Check.
        /// </summary>
        public int CRC;

        public ProtocolFrame()
        {
            Data = new byte[DataLength];
        }

        public ProtocolFrame(byte id, char[]command, byte dataLength, byte[]data)
        {
            Data = data;
            command.CopyTo(Command,0);
            Id = id;
            DataLength = dataLength;
        }

    }
}
