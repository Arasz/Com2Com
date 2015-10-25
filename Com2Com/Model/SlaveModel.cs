using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com2Com.Model
{
    public class SlaveModel
    {
        public uint DigitalValue { get; set; } = 0x00;

        public double AnalogValue { get; set; } = 0;

        public int SlaveId { get; set; } = 255;

    }
}
