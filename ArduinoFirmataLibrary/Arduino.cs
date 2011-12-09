// Copyright (c) 2009 Tasos Valsamidis
// Contributions by Noriaki Mitsunaga
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;

namespace ArduinoFirmataLibrary
{
    public delegate void NewAnalogValueIsAvailableHandler(AnalogPinMessage newValue);
    public delegate void NewDigitalValueIsAvailableHandler(DigitalPortMessage newValue);

    public abstract class Arduino : IDisposable
    {
        #region Private Fields
        /// <summary>
        /// The serial port that will be used for all communication
        /// </summary>
        private SerialPort serialPort;

        /// <summary>
        /// The possible states of the Firmata protocol
        /// </summary>
        private enum FirmataState
        {
            NONE = 0,
            WAIT_ONE_PARAM_CMD_1 = 1,
            WAIT_TWO_PARAM_CMD_1 = 2,
            WAIT_TWO_PARAM_CMD_2 = 3,
            WAIT_SYSEX_DATA = 4
        }

        /// <summary>
        /// The current state on the Firmata protocol
        /// </summary>
        private FirmataState firmataState = FirmataState.NONE;
        
        private int firmataMessage = 0;
        private byte[] msgBuf = new byte[Firmata.MAX_DATA_BYTES];
        private int msgLength = 0;

        /// <summary>
        /// This array holds the pin mapping for analog reporting. This can only be changed in the standby state (for now)
        /// </summary>
        private readonly bool[] analogReportState = new bool[Firmata.MAX_ANALOG_PINS];

        /// <summary>
        /// This array holds the pin mapping for digital reporting. This can only be changed in the standby state (for now)
        /// </summary>
        private readonly bool[] digitalReportState = new bool[Firmata.MAX_DIGITAL_PORTS];


        private int[] analogs = new int[Firmata.MAX_ANALOG_PINS];
        private int[] port_inputs = new int[Firmata.MAX_DIGITAL_PORTS];
        private int[] port_outputs = new int[Firmata.MAX_DIGITAL_PORTS];

        private Int64 rxbytes = 0;
        private Int64 txbytes = 0;

        //////////////////////////////////////////////////////
        //                     Flags                       ///
        //////////////////////////////////////////////////////
        private bool firmwareVersionResponded = false;
        private bool capabilityResponded = false;
        private bool pinStateResponded = false;
        private bool analogMappingResponded = false;
        private bool protocolVersionResponded = false;
        #endregion

        #region Properties
        /// <summary>
        /// The current protocol version running at the device
        /// </summary>
        public string ProtocolVersion { get; private set; }

        /// <summary>
        /// The current firmware version running at the device
        /// </summary>
        public string FirmwareVersion { get; private set; }

        /// <summary>
        /// The current firmware name running at the device
        /// </summary>
        public string FirmwareName { get; private set; }

        /// <summary>
        /// The pins list of the device
        /// </summary>
        public IList<Pin> Pins { get; private set; }
        #endregion

        /// <summary>
        /// This is an event that fires whenever a new analog value has come through the
        /// serial port.
        /// </summary>
        public event NewAnalogValueIsAvailableHandler NewAnalogValueIsAvailable;

        /// <summary>
        /// This is an event that fires whenever a new digital value has come through the
        /// serial port. 
        /// </summary>
        public event NewDigitalValueIsAvailableHandler NewDigitalValueIsAvailable;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Arduino class giving information about the
        /// connection parameters, as they are set up in the arduino sketch. The arduino
        /// must be running the Firmata program
        /// </summary>
        /// <param name="portName">The name of the assigned COM port</param>
        /// <param name="baudRate">The data relay speed</param>
        /// <param name="parity">One of the parity values, usually none</param>
        /// <param name="databits">The number of bits, usually 8</param>
        /// <param name="stopBits">The number of stopbits, usually 1</param>
        protected Arduino(string portName, int baudRate, Parity parity, int databits, StopBits stopBits)
        {
            Pins = new List<Pin>();

            Pins = new Pin[Firmata.MAX_DIGITAL_PINS];
            for (int i=0; i<Pins.Count; i++)
                Pins[i] = new Pin();

            try
            {
                serialPort = new SerialPort(portName, baudRate, parity, databits, stopBits);
                // Open the port
                serialPort.Open();
            }
            catch (IOException ioException)
            {
                Console.WriteLine(ioException);
                Dispose();
                return;
            }

            serialPort.DataReceived += SerialPortDataReceived;
        }

