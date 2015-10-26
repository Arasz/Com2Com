using Com2Com.Model;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com2Com.ViewModel
{
    public class ProtocolFrameMessage : MessageBase
    {
        public ProtocolFrame Frame { get; set; }

        public ProtocolFrameMessage(ProtocolFrame frame)
        {
            Frame = frame;
        }
    }
}
