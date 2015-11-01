using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Com2Com.Model
{
    
    public class SlaveModel
    {
        [JsonProperty("di")]
        public ushort DigitalValue { get; set; } = 0x00;
        [JsonProperty("ai")]
        public short AnalogValue { get; set; } = 0;
        [JsonProperty("id")]
        public int SlaveId { get; set; } = 255;

    }
}
