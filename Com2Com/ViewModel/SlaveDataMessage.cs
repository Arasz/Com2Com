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

        public bool DataChanged { get; set; }

        public SlaveDataMessage(SlaveModel slaveModel, bool dataChanged = true)
        {
            SlaveModel = slaveModel;
            DataChanged = dataChanged;
        }
    }
}
