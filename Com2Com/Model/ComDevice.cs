using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;

namespace Com2Com.Model
{
    public enum DataBits
    {
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
    }

    public class ComDeviceException : Exception
    {
        public string PortType { get; set; }
        public string PortName { get; set; }
        public string Cause { get; set; }

        public ComDeviceException()
        {
        }

        public ComDeviceException(string message) : base(message)
        {
        }

        public ComDeviceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public abstract class ComDevice
    {
        protected SerialPort _inSerialPort;
        protected SerialPort _outSerialPort;

        public string OutComName { get; set; } = "";
        public string InComName { get; set; } = "";
        public int BaudRate { get; set; } = 9600;
        public Parity Parity { get; set; } = Parity.None;
        public StopBits StopBits { get; set; } = StopBits.One;
        public DataBits DataBits { get; set; } = DataBits.Eight;

        public void Connect()
        {
            if(!string.IsNullOrEmpty(InComName))
            {
                ConfigureSerialPort(ref _inSerialPort, InComName);
                try
                {
                    _inSerialPort.Open();
                }
                #region CatchExceptions
                catch (IOException ioex)
                {
                    throw new ComDeviceException(ioex.Message,ioex)
                    {
                        PortType = "In",
                        PortName = InComName,
                        Cause = "The port is in an invalid state. (Port parameters may not be correct)"
                    };

                }
                catch (ArgumentOutOfRangeException argoorex)
                {
                    throw new ComDeviceException(argoorex.Message, argoorex)
                    {
                        PortType = "In",
                        PortName = InComName,
                        Cause = "The port is in an invalid state. (Port parameters may not be correct)"
                    };
                }
                catch (ArgumentException argex)
                {
                    throw new ComDeviceException(argex.Message, argex)
                    {
                        PortType = "In",
                        PortName = InComName,
                        Cause = "The port name does not begin with COM."
                    };
                }
                catch (UnauthorizedAccessException unaccex)
                {
                    throw new ComDeviceException(unaccex.Message, unaccex)
                    {
                        PortType = "In",
                        PortName = InComName,
                        Cause = "Access is denied to the port."
                    };
                }
                catch (InvalidOperationException invopex)
                {
                    throw new ComDeviceException(invopex.Message, invopex)
                    {
                        PortType = "In",
                        PortName = InComName,
                        Cause = "The specified port on the current instance of the SerialPort is already open."
                    };
                }
                #endregion
            }
            if (!string.IsNullOrEmpty(OutComName))
            {
                ConfigureSerialPort(ref _outSerialPort, OutComName);
                try
                {
                    _outSerialPort.Open();
                }
                #region CatchExceptions
                catch (IOException ioex)
                {
                    throw new ComDeviceException(ioex.Message, ioex)
                    {
                        PortType = "Out",
                        PortName = OutComName,
                        Cause = "The port is in an invalid state. (Port parameters may not be correct)"
                    };

                }
                catch (ArgumentOutOfRangeException argoorex)
                {
                    throw new ComDeviceException(argoorex.Message, argoorex)
                    {
                        PortType = "Out",
                        PortName = OutComName,
                        Cause = "The port is in an invalid state. (Port parameters may not be correct)"
                    };
                }
                catch (ArgumentException argex)
                {
                    throw new ComDeviceException(argex.Message, argex)
                    {
                        PortType = "Out",
                        PortName = OutComName,
                        Cause = "The port name does not begin with COM."
                    };
                }
                catch (UnauthorizedAccessException unaccex)
                {
                    throw new ComDeviceException(unaccex.Message, unaccex)
                    {
                        PortType = "Out",
                        PortName = OutComName,
                        Cause = "Access is denied to the port."
                    };
                }
                catch (InvalidOperationException invopex)
                {
                    throw new ComDeviceException(invopex.Message, invopex)
                    {
                        PortType = "Out",
                        PortName = OutComName,
                        Cause = "The specified port on the current instance of the SerialPort is already open."
                    };
                }
                #endregion

            }
        }

        protected void ConfigureSerialPort(ref SerialPort port, string name)
        {
            if (port == null)
            {
                port = new SerialPort()
                {
                    PortName = name,
                    Parity = this.Parity,
                    BaudRate = this.BaudRate,
                    StopBits = this.StopBits,
                    DataBits = (int)this.DataBits,
                };
            }
            else
            {
                if (port.IsOpen)
                    port.Close();

                port.PortName = name;
                port.Parity = Parity;
                port.BaudRate = BaudRate;
                port.StopBits = StopBits;
                port.DataBits = (int)DataBits;
            }

        }
    }
}
