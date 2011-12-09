//Copyright (c) 2009 Tasos Valsamidis
//
//Permission is hereby granted, free of charge, to any person
//obtaining a copy of this software and associated documentation
//files (the "Software"), to deal in the Software without
//restriction, including without limitation the rights to use,
//copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the
//Software is furnished to do so, subject to the following
//conditions:
//
//The above copyright notice and this permission notice shall be
//included in all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
//HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace ArduinoFirmataLibrary
{
    public delegate void NewAnalogValueIsAvailableHandler(AnalogPortMessage newValue);
    public delegate void NewDigitalValueIsAvailableHandler(DigitalPortMessage newValue);

    public class Arduino : IDisposable
    {
        #region Constants
        // This is a bit mask to get the pin value from a byte
        private const byte PINMASK = 0x0F;
        // This is a bit mask to get the command bits from a byte
        private const int COMMANDMASK = 0xF0;
        
        // This is the enable report command to be used with the toggle analog and toggle digital commands
        private const byte ENABLEREPORT = 0x01;
        // This is the disable report command to be used with the toggle analog and toggle digital commands
        private const byte DISABLEREPORT = 0x00;
        
        // These are the valid pwm pins for the arduino duemilanove
        private static readonly byte[] VALIDPWMPINS = new byte[] { 3, 5, 6, 9, 10, 11 };
        #endregion


        #region Private Fields
        // The serial port that will be used for all communication
        private SerialPort serialPort;

        // This array holds the pin mapping for analog reporting
        // This can only be changed in the standby state (for now)
        private readonly bool[] analogReportState = new bool[6];

        // This array holds the pin mapping for digital reporting
        // This can only be changed in the standby state (for now)
        private readonly bool[] digitalReportState = new bool[3];

        // The current state of the Arduino object (used in such a way 
        // that we don't do anything that wasn't predicted)
        private ArduinoLibraryStates currentState;
        #endregion

        /// <summary>
        /// The current state of the Arduino object
        /// </summary>
        public ArduinoLibraryStates CurrentState
        {
            get { return currentState; }
        }

        /// <summary>
        /// This is an event that fires whenever a new analog value has come through the
        /// serial port. It only fires in the continuous operation state
        /// </summary>
        public event NewAnalogValueIsAvailableHandler NewAnalogValueIsAvailable;

        /// <summary>
        /// This is an event that fires whenever a new digital value has come through the
        /// serial port. It only fires in the continuous operation state
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
        public Arduino(string portName, int baudRate, Parity parity, int databits, StopBits stopBits)
        {
            serialPort = new SerialPort(portName, baudRate, parity, databits, stopBits);
            
            // Set this threshold because all the messages we receive have 3 bytes
            serialPort.ReceivedBytesThreshold = 3;

            // Toggle the current state to standby
            currentState = ArduinoLibraryStates.StandBy;

            // Open the port
            serialPort.Open();
        }

        /// <summary>
        /// Initializes a new instance of the Arduino class giving information about the
        /// port name, all the other connection parameters get their default values.
        /// The arduino must be running the Firmata program. 
        /// </summary>
        /// <param name="portName">The name of the assigned COM port</param>
        public Arduino(string portName) : this(portName, 115200, Parity.None, 8, StopBits.One) { }

        #endregion

        /// <summary>
        /// Toggle the report state for the pin specified. This method works only in the standby state
        /// </summary>
        /// <param name="pin">The pin whose analog report state we want to change. Valid pin are from
        /// 0 to 5</param>
        /// <param name="newState">The new state</param>
        public void ToggleAnalogReportStateForPin(int pin, bool newState)
        {
            // If the pin is not an analog pin
            if (pin < 0 || pin > 5)
                throw new ArduinoException(ArduinoErrorCodes.INVALIDPIN);

            // If we are not in the standby state
            if (CurrentState != ArduinoLibraryStates.StandBy)
                throw new ArduinoException(ArduinoErrorCodes.CURRENTSTATEDOESNOTPERMITOPERATION);

            analogReportState[pin] = newState;
        }

        /// <summary>
        /// Toggle the report state for the pin specified. This method works only in the standby state
        /// </summary>
        /// <param name="port">The port whose digital report state we want to change. Valid ports are from
        /// 0 to 2</param>
        /// <param name="newState">The new state</param>
        public void ToggleDigitalReportStateForPort(byte port, bool newState)
        {
            // If the pin is not an analog pin
            if (port < 0 || port > 2)
                throw new ArduinoException(ArduinoErrorCodes.INVALIDVALUE);

            // If we are not in the standby state
            if (CurrentState != ArduinoLibraryStates.StandBy)
                throw new ArduinoException(ArduinoErrorCodes.CURRENTSTATEDOESNOTPERMITOPERATION);

            digitalReportState[port] = newState;
        }

        /// <summary>
        /// Start the process of receiving messages. This means that all the analog
        /// and digital ports that have been assigned to report will be toggled and
        /// a handler will be setup for all the incoming data. Subscribe to the NewAnalogValueIsAvailable
        /// event to get this data
        /// </summary>
        public void StartReceivingReports()
        {
            try
            {
                for (int i = 0; i < analogReportState.Length; i++)
                {
                    serialPort.Write(GetAnalogReportCommandForPin(analogReportState[i], i), 0, 2);
                }


                for (int i = 0; i < digitalReportState.Length; i++)
                {
                    serialPort.Write(GetDigitalReportCommandForPort(digitalReportState[i],i),0,2);
                }


                currentState = ArduinoLibraryStates.ContinuousOperation;
                serialPort.DataReceived += port_DataReceived;
            }
            catch(Exception)
            {
                throw new ArduinoException(ArduinoErrorCodes.UNKNOWNERROR);
            }
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[3];

            serialPort.Read(buffer, 0, 3);
            // We have an analog report 
            if ((buffer[0] & COMMANDMASK) == FirmataCommands.ANALOGMESSAGE )
            {
                int tempPin = (buffer[0] & PINMASK);
                int tempValue = GetValueFromBytes(buffer[2], buffer[1]);

                // If the value is out of the expected range then there is a problem
                if (tempValue <= 1023 && tempValue >= 0)
                {
                    // Check if we have someone who has subscribed to the newanalogvalueisavailable event
                    if (NewAnalogValueIsAvailable != null)
                    {
                        // Sent the newly read value for the current pin to the subscribers
                        NewAnalogValueIsAvailable(
                            new AnalogPortMessage
                                {
                                    AnalogValue = ((float)tempValue) / 1023 * 5, 
                                    Pin = tempPin
                                });
                    }
                }
            }
            else if ((buffer[0] & COMMANDMASK) == FirmataCommands.DIGITALMESSAGE)
            {
                int port = (buffer[0] & PINMASK);
                int tempValue = GetValueFromBytes(buffer[2], buffer[1]);
                bool[] pins = GetPinValuesFromPort((byte) tempValue);

                // Check if we have someone who has subscribed to the newdigitalvalueisavailable event
                if (NewDigitalValueIsAvailable != null)
                {
                    // Sent the newly read value for the current port to the subscribers
                    NewDigitalValueIsAvailable(
                        new DigitalPortMessage
                            {
                                Pins = pins,
                                Port = port
                        });
                }
            }
        }

        /// <summary>
        /// Toggle all reporting off
        /// </summary>
        public void StopReceivingReports()
        {
            try
            {
                serialPort.DataReceived -= port_DataReceived;

                // Close all reports
                for (int i = 0; i < analogReportState.Length; i++)
                {
                    serialPort.Write(GetAnalogReportCommandForPin(false, i), 0, 2);
                }

                for (int i = 0; i < digitalReportState.Length; i++)
                {
                    serialPort.Write(GetDigitalReportCommandForPort(false, i), 0, 2);
                }
            }
            catch (Exception)
            {
                throw new ArduinoException(ArduinoErrorCodes.UNKNOWNERROR);
            }
            finally
            {
                // Discard the in buffer so we don't have leftovers when we reopen the port
                serialPort.DiscardInBuffer();
                
                //Return to the standby state
                currentState = ArduinoLibraryStates.StandBy;
            }
        }

        #region OneShot Operations

        /// <summary>
        /// Read the voltage on the analog pin that is specified
        /// </summary>
        /// <param name="pin">The analog input pin. Valid values are 0-5</param>
        /// <returns>The analog reading in volts. Error number if there was an error</returns>
        public float ReadAnalog(byte pin)
        {
            // If the pin is not an analog pin
            if ((pin < 0 || pin > 5) )
                throw new ArduinoException(ArduinoErrorCodes.INVALIDPIN);

            // If we are not in the standby state
            if (CurrentState != ArduinoLibraryStates.StandBy)
                throw new ArduinoException(ArduinoErrorCodes.CURRENTSTATEDOESNOTPERMITOPERATION);

            // Toggle the current state
            currentState = ArduinoLibraryStates.OneShotOperation;

            try
            {
                // Write the command to enable the analog report for the pin we want
                serialPort.Write(GetAnalogReportCommandForPin(true,pin), 0, 2);
                Thread.Sleep(25);
                byte[] buffer = new byte[3];

                while (true)
                {
                    serialPort.Read(buffer, 0, 3);
                    // We have an analog report and we have the correct pin
                    if ((buffer[0] & COMMANDMASK) == FirmataCommands.ANALOGMESSAGE &&
                        (buffer[0] & PINMASK) == pin)
                    {
                        // Write the command to disable the analog report for the pin we want
                        serialPort.Write(GetAnalogReportCommandForPin(false, pin), 0, 2);
                        // Discard any other data left in the buffer
                        serialPort.DiscardInBuffer();

                        int tempValue = GetValueFromBytes(buffer[2], buffer[1]);

                        // If the value is out of the expected range then there is a problem
                        if (tempValue > 1023 || tempValue < 0)
                        {
                            throw new ArduinoException(ArduinoErrorCodes.INVALIDVALUE);
                        }

                        // Normalize the value to volts
                        return ((float)tempValue) / 1023 * 5;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new ArduinoException(ArduinoErrorCodes.UNKNOWNERROR);
            }
            finally
            {
                // Reset the current state
                currentState = ArduinoLibraryStates.StandBy;
            }
        }

        /// <summary>
        /// Write an analog value that should be set to the PWM outputs of arduino
        /// </summary>
        /// <param name="pin">The analog output pin. Valid pins are 3,5,6,9,10,11 for newer boards. 
        /// 9,10,11 for older boards</param>
        /// <param name="outputValue">The PWM duty cycle in percentage.</param>
        /// <returns>Error number if there was an error. 0 otherwise</returns>
        public void SetPWMOutput(byte pin, int outputValue)
        {
            // Check if we have a percentile value
            if (outputValue < 0 || outputValue > 100)
                throw new ArduinoException(ArduinoErrorCodes.INVALIDVALUE);

            // Check if we have a valid pin number
            if (!VALIDPWMPINS.Contains(pin))
                throw new ArduinoException(ArduinoErrorCodes.INVALIDPIN);

            // If we are not in the standby state
            if (CurrentState != ArduinoLibraryStates.StandBy)
                throw new ArduinoException(ArduinoErrorCodes.CURRENTSTATEDOESNOTPERMITOPERATION);


            // Toggle the current state
            currentState = ArduinoLibraryStates.OneShotOperation;

            // Convert this percentile value to something that arduino will understand
            byte pwmValue = (byte)(outputValue * 255 / 100);


            // No need to write the command that sets this pin to analog output.
            // it is already implemented at the standard firmata program

            try
            {
               serialPort.Write(GetAnalogOutputCommandForPin(pwmValue,pin), 0, 3);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new ArduinoException(ArduinoErrorCodes.UNKNOWNERROR);
            }
            finally
            {
                // Reset the current state to standby
                currentState = ArduinoLibraryStates.StandBy;
            }
        }

        /// <summary>
        /// Change the digital output for a port of arduino's ATMEGA chip.
        /// The pins that we want to change must have been put into output mode
        /// </summary>
        /// <param name="port">The port that corresponds to the port of the ATMEGA</param>
        /// <param name="pins">A byte whose individual bits represent the 
        /// state of the corresponding pin</param>
        /// <returns>Error number if there was an error. 0 otherwise</returns>
        public void SetDigitalOutput(byte port, byte pins)
        {
            // If we are not in the standby state
            if (CurrentState != ArduinoLibraryStates.StandBy)
                throw new ArduinoException(ArduinoErrorCodes.CURRENTSTATEDOESNOTPERMITOPERATION);


            // Toggle the current state
            currentState = ArduinoLibraryStates.OneShotOperation;

            // TODO : Maybe we need to write the command that sets this pin to digital output.

            try
            {
                serialPort.Write(GetDigitalOutputCommandForPort(port, pins), 0, 3);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new ArduinoException(ArduinoErrorCodes.UNKNOWNERROR);
            }
            finally
            {
                // Reset the current state to standby
                currentState = ArduinoLibraryStates.StandBy;
            }
        }
        
        public byte ReadDigital(byte port)
        {
            // If we are not in the standby state
            if (CurrentState != ArduinoLibraryStates.StandBy)
                throw new ArduinoException(ArduinoErrorCodes.CURRENTSTATEDOESNOTPERMITOPERATION);

            // Toggle the current state
            currentState = ArduinoLibraryStates.OneShotOperation;

            try
            {
                // Write the command to enable the analog report for the pin we want
                serialPort.Write(GetDigitalReportCommandForPort(true, port), 0, 2);
                Thread.Sleep(25);
                byte[] buffer = new byte[3];

                while (true)
                {
                    // BUG : find why there is no answer
                    serialPort.Read(buffer, 0, 3);

                    // We have an analog report and we have the correct pin
                    if ((buffer[0] & COMMANDMASK) == FirmataCommands.DIGITALMESSAGE && 
                        (buffer[0] & PINMASK) == port)
                    {
                        // Write the command to disable the analog report for the pin we want
                        serialPort.Write(GetDigitalReportCommandForPort(false, port), 0, 2);

                        return (byte)GetValueFromBytes(buffer[2], buffer[1]);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new ArduinoException(ArduinoErrorCodes.UNKNOWNERROR);
            }
            finally
            {
                // Reset the current state
                currentState = ArduinoLibraryStates.StandBy;
            }
        }
        
        public void TogglePinState(byte pin, byte mode)
        {
            // The protocol allows 127 pins
            if ( pin > 127 )
                throw new ArduinoException(ArduinoErrorCodes.INVALIDPIN);

            // If we are not in the standby state
            if (CurrentState != ArduinoLibraryStates.StandBy)
                throw new ArduinoException(ArduinoErrorCodes.CURRENTSTATEDOESNOTPERMITOPERATION);

            // Toggle the current state
            currentState = ArduinoLibraryStates.OneShotOperation;

            try
            {
                // Write the command to change the pin mode
                serialPort.Write(new byte[]{FirmataCommands.SETPINMODE, pin, mode}, 0, 2);
                Thread.Sleep(25);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new ArduinoException(ArduinoErrorCodes.UNKNOWNERROR);
            }
            finally
            {
                // Reset the current state
                currentState = ArduinoLibraryStates.StandBy;
            }
        }
        #endregion 

        #region Helper Methods

        /// <summary>
        /// Get the command for analog output
        /// Take the 4 MSB from the command type and the 4 LSB from the pin and create a command
        /// Also split the int value to a two-byte representation
        /// </summary>
        private static byte[] GetAnalogOutputCommandForPin(int value, int pin)
        {
            // Translate the value to the format wanted by the firmata library
            byte LSB, MSB;
            GetBytesFromValue(value, out MSB, out LSB);

            // We take the 4 MSB from the command type and the 4 LSB from the pin
            byte writeCommand = (byte)(FirmataCommands.ANALOGMESSAGE | pin);

            // Write the command to enable the analog output for the pin we want
            return new byte[] { writeCommand, LSB, MSB };
        }

        /// <summary>
        /// Get the command for analog report toggle
        /// Take the 4 MSB from the command type and the 4 LSB from the pin and create a command
        /// Also put the report enable or disable flag in the command
        /// </summary>
        private static byte[] GetAnalogReportCommandForPin(bool shouldEnable, int pin)
        {
            byte readCommand = (byte)(FirmataCommands.TOGGLEANALOGREPORT | pin);
            if (shouldEnable) 
                return new byte[] {readCommand, ENABLEREPORT};
            else 
                return new byte[] {readCommand, DISABLEREPORT};
        }

        /// <summary>
        /// Get the command for digital output
        /// Take the 4 MSB from the command type and the 4 LSB from the pin and create a command
        /// Also split the byte value to a two-byte representation
        /// </summary>
        private static byte[] GetDigitalOutputCommandForPort(byte port, byte portValue)
        {
            // Translate the value to the format wanted by the firmata library
            byte LSB, MSB;
            GetBytesFromValue(portValue, out MSB, out LSB);

            // We take the 4 MSB from the command type and the 4 LSB from the pin
            byte writeCommand = (byte)(FirmataCommands.DIGITALMESSAGE | port);

            // Write the command to enable the analog output for the pin we want
            return new byte[] { writeCommand, LSB, MSB };
        }

        /// <summary>
        /// Get the command for digital report toggle
        /// Take the 4 MSB from the command type and the 4 LSB from the pin and create a command
        /// Also put the report enable or disable flag in the command
        /// </summary>
        private static byte[] GetDigitalReportCommandForPort(bool shouldEnable, int port)
        {
            byte readCommand = (byte)(FirmataCommands.TOGGLEDIGITALREPORT | port);
            if (shouldEnable)
                return new byte[] { readCommand, ENABLEREPORT };
            else
                return new byte[] { readCommand, DISABLEREPORT };
        }
        
        /// <summary>
        /// Get the integer value that was sent using the 7-bit messages of the firmata protocol
        /// </summary>
        public static int GetValueFromBytes(byte MSB, byte LSB)
        {
            int tempValue = MSB & 0x7F;
            tempValue = tempValue << 7;
            tempValue = tempValue | (LSB & 0x7F);
            return tempValue;
        }

        /// <summary>
        /// Split an integer value to two 7-bit parts so it can be sent using the firmata protocol
        /// </summary>
        public static void GetBytesFromValue(int value, out byte MSB, out byte LSB)
        {
            LSB = (byte)(value & 0x7F);
            MSB = (byte)((value >> 7) & 0x7F);
        }

        /// <summary>
        /// Send a byte representing a port and get an array of boolean values indicating
        /// the state of each individual pin
        /// </summary>
        public static bool[] GetPinValuesFromPort(byte portConfiguration)
        {
            bool[] pins = new bool[8];

            for (int i = 0; i < pins.Length; i++)
            {
                pins[i] = ((portConfiguration >> i) & 0x01) == 1;
            }

            return pins;
        }

        /// <summary>
        /// Send an array of boolean values indicating the state of each individual 
        /// pin and get a byte representing a port 
        /// </summary>
        public static byte GetPortFromPinValues(bool[] pins)
        {
            byte port = 0;
            for (int i = 0; i < pins.Length; i++)
            {
                port |= (byte) ((pins[i] ? 1 : 0) << i);
            }

            return port;
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
            if (disposing) 
            {
                // Free other state (managed objects).
                if ( serialPort != null )
                {
                    serialPort.Close();
                    serialPort.Dispose();
                }
            }
            // Free your own state (unmanaged objects).
            // Set large fields to null.
            serialPort = null;
        }

        // Use C# destructor syntax for finalization code.
        ~Arduino()
        {
            Dispose (false);
        }
        #endregion
    }
}
