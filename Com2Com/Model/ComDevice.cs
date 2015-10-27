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

        /// <summary>
        /// 
        /// </summary>
        public SettingsModel PortSettings
        {
            get { return _portSettings; }
            set
            {
                // TODO: Think about copy  
                _portSettings = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Connected { get; private set; }


        /// <summary>
        /// Opens used serial ports.
        /// </summary>
        public void Connect()
        {
            if (!string.IsNullOrEmpty(_portSettings?.PortName))
            {
                ConfigureSerialPort();
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
                        PortName = _portSettings.PortName,
                        Cause = "The port is in an invalid state. (Port parameters may not be correct)"
                    };

                }
                catch (ArgumentOutOfRangeException argoorex)
                {
                    throw new ComDeviceException(argoorex.Message, argoorex)
                    {
                        PortType = "In",
                        PortName = _portSettings.PortName,
                        Cause = "One or more of the properties for this instance are invalid. (For example wrong parity value)"
                    };
                }
                catch (ArgumentException argex)
                {
                    throw new ComDeviceException(argex.Message, argex)
                    {
                        PortType = "In",
                        PortName = _portSettings.PortName,
                        Cause = "The port name does not begin with COM."
                    };
                }
                catch (UnauthorizedAccessException unaccex)
                {
                    throw new ComDeviceException(unaccex.Message, unaccex)
                    {
                        PortType = "In",
                        PortName = _portSettings.PortName,
                        Cause = "Access is denied to the port."
                    };
                }
                catch (InvalidOperationException invopex)
                {
                    throw new ComDeviceException(invopex.Message, invopex)
                    {
                        PortType = "In",
                        PortName = _portSettings.PortName,
                        Cause = "The specified port on the current instance of the SerialPort is already open."
                    };
                }
                finally
                {
                    if (!_serialPort.IsOpen)
                        Dispose();
                    else
                        Connected = true;
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
                        PortName = _portSettings.PortName,
                        PortType = "IN",
                        Cause = "The port is in an invalid state. (Port parameters may not be correct)",
                    };
                }
                finally
                {
                    if (!_serialPort.IsOpen)
                        Connected = false;
                }
            }
        }

        /// <summary>
        /// Configure serial port with parameters included in port settings
        /// </summary>
        /// <param name="serialPort">Serial port passed</param>
        protected void ConfigureSerialPort()
        {
            if (_serialPort == null)
            {
                _serialPort = new SerialPort()
                {
                    PortName = _portSettings.PortName,
                    Parity = _portSettings.Parity,
                    BaudRate = _portSettings.BaudRate,
                    StopBits = _portSettings.StopBits,
                    DataBits = (int)_portSettings.DataBits,
                };
            }
            else
            {
                if (_serialPort.IsOpen)
                    Disconnect();

                _serialPort.PortName = _serialPort.PortName;
                _serialPort.Parity = _portSettings.Parity;
                _serialPort.BaudRate = _portSettings.BaudRate;
                _serialPort.StopBits = _portSettings.StopBits;
                _serialPort.DataBits = (int)_portSettings.DataBits;
            }

            _serialPort.DataReceived += SerialPort_DataReceived;
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
