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
using ArduinoFirmataLibrary;

namespace ArduinoFirmataTest
{
    class Program
    {
        private static Arduino ard;
        private static DateTime previousTime = DateTime.Now;

        static void Main(string[] args)
        {
            ard = new Arduino("COM4");


            #region Test ReadAnalog
            //            Console.WriteLine("Test oneshot read analog");
            //            Console.WriteLine(ard.ReadAnalog(0));
            //            Console.WriteLine("Analog test finished. Press a key to continue");
            //            Console.ReadKey(true);
            #endregion

            #region Test WritePWM
            //            Console.WriteLine("Test oneshot PWM write. Connect an LED to pin 9 to see the result");
            //            ard.SetPWMOutput(9,20);
            //            Thread.Sleep(500);
            //            ard.SetPWMOutput(9, 40);
            //            Thread.Sleep(500);
            //            ard.SetPWMOutput(9, 60);
            //            Thread.Sleep(500);
            //            ard.SetPWMOutput(9, 80);
            //            Thread.Sleep(500);
            //            ard.SetPWMOutput(9, 100);
            //            Console.WriteLine("PWM test finished. Press a key to continue");
            //            Console.ReadKey(true);
            //            ard.SetPWMOutput(9, 0);
            #endregion

            #region Test WriteDigital
            //            Console.WriteLine("Digital write test");
            //            bool[] pins = new bool[]{true,true,true,true,true,true,false,false};
            //            ard.SetDigitalOutput(ATPort.PORTB,Arduino.GetPortFromPinValues(pins));
            //            Console.WriteLine("Digital pins should be on. Press a key to continue");
            //            Console.ReadKey(true);
            //            pins = new bool[] { false, false, false, false, false, false, false, false };
            //            ard.SetDigitalOutput(ATPort.PORTB, Arduino.GetPortFromPinValues(pins));
            //            Console.WriteLine("Digital pins should be off. Digital Write test finished. Press a key to continue");
            //            Console.ReadKey(true);
            #endregion

            #region Test ReadDigital
//            Console.WriteLine(ard.ReadDigital(ATMegaPorts.PORTD));
//            Console.ReadKey();
            #endregion

            #region Test Analog Report
            //            Console.WriteLine("Analog continuous read test. Press a key and see the values of pins 0-2");
            //            Console.WriteLine("To stop press a key again");
            //            Console.ReadKey(true);
            //            ard.ToggleAnalogReportStateForPin(0, true);
            //            ard.ToggleAnalogReportStateForPin(1, true);
            //            ard.ToggleAnalogReportStateForPin(2, true);
            //            ard.NewAnalogValueIsAvailable += ard_NewAnalogValueIsAvailable;
            //            ard.StartReceivingReports();
            //            Console.ReadKey(true);
            //            ard.StopReceivingReports();
            //            ard.NewAnalogValueIsAvailable -= ard_NewAnalogValueIsAvailable;
            //            ard.ToggleAnalogReportStateForPin(0,false);
            //            ard.ToggleAnalogReportStateForPin(1,false);
            //            ard.ToggleAnalogReportStateForPin(2,false);
            //            Console.ReadKey(true);
            #endregion

            #region Test Digital Report
            Console.WriteLine("Digital continuous read test. Press a key and see the values of ports 0-2");
            Console.WriteLine("To stop press a key again");
            Console.ReadKey(true);
            ard.TogglePinState(12,PinModes.INPUT );
            ard.ToggleDigitalReportStateForPort(0, true);
            ard.ToggleDigitalReportStateForPort(1, true);
            ard.ToggleDigitalReportStateForPort(2, true);
            ard.NewDigitalValueIsAvailable += ard_NewDigitalValueIsAvailable;
            ard.StartReceivingReports();
            Console.ReadKey(true);
            ard.StopReceivingReports();
            ard.NewDigitalValueIsAvailable -= ard_NewDigitalValueIsAvailable;
            ard.ToggleDigitalReportStateForPort(0, false);
            ard.ToggleDigitalReportStateForPort(1, false);
            ard.ToggleDigitalReportStateForPort(2, false);
            Console.ReadKey(true);
            #endregion

            #region Test helper methods
            //            bool[] pins = Arduino.GetPinValuesFromPort(0xAF);
            //            Arduino.GetPortFromPinValues(pins);
            #endregion
        }

        static void ard_NewDigitalValueIsAvailable(DigitalPortMessage newValue)
        {
            DateTime thisTime = DateTime.Now;
            int milliseconds = (thisTime - previousTime).Milliseconds;
            Console.WriteLine("Pins: {0} {1} {2} {3} {4} {5} {6} {7}\tPort: {8}\tTimespan: {9}",
                newValue.Pins[0],newValue.Pins[1],newValue.Pins[2],
                newValue.Pins[3],newValue.Pins[4],newValue.Pins[5],
                newValue.Pins[6],newValue.Pins[7],
                newValue.Port, milliseconds);
            previousTime = thisTime;
        }

        static void ard_NewAnalogValueIsAvailable(AnalogPortMessage newValue)
        {
            DateTime thisTime = DateTime.Now;
            int milliseconds = (thisTime - previousTime).Milliseconds;
            Console.WriteLine("Pin: {0}\tValue: {1:0.000}\tTimespan: {2}",
                newValue.Pin, newValue.AnalogValue, milliseconds);
            previousTime = thisTime;
        }

    }
}
