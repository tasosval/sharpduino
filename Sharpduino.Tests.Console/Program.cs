using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Sharpduino.Constants;
using Sharpduino.EventArguments;
using Sharpduino.Messages.Send;
using Sharpduino.Messages.TwoWay;
using Sharpduino.SerialProviders;

namespace Sharpduino.Tests.Consoles
{
    class Program
    {
        private static bool isInitialized = false;
        static void Main(string[] args)
        {
            ComPortProvider port = new ComPortProvider("COM6");
            using (var easyFirmata = new EasyFirmata(port))
            {
                //easyFirmata.Initialized += easyFirmata_Initialized;
                //easyFirmata.NewAnalogValue += easyFirmata_NewAnalogValue;

                while (!easyFirmata.IsInitialized) ;

                for (int i = 0; i < easyFirmata.Pins.Count; i++)
                {
                    var pin = easyFirmata.Pins[i];
                    Console.WriteLine("Pin:{0} Current Mode:{1}", i,pin.CurrentMode);
                    foreach (var s in pin.Capabilities)
                    {
                        Console.WriteLine("\t{0} : {1} Resolution",s.Key,s.Value);
                    }
                }
                Console.ReadKey();

                #region Servo Test

//                easyFirmata.SendMessage(new ServoConfigMessage(){Pin = 9});
//                int val = 90;
//                ConsoleKey key;
//                do
//                {
//                    key = Console.ReadKey().Key;
//                    if ( key == ConsoleKey.UpArrow )
//                        easyFirmata.SendMessage(new AnalogMessage(){Pin = 9,Value = ++val});
//                    else if ( key == ConsoleKey.DownArrow )
//                        easyFirmata.SendMessage(new AnalogMessage() { Pin = 9, Value = --val });
//
//                    Thread.Sleep(100);
//
//                } while (key != ConsoleKey.Enter);

                #endregion
            }

//            using (var arduino = new ArduinoUno("COM4"))
//            {
//                while (!arduino.IsInitialized) ;
//                arduino.SetPinMode(ArduinoUnoPins.A0, PinModes.Output);
//                arduino.SetDO(ArduinoUnoPins.A0, true);
//
//                Console.ReadKey();
//            }
//
//            using (var ardMega = new ArduinoMega("COM6"))
//            {
//                while (!ardMega.IsInitialized) ;
//                while (!ardMega.IsInitialized) ;
//                int a = 5;
//            }
        }

        static void easyFirmata_NewAnalogValue(object sender, NewAnalogValueEventArgs e)
        {
            Console.WriteLine("New Value for pin {0} : {1}",e.AnalogPin,e.NewValue);
        }

        static void easyFirmata_Initialized(object sender, EventArgs e)
        {
            Console.WriteLine("Firmata has initialized");
            isInitialized = true;
        }
    }
}