        /// <summary>
        /// Initializes a new instance of the Arduino class giving information about the
        /// port name, all the other connection parameters get their default values.
        /// The arduino must be running the Firmata program. 
        /// Default Values: baudrate=57600, parity=none, databits=8, stopbits=1
        /// </summary>
        /// <param name="portName">The name of the assigned COM port</param>
        protected Arduino(string portName) : this(portName, 57600, Parity.None, 8, StopBits.One) { }

        #endregion


        /// <summary>
        /// Initialize the Arduino
        /// </summary>
        /// <returns></returns>
        public int Init()
        {
            // Stop previous 
            StopReceivingReports();
            for (int i = 0; i < 100 && !protocolVersionResponded; i++)
            {
                MQueryProtocolVersion();
                System.Threading.Thread.Sleep(100);
            }
            if (!protocolVersionResponded)
                return -1;
            MQueryFirmwareVersion();
            MQueryCapability();
            MQueryAnalogMapping();

            for (int i = 0; i < 200 && !(firmwareVersionResponded && capabilityResponded && analogMappingResponded); i++)
            {
                System.Threading.Thread.Sleep(10);
            }

            for (int i = 0; i < Firmata.MAX_DIGITAL_PINS; i++)
            {
                MQueryPinState(i);
                for (int j = 0; j < 200 && !pinStateResponded; j++)
                {
                    System.Threading.Thread.Sleep(1);
                }
            }

            for (int i = 0; i < Firmata.MAX_DIGITAL_PORTS; i++)
                SetDigitalReportStateForPort((byte)i, true);
            for (int i = 0; i < Firmata.MAX_ANALOG_PINS; i++)
                SetAnalogReportStateForPin(i,true);
            UpdateReportsToReceive();

            return 0;
        }


        /// <summary>
        /// Set the report state for the pin specified. This method works only in the standby state
        /// </summary>
        /// <param name="pin">The pin whose analog report state we want to change. Valid pin are from
        /// 0 to 5</param>
        /// <param name="newState">The new state</param>
        public void SetAnalogReportStateForPin(int pin, bool newState)
        {
            // Check if the pin is valid
            if (pin < 0 || pin >= Firmata.MAX_ANALOG_PINS)
                throw new ArduinoException(ArduinoErrorCodes.INVALIDPIN);

            analogReportState[pin] = newState;
        }

        /// <summary>
        /// Set the report state for the pin specified. This method works only in the standby state
        /// </summary>
        /// <param name="port">The port whose digital report state we want to change. Valid ports are from
        /// 0 to 2</param>
        /// <param name="newState">The new state</param>
        public void SetDigitalReportStateForPort(byte port, bool newState)
        {
            // Check if the pin is valid
            if (port < 0 || port >= Firmata.MAX_DIGITAL_PORTS)
                throw new ArduinoException(ArduinoErrorCodes.INVALIDVALUE);

            digitalReportState[port] = newState;
        }

        public void UpdateReportsToReceive()
        {
                for (int i = 0; i < analogReportState.Length; i++)
                {
                    MReportAnalogPin(i, analogReportState[i]);
                }

                for (int i = 0; i < digitalReportState.Length; i++)
                {
                    MReportDigitalPort(i, digitalReportState[i]);
                }
        }

        public void StopReceivingReports()
        {
                for (int i = 0; i < analogReportState.Length; i++)
                {
                    MReportAnalogPin(i, false);
                }

                for (int i = 0; i < digitalReportState.Length; i++)
                {
                    MReportDigitalPort(i, false);
                }
        }

        #region Arduino IDE like functions

