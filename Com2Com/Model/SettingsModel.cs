using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com2Com.Model
{
    public class SettingsModel
    {
        public string PortName = "";
        public int BaudRate = 9600;
        public Parity Parity = Parity.None;
        public StopBits StopBits = StopBits.One;
        public DataBits DataBits = DataBits.Eight;
    }
}
