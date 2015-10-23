using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com2Com.Model
{
    class MasterDeviceModel : ComDevice
    {
        protected override void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