        protected int AnalogRead(int pin)
        {
            if (pin < 0 || pin >= Firmata.MAX_ANALOG_PINS)
                return -1;

            return analogs[pin];
        }

        protected void AnalogWrite(int pin, int value)
        {
            if (pin < 0 || pin >= Firmata.MAX_ANALOG_PINS)
                return;

            // Check if we have a percentile value
            if (value < 0)
                value = 0;
            else if (value > 0x3fff)
                value = 0x3fff;

            // Check if we are already in pwm mode
            if (Pins[pin].CurrentMode != PinModes.PWM)
            {
                // If not try to see if the pin supports pwm
                if (!Pins[pin].SupportedModes.Contains(PinModes.PWM))
                    return;

                MSetPinMode(pin, PinModes.PWM);
            }

            // Set the value
            MAnalogIOMessage(pin, value);
            Pins[pin].Output = value;
        }

        protected int DigitalRead(int pin)
        {
            return Pins[pin].Input;
        }

        protected void DigitalWrite(int pin, int value)
        {
            byte port = (byte)(pin/8);

            if (pin < 0 || pin >= Firmata.MAX_DIGITAL_PINS)
                return;
            DigitalWrite_((byte)pin, value);
            DigitalWritePort(port, (byte)port_outputs[port]);
        }

        private void DigitalWrite_(int pin, int value)
        {
            // Do nothing if we have wrong pin number
            if (pin<0 || pin >= Firmata.MAX_DIGITAL_PINS)
                return; 

            // Get the port and the pin position in this port
            int port = pin / 8;
            int pinInPort = pin % 8;

            Pins[pin].Output = value == 0 ? 0 : 1;
            if (value == 0)
                port_outputs[port] &=  ~(1 << pinInPort); 
            else
                port_outputs[port] |= 1 << pinInPort;
        }

        private void DigitalWritePort(int port, byte pins)
        {
            if (port < 0 || port >= Firmata.MAX_DIGITAL_PORTS)
                return;
            MDigitalIOMessage((byte)port, pins);
        }

        public void SetPinMode(int pin, PinModes mode)
        {
            MSetPinMode(pin, mode);
        }

        public void ServoAttach(int pin, int min, int max, int ang)
        {
            if (pin < 0 || pin >= Firmata.MAX_DIGITAL_PINS)
                return;
            if (!Pins[pin].SupportedModes.Contains(PinModes.Servo))
                return;
            
            MServoConfig(pin, min, max, ang);
            MSetPinMode(pin, PinModes.Servo);
            Pins[pin].CurrentMode = PinModes.Servo;
        }

        public void ServoAttach(int pin)
        { 
            ServoAttach(pin, 544, 2400, 90);
        }

        public void ServoWrite(int pin, int Value)
        {
            if (Pins[pin].CurrentMode == PinModes.Servo)
            {
                MAnalogIOMessage((byte)pin, Value);
                Pins[pin].Output = Value;
            }
        }
        #endregion 

