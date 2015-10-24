using Com2Com.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com2Com.ViewModel
{
    class SerialPortSettingsMessage
    {
        public SettingsModel SerialPortSettings{ get; set; }

        public SerialPortSettingsMessage(SettingsModel serialPortSettings)
        {
            SerialPortSettings = serialPortSettings;
        }
    }
}
