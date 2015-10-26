using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com2Com.Model;

namespace Com2Com.ViewModel
{
    class SlaveDataMessage
    {
        public SlaveModel SlaveModel { get; set; }

        public bool AnalogDataChanged { get; set; }

        public bool DigitalDataChanged { get; set; }

        public SlaveDataMessage(SlaveModel slaveModel, bool analogDataChanged = false, bool digitalDataChanged = false)
        {
            SlaveModel = slaveModel;
            AnalogDataChanged = analogDataChanged;
            DigitalDataChanged = digitalDataChanged;
        }
    }
}