        #region Functions to handle serial port
        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (serialPort)
            {

                while (serialPort.IsOpen && serialPort.BytesToRead > 0)
                {
                    int c = serialPort.ReadByte();
                    rxbytes++;

                    // Console.WriteLine(firmataState);
                    // Console.WriteLine(c);
                    if (firmataState == FirmataState.WAIT_SYSEX_DATA &&
                        (c != Firmata.SYSEX_END) && (c & 0x80) != 0)
                    {
                        firmataState = FirmataState.NONE;
                    }
                    switch (firmataState)
                    {
                        case FirmataState.NONE:
                            if ((c & 0x80) == 0)    // Ignore data byte in this state
                                break;
                            firmataMessage = c;
                            if ((firmataMessage & 0xf0) == Firmata.ANALOG_MESSAGE ||
                                (firmataMessage & 0xf0) == Firmata.DIGITAL_MESSAGE ||
                                firmataMessage == Firmata.PROTOCOL_VERSION)
                                firmataState = FirmataState.WAIT_TWO_PARAM_CMD_1;
                            else if (firmataMessage == Firmata.SYSEX_START)
                            {
                                firmataState = FirmataState.WAIT_SYSEX_DATA;
                                msgLength = 0;
                            }
                            break;
                        case FirmataState.WAIT_ONE_PARAM_CMD_1:
                            if ((c & 0x80) != 0)    // Got command -> error
                            {
                                firmataState = FirmataState.NONE;
                                break;
                            }
                            // Do something when we recieve one parameter message

                            firmataState = FirmataState.NONE;
                            break;
                        case FirmataState.WAIT_TWO_PARAM_CMD_1:
                            if ((c & 0x80) != 0)    // Got command -> error
                            {
                                firmataState = FirmataState.NONE;
                                break;
                            }
                            msgBuf[0] = (byte)c;
                            firmataState = FirmataState.WAIT_TWO_PARAM_CMD_2;
                            break;
                        case FirmataState.WAIT_TWO_PARAM_CMD_2:
                            if ((c & 0x80) != 0)    // Got command -> error
                            {
                                firmataState = FirmataState.NONE;
                                break;
                            }
                            msgBuf[1] = (byte)c;

                            if ((firmataMessage & 0xf0) == Firmata.ANALOG_MESSAGE)
                            {
                                int tempPin = (firmataMessage & 0xf);
                                int tempValue = BitHelper.Sevens2Fourteen(msgBuf[0], msgBuf[1]);

                                analogs[tempPin] = tempValue;

                                // Check if we have someone who has subscribed to the newanalogvalueisavailable event
                                if (NewAnalogValueIsAvailable != null)
                                {
                                    // Sent the newly read value for the current pin to the subscribers
                                    NewAnalogValueIsAvailable(
                                        new AnalogPinMessage
                                        {
                                            Value = tempValue,
                                            Pin = tempPin
                                        });
                                }
                            }
                            else if ((firmataMessage & 0xf0) == Firmata.DIGITAL_MESSAGE)
                            {
                                int port = (firmataMessage & 0xf);
                                int tempValue = BitHelper.Sevens2Fourteen(msgBuf[0], msgBuf[1]);
                                int[] pins_ = BitHelper.PortVal2PinVals((byte)tempValue);

                                port_inputs[port] = tempValue;
                                for (int i = 0; i < 8; i++)
                                {
                                    Pins[port * 8 + i].Input = (int)((tempValue >> i) & 1);
                                }

                                // Check if we have someone who has subscribed to the newdigitalvalueisavailable event
                                if (NewDigitalValueIsAvailable != null)
                                {
                                    // Sent the newly read value for the current port to the subscribers
                                    NewDigitalValueIsAvailable(
                                        new DigitalPortMessage
                                        {
                                            Port = port,
                                            Pins = pins_
                                        });
                                }
                            }
                            else if (firmataMessage == Firmata.PROTOCOL_VERSION)
                            {
                                ProtocolVersion = string.Format("{0}.{1}", msgBuf[0], msgBuf[1]);
                                protocolVersionResponded = true;
                            }
                            firmataState = FirmataState.NONE;
                            break;
                        case FirmataState.WAIT_SYSEX_DATA:
                            if (c != Firmata.SYSEX_END)
                            {
                                if (msgLength >= Firmata.MAX_DATA_BYTES)
                                {
                                    firmataState = FirmataState.NONE;
                                    break;
                                }
                                msgBuf[msgLength] = (byte)c;
                                msgLength++;
                            }
                            else // c == FirmataCommands.SYSEX_END
                            {
                                if (msgBuf[0] == Firmata.SYSEX_QUERY_FIRMWARE && msgLength > 1)
                                {
                                    FirmwareVersion = string.Format("{0}.{1}", msgBuf[1], msgBuf[2]);
                                    var firm_name = new StringBuilder();
                                    for (int i = 3; i < msgLength; i += 2)
                                    {
                                        int s = BitHelper.Sevens2Fourteen(msgBuf[i], msgBuf[i + 1]);
                                        firm_name.Append((char)s);
                                    }
                                    FirmwareName = firm_name.ToString();
                                    firmwareVersionResponded = true;
                                }
                                else if (msgBuf[0] == (byte)Firmata.SYSEX_CAPBILITY_RESPONSE)
                                {
                                    int p = 0;

                                    for (int i = 1; i < msgLength; i++, p++)
                                    {
                                        while (msgBuf[i] != 127)
                                        {
                                            var mode = (PinModes) msgBuf[i++];
                                            Pins[p].SupportedModes.Add(mode);
                                            Pins[p].SupportedResolution.Add(mode, msgBuf[i++]);
                                        }
                                    }
                                    capabilityResponded = true;
                                }
                                else if (msgBuf[0] == (byte)Firmata.SYSEX_ANALOG_MAPPING_RESPONSE)
                                {
                                    for (int i = 1; i < msgLength; i++)
                                    {
                                        if (msgBuf[i] < 127)
                                            Pins[i - 1].AnalogPin = msgBuf[i];
                                        else
                                            Pins[i - 1].AnalogPin = -1;
                                    }
                                    analogMappingResponded = true;
                                }
                                else if (msgBuf[0] == (byte)Firmata.SYSEX_PIN_STATE_RESPONSE)
                                {
                                    int pno = msgBuf[1];
                                    Pins[pno].CurrentMode = (PinModes)msgBuf[2];
                                    // System.Diagnostics.Debug.WriteLine(pno+": "+Pins[pno].CurrentMode);
                                    int tmp = 0;
                                    for (int i = 3; i < msgLength; i++)
                                        tmp |= (msgBuf[i] << (i - 3) * 7);
                                    Pins[pno].Output = tmp;
                                    if (Pins[pno].CurrentMode == PinModes.Output)
                                        DigitalWrite_(pno, Pins[pno].Output);
                                    pinStateResponded = true;
                                }
                                firmataState = FirmataState.NONE;
                            }

                            break;
                        default:
                            firmataState = FirmataState.NONE;
                            break;

                    }
                }
            }
        }

