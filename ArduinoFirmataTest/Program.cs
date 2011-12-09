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

#define SHOW_MAP
//#define TEST_DIGITAL_ANALOG_READ
//#define TEST_ANALOG_CALLBACK
//#define TEST_DIGITAL_CALLBACK
//#define TEST_DIGITAL_WRITE
//#define TEST_ANALOG_WRITE
//#define TEST_SERVO

using System;
using ArduinoFirmataLibrary;

namespace ArduinoFirmataTest
{
    class Program
    {
        private static ArduinoUno arduino;
        private static DateTime previousTime = DateTime.Now;

        static void Main(string[] args)
        {
            
            arduino = new ArduinoUno("COM3");
            arduino.Init();

            Console.WriteLine("Protocol version: " + arduino.ProtocolVersion);
            Console.WriteLine("Firmware version: " + arduino.FirmwareVersion + " " + arduino.FirmwareName);

#if SHOW_MAP
            #region show corresponding analog pin, current mode, current output, and supported modes

            Console.Write("Pin Analog Pin\tMode\tOutput\tSupported Modes");

            for (int i = 0; i < 24; i++)
            {
                Console.Write(
                    "\n{0}\t{1}\t{2}\t{3}\t", 
                    i,
                    arduino.Pins[i].AnalogPin,
                    arduino.Pins[i].CurrentMode,
                    arduino.Pins[i].Output
                );
                for (int j = 0; j < arduino.Pins[i].SupportedModes.Count; j++)
                {
                    Console.Write("{0}:{1}", arduino.Pins[i].SupportedModes[j], arduino.Pins[i].SupportedResolution[arduino.Pins[i].SupportedModes[j]]);
                    Console.Write(j != arduino.Pins[i].SupportedModes.Count - 1 ? ", " : "");
                }
            }
            Console.WriteLine();
            #endregion
#endif

#if TEST_DIGITAL_ANALOG_READ
            #region Test DigitalRead() and analogRead()
            Console.WriteLine("Test Arduino like functions (analogRead() and digitalRead()). \nPress a key to start");
            Console.WriteLine("Press a key once again to stop.");
            Console.ReadKey(true);

            // Uncomment next two lines if you need to make the pins inputs.
             for (int i = 0; i < 128; i++)
                arduino.SetPinMode(i, PinModes.Input );
            while (true)
            {
                var digitalPins = Enum.GetValues(typeof (ArduinoUnoDigitalPins)).Cast<ArduinoUnoDigitalPins>();
                var analogPins = Enum.GetValues(typeof (ArduinoUnoAnalogPins)).Cast<ArduinoUnoAnalogPins>();
                foreach (var digitalPin in digitalPins)
                {
                    Console.Write(arduino.DigitalRead(digitalPin) + " ");
                }
                Console.WriteLine();
                foreach (var analogPin in analogPins)
                {
                    Console.Write(arduino.AnalogRead(analogPin) + " ");
                }
                Console.WriteLine();
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    break;
                }
                System.Threading.Thread.Sleep(50);
            }
            #endregion
#endif

#if TEST_ANALOG_CALLBACK
            #region Test Analog Report
            Console.WriteLine("Analog callback test. Only analog 0-2 are recieved. Press a key to start");
                        Console.WriteLine("Press a key once again to stop.");
                        Console.ReadKey(true);
                        arduino.StopReceivingReports();
                        for (int i = 0; i < Firmata.MAX_ANALOG_PINS; i++)
                            arduino.SetAnalogReportStateForPin(i,false);;
                        arduino.SetAnalogReportStateForPin(0,true);
                        arduino.SetAnalogReportStateForPin(1, true);
                        arduino.SetAnalogReportStateForPin(2, true);
                        arduino.UpdateReportsToReceive();
                        arduino.NewAnalogValueIsAvailable += AnalogCallback;
                                    
                        Console.ReadKey(true);
                        arduino.StopReceivingReports();
                        arduino.NewAnalogValueIsAvailable -= AnalogCallback;
                        for (int i = 0; i < Firmata.MAX_ANALOG_PINS; i++)
                            arduino.SetAnalogReportStateForPin(i,false);
            #endregion
#endif

#if TEST_DIGITAL_CALLBACK
            #region Test Digital Report
            Console.WriteLine("Digital continuous read test. Press a key and see the values.");
            Console.WriteLine("Press a key once again to stop.");
            Console.WriteLine("Note: Values are shown only if a pin value is changed.");
            Console.ReadKey(true);
            // Uncomment next two lines if you need to make the pins inputs.
            // for (int i = 0; i < 128; i++)
            //    arduino.SetPinMode(i, PinModes.INPUT );
            for (int i = 0; i < Firmata.MAX_DIGITAL_PORTS; i++)
                arduino.SetDigitalReportStateForPort((byte)i,true);
            
