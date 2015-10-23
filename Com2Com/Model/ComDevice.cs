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

        protected SerialPort _serialPort;
        protected SettingsModel _portSettings;


        public string PortName { get; set; } = "";
        public int BaudRate { get; set; } = 9600;
        public Parity Parity { get; set; } = Parity.None;
        public StopBits StopBits { get; set; } = StopBits.One;
        public DataBits DataBits { get; set; } = DataBits.Eight;

        /// <summary>
        /// Opens used serial ports.
        /// </summary>
        public void Connect()
        {
            if (!string.IsNullOrEmpty(PortName))
            {
                ConfigureSerialPort(ref _serialPort, PortName);
                try
                {
                    _serialPort.Open();
                }
                #region CatchExceptions
                catch (IOException ioex)
                {
                    throw new ComDeviceException(ioex.Message, ioex)
                    {
                        PortType = "In",
                        PortName = PortName,
                        Cause = "The port is in an invalid state. (Port parameters may not be correct)"
                    };

                }
                catch (ArgumentOutOfRangeException argoorex)
                {
                    throw new ComDeviceException(argoorex.Message, argoorex)
                    {
                        PortType = "In",
                        PortName = PortName,
                        Cause = "One or more of the properties for this instance are invalid. (For example wrong parity value)"
                    };
                }
                catch (ArgumentException argex)
                {
                    throw new ComDeviceException(argex.Message, argex)
                    {
                        PortType = "In",
                        PortName = PortName,
                        Cause = "The port name does not begin with COM."
                    };
                }
                catch (UnauthorizedAccessException unaccex)
                {
                    throw new ComDeviceException(unaccex.Message, unaccex)
                    {
                        PortType = "In",
                        PortName = PortName,
                        Cause = "Access is denied to the port."
                    };
                }
                catch (InvalidOperationException invopex)
                {
                    throw new ComDeviceException(invopex.Message, invopex)
                    {
                        PortType = "In",
                        PortName = PortName,
                        Cause = "The specified port on the current instance of the SerialPort is already open."
                    };
                }
                finally
                {
                    if (!_serialPort.IsOpen)
                        Dispose();
                }
                #endregion
            }
        }

        /// <summary>
        /// Closes used serial ports
        /// </summary>
        public void Disconnect()
        {
            if (_serialPort?.IsOpen ?? false) // if serial port is != null and isOpen
            {
                try
                {
                    _serialPort.Close();
                }
                catch (IOException ioex)
                {
                    throw new ComDeviceException(ioex.Message, ioex)
                    {
                        PortName = PortName,
                        PortType = "IN",
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
                    Disconnect();

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
                    _serialPort?.Dispose();

                    _serialPort = null;
                }
                else
                {

                }
        }

        #endregion
    }
}