        private void SerialWrite(byte[] buf, int sz)
        {
            if (serialPort == null || !serialPort.IsOpen)
                return;

            try
            {
                // Write the command to change the pin mode
                serialPort.Write(buf, 0, sz);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new ArduinoException(ArduinoErrorCodes.UNKNOWNERROR);
            }
            finally
            {
                txbytes += sz;
            }
        }
        #endregion

        #region Functions to send Firmata messages
        protected void MAnalogIOMessage(int pin, int value)
        {
            // Translate the value to the format wanted by the firmata library
            byte LSB, MSB;
            BitHelper.Fourteen2Sevens(value, out LSB, out MSB);

            // We take the 4 MSB from the command type and the 4 LSB from the pin
            byte writeCommand = (byte)(Firmata.ANALOG_MESSAGE | pin);

            // Write the command to enable the analog output for the pin we want
            SerialWrite(new byte[] { writeCommand, LSB, MSB }, 3);
        }

        protected void MDigitalIOMessage(byte port, byte portValue)
        {
            // Translate the value to the format wanted by the firmata library
            byte LSB, MSB;
            BitHelper.Fourteen2Sevens(portValue, out LSB, out MSB);

            // We take the 4 MSB from the command type and the 4 LSB from the pin
            byte writeCommand = (byte)(Firmata.DIGITAL_MESSAGE | port);

            // Write the command to enable the analog output for the pin we want
            SerialWrite(new byte[] { writeCommand, LSB, MSB }, 3);
        }

        protected void MQueryAnalogMapping()
        {
            // Send a query to find out the analog mappings
            SerialWrite(new byte[] { Firmata.SYSEX_START, Firmata.SYSEX_ANALOG_MAPPING_QUERY, Firmata.SYSEX_END }, 3);
            analogMappingResponded = false;
        }

        protected void MQueryCapability()
        {
            SerialWrite(new byte[] { Firmata.SYSEX_START, Firmata.SYSEX_CAPBILITY_QUERY, Firmata.SYSEX_END }, 3);
            capabilityResponded = false;
        }