            arduino.NewDigitalValueIsAvailable += DigitalCallback;
            arduino.UpdateReportsToReceive();
            Console.ReadKey(true);
            arduino.StopReceivingReports();
            arduino.NewDigitalValueIsAvailable -= DigitalCallback;
            #endregion
#endif

#if TEST_DIGITAL_WRITE
            #region Test WriteDigital
            Console.WriteLine("Digital write test");
            Console.WriteLine("Watch LED on Arduino to blink for 10 times. \nPress a key to continue");
            Console.ReadKey(true);
            arduino.SetPinMode(13, PinModes.Output);

            for (int i = 0; i < 10; i++)
            {
                arduino.DigitalWrite(ArduinoUnoDigitalPins.D13, 1);
                System.Threading.Thread.Sleep(200);
                arduino.DigitalWrite(ArduinoUnoDigitalPins.D13, 0);
                System.Threading.Thread.Sleep(200);
            }
            #endregion
#endif

#if TEST_ANALOG_WRITE
            #region Test AnalogWrite()
            Console.WriteLine("Test analogWrite(). Connect an LED to pin 9 to see the result");
            Console.WriteLine("Press any key to start");
            Console.ReadKey(true);
            arduino.AnalogWrite(ArduinoUnoPWMPins.D9_PWM, 10);
            System.Threading.Thread.Sleep(500);
            arduino.AnalogWrite(ArduinoUnoPWMPins.D9_PWM, 50);
            System.Threading.Thread.Sleep(500);
            arduino.AnalogWrite(ArduinoUnoPWMPins.D9_PWM, 100);
            System.Threading.Thread.Sleep(500);
            arduino.AnalogWrite(ArduinoUnoPWMPins.D9_PWM, 500);
            System.Threading.Thread.Sleep(500);
            arduino.AnalogWrite(ArduinoUnoPWMPins.D9_PWM, 1023);
            Console.WriteLine("Test finished. Press a key to continue");
            Console.ReadKey(true);
            arduino.AnalogWrite(ArduinoUnoPWMPins.D9_PWM, 0);
            #endregion
#endif

#if TEST_SERVO
            #region Test ServoAttach() and ServoWrite()
            Console.WriteLine("Test ServoAttach() and ServoWrite(). Connect a servo to pin 8 to see the result");
            Console.WriteLine("Press any key to start");
            arduino.ServoAttach(8);
            arduino.ServoWrite(8, 90);
            Console.ReadKey(true);
            System.Threading.Thread.Sleep(500);
            arduino.ServoWrite(8, 70);
            System.Threading.Thread.Sleep(500);
            arduino.ServoWrite(8, 60);
            System.Threading.Thread.Sleep(500);
            arduino.ServoWrite(8, 70);
            System.Threading.Thread.Sleep(500);
            arduino.ServoWrite(8, 90);
            System.Threading.Thread.Sleep(500);
            arduino.ServoWrite(8, 110);
            System.Threading.Thread.Sleep(500);
            arduino.ServoWrite(8, 120);
            Console.WriteLine("Test finished.");
            #endregion
#endif
                        
            Console.WriteLine("Press a key to close the console.");
            Console.ReadKey(true);
        }

        static void DigitalCallback(DigitalPortMessage val)
        {
            DateTime thisTime = DateTime.Now;
            int milliseconds = (thisTime - previousTime).Milliseconds;
            Console.WriteLine("Pins: {0} {1} {2} {3} {4} {5} {6} {7}\tPort: {8}\tTimespan: {9}",
                val.Pins[0],val.Pins[1],val.Pins[2],
                val.Pins[3],val.Pins[4],val.Pins[5],
                val.Pins[6],val.Pins[7],
                val.Port, milliseconds);
            previousTime = thisTime;
        }

        static void AnalogCallback(AnalogPinMessage val)
        {
            DateTime thisTime = DateTime.Now;
            int milliseconds = (thisTime - previousTime).Milliseconds;
//            Console.WriteLine("Pin: {0}\tValue: {1:0.000}\tTimespan: {2}",
            Console.WriteLine("Pin: {0}\tValue: {1}\tTimespan: {2}",
                  val.Pin, val.Value, milliseconds);
            previousTime = thisTime;
        }

    }
}
