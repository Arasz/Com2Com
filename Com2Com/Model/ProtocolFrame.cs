﻿using System;
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
        public int Id { get; private set; } = 0;
        /// <summary>
        /// Data length in bytes
        /// </summary>
        public int DataLength { get; private set; } = 1;

        /// <summary>
        /// Command
        /// </summary>
        public char[] Command { get; private set; } = new char[2];

        /// <summary>
        /// Data array
        /// </summary>
        public byte[] Data { get; private set; }

        public int FrameLength { get; private set; } = 5;  

        /// <summary>
        /// CRC - Cyclic Redundancy Check.
        /// </summary>
       // public int CRC;

        public ProtocolFrame(int id, char[] command, byte[]data)
        {
            Data = data;
            DataLength = (data.Length);
            Array.Copy(command.Take(2).ToArray(), Command, 2);
            Id = id;
            FrameLength = 4 + DataLength;
        }

        public byte[] ConvertFrameToByteArray()
        {
            byte[] byteArray = new byte[FrameLength];

            int index = 0;
            byteArray[index++] = Convert.ToByte(Id);
            byteArray[index++] = Convert.ToByte(DataLength);
            Array.Copy(Command, 0, byteArray, ++index, Command.Length);
            Array.Copy(Data, 0, byteArray, (index + Command.Length), DataLength);

            return byteArray;
        }

    }
}