        protected void MQueryFirmwareVersion()
        {
            SerialWrite(new byte[] { Firmata.SYSEX_START, Firmata.SYSEX_QUERY_FIRMWARE, Firmata.SYSEX_END }, 3);
            firmwareVersionResponded = false;
        }

        protected void MQueryPinState(int i)
        {
            if (i < 0 || i >= Firmata.MAX_DIGITAL_PINS)
                return;
            SerialWrite(new byte[] { Firmata.SYSEX_START, Firmata.SYSEX_PIN_STATE_QUERY, (byte)i, Firmata.SYSEX_END }, 4);
            pinStateResponded = false;
        }

        protected void MQueryProtocolVersion()
        {
            SerialWrite(new byte[] { Firmata.PROTOCOL_VERSION }, 1);
            protocolVersionResponded = false;
        }

        protected void MReportAnalogPin(int pin, bool shouldEnable)
        {
            byte readCommand = (byte)(Firmata.REPORT_ANALOG_PIN | pin);
            if (shouldEnable)
                SerialWrite(new byte[] { readCommand, 1 }, 2);
            else
                SerialWrite(new byte[] { readCommand, 0 }, 2);
        }

        protected void MReportDigitalPort(int port, bool shouldEnable)
        {
            byte Command = (byte)(Firmata.REPORT_DIGITAL_PORT | port);
            if (shouldEnable)
                SerialWrite(new byte[] { Command, 1 }, 2);
            else
                SerialWrite(new byte[] { Command, 0 }, 2);
        }

        protected void MReset()
        {
            SerialWrite(new byte[] { Firmata.SYSTEM_RESET }, 1);
        }

        protected void MSamplingInterval(int interval)
        {
            byte intL, intM;
            BitHelper.Fourteen2Sevens(interval, out intL, out intM);
 
            SerialWrite(new byte[] { Firmata.SYSEX_START,
                                     Firmata.SYSEX_SAMPLING_INTERVAL,
                                     intL, intM,
                                     Firmata.SYSEX_END}, 10);
        }

        protected void MServoConfig(int pin, int min, int max, int ang)
        {
            if (pin < 0 || pin >= Firmata.MAX_DIGITAL_PINS)
                throw new ArduinoException(ArduinoErrorCodes.INVALIDPIN); 
            
            byte minL, minM, maxL, maxM, angL, angM;
            BitHelper.Fourteen2Sevens(min, out minL, out minM);
            BitHelper.Fourteen2Sevens(max, out maxL, out maxM);
            BitHelper.Fourteen2Sevens(ang, out angL, out angM);

            SerialWrite(new byte[] { Firmata.SYSEX_START,
                                     Firmata.SYSEX_SERVO_CONFIG,
                                     (byte)pin,
                                     minL, minM, maxL, maxM,
                                     angL, angM,
                                     Firmata.SYSEX_END}, 10);
        }

        protected void MSetPinMode(int pin_, PinModes mode_)
        {
            byte pin = (byte)pin_;
            byte mode = (byte)mode_;

            if (pin<0 || pin >= Firmata.MAX_DIGITAL_PINS)
                throw new ArduinoException(ArduinoErrorCodes.INVALIDPIN);
            
            SerialWrite(new byte[] { Firmata.SET_PIN_MODE, pin, mode }, 3);
            Pins[pin].CurrentMode = mode_;
        }
        #endregion

        #region Dispose Functionality
        //Implement IDisposable.
        public void Dispose() 
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        protected virtual void Dispose(bool disposing) 
        {
            StopReceivingReports();
            lock (serialPort)
            {
                if (disposing)
                {
                    serialPort.DataReceived -= SerialPortDataReceived;
                    // Free other state (managed objects).
                    if (serialPort != null)
                    {
                        serialPort.Close();
                        serialPort.Dispose();
                    }
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                serialPort = null;
            }
        }

        // Use C# destructor syntax for finalization code.
        ~Arduino()
        {
            Dispose (false);
        }
        #endregion
    }
}
