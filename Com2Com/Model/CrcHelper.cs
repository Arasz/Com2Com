using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com2Com.Model
{
    public static class CrcHelper
    {
        /// <summary>
        /// CRC number length in bits
        /// </summary>
        readonly static uint n = 8;

        /// <summary>
        /// CRC divisor
        /// </summary>
        private static int _crcPolynomial = 0x32;

        public static int CalculateCRC(byte[] data)
        {
            byte[] crcArray = new byte[data.Length + 1]; // Last element is equal to zero by default
            data.CopyTo(crcArray, 0);

            // Main calculation loop

            return 0;
        }

        public static bool CheckeDataIntegirity(byte[] data, int CRC)
        {
            return true;
        }
    }
}
