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

    public abstract class ComDevice : IDisposable
    {

        protected SerialPort _inSerialPort;
        protected SerialPort _outSerialPort;

        public string OutPortName { get; set; } = "";
        public string InPortName { get; set; } = "";
        public int BaudRate { get; set; } = 9600;
        public Parity Parity { get; set; } = Parity.None;
        public StopBits StopBits { get; set; } = StopBits.One;
        public DataBits DataBits { get; set; } = DataBits.Eight;

        /// <summary>
        /// Opens used serial ports.
        /// </summary>
        public void Connect()
        {
            if (!string.IsNullOrEmpty(InPortName))
            {
                ConfigureSerialPort(ref _inSerialPort, InPortName);
                try
                {
                    _inSerialPort.Open();
                }
                #region CatchExceptions
                catch (IOException ioex)
                {
                    throw new ComDeviceException(ioex.Message, ioex)
                    {
                        PortType = "In",
                        PortName = InPortName,
                        Cause = "The port is in an invalid state. (Port parameters may not be correct)"
                    };

                }
                catch (ArgumentOutOfRangeException argoorex)
                {
                    throw new ComDeviceException(argoorex.Message, argoorex)
                    {
                        PortType = "In",
                        PortName = InPortName,
                        Cause = "One or more of the properties for this instance are invalid. (For example wrong parity value)"
                    };
                }
                catch (ArgumentException argex)
                {
                    throw new ComDeviceException(argex.Message, argex)
                    {
                        PortType = "In",
                        PortName = InPortName,
                        Cause = "The port name does not begin with COM."
                    };
                }
                catch (UnauthorizedAccessException unaccex)
                {
                    throw new ComDeviceException(unaccex.Message, unaccex)
                    {
                        PortType = "In",
                        PortName = InPortName,
                        Cause = "Access is denied to the port."
                    };
                }
                catch (InvalidOperationException invopex)
                {
                    throw new ComDeviceException(invopex.Message, invopex)
                    {
                        PortType = "In",
                        PortName = InPortName,
                        Cause = "The specified port on the current instance of the SerialPort is already open."
                    };
                }
                #endregion
            }
            if (!string.IsNullOrEmpty(OutPortName))
            {
                ConfigureSerialPort(ref _outSerialPort, OutPortName);
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
                        PortName = OutPortName,
                        Cause = "The port is in an invalid state. (Port parameters may not be correct)"
                    };

                }
                catch (ArgumentOutOfRangeException argoorex)
                {
                    throw new ComDeviceException(argoorex.Message, argoorex)
                    {
                        PortType = "Out",
                        PortName = OutPortName,
                        Cause = "One or more of the properties for this instance are invalid. (For example wrong parity value)"
                    };
                }
                catch (ArgumentException argex)
                {
                    throw new ComDeviceException(argex.Message, argex)
                    {
                        PortType = "Out",
                        PortName = OutPortName,
                        Cause = "The port name does not begin with COM."
                    };
                }
                catch (UnauthorizedAccessException unaccex)
                {
                    throw new ComDeviceException(unaccex.Message, unaccex)
                    {
                        PortType = "Out",
                        PortName = OutPortName,
                        Cause = "Access is denied to the port."
                    };
                }
                catch (InvalidOperationException invopex)
                {
                    throw new ComDeviceException(invopex.Message, invopex)
                    {
                        PortType = "Out",
                        PortName = OutPortName,
                        Cause = "The specified port on the current instance of the SerialPort is already open."
                    };
                }
                #endregion

            }
        }

        /// <summary>
        /// Closes used serial ports
        /// </summary>
        public void Disconnect()
        {
            if (_inSerialPort?.IsOpen ?? false) // if serial port is != null and isOpen
            {
                try
                {
                    _inSerialPort.Close();
                }
                catch (IOException ioex)
                {
                    throw new ComDeviceException(ioex.Message, ioex)
                    {
                        PortName = InPortName,
                        PortType = "IN",
                        Cause = "The port is in an invalid state. (Port parameters may not be correct)",
                    };
                }
            }
            if (_outSerialPort?.IsOpen ?? false)
            {
                try
                {
                    _outSerialPort.Close();
                }
                catch (IOException ioex)
                {
                    throw new ComDeviceException(ioex.Message, ioex)
                    {
                        PortName = OutPortName,
                        PortType = "OUT",
                        Cause = "The port is in an invalid state. (Port parameters may not be correct)",
                    };
                }
            }
        }

        protected void ConfigureSerialPort(ref SerialPort serialPort, string name)
        {
            if (serialPort == null)
            {
                serialPort = new SerialPort()
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
                if (serialPort.IsOpen)
                    serialPort.Close();

                serialPort.PortName = name;
                serialPort.Parity = Parity;
                serialPort.BaudRate = BaudRate;
                serialPort.StopBits = StopBits;
                serialPort.DataBits = (int)DataBits;
            }

            serialPort.DataReceived += SerialPort_DataReceived;
        }

        protected abstract void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e);

        #region Basic Dispose Pattern

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
                if (disposing == true)
                {
                // TODO: Think about closing port before disposing 
                    _inSerialPort?.Dispose();
                    _outSerialPort?.Dispose();

                    _inSerialPort = null;
                    _outSerialPort = null;

                }
                else
                {

                }
        }

        #endregion
    }
}
