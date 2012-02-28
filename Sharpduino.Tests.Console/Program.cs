using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Sharpduino.Library.Base.SerialProviders;

namespace Sharpduino.Tests.Consoles
{
    class Program
    {
        private static bool isInitialized = false;
        static void Main(string[] args)
        {
            ComPortProvider port = new ComPortProvider("COM4");
            using (var easyFirmata = new EasyFirmata(port))
            {
                easyFirmata.Initialized += easyFirmata_Initialized;
                //easyFirmata.NewAnalogValue += easyFirmata_NewAnalogValue;

                while (!isInitialized)
                {
                    Thread.Sleep(10);
                }

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
            }
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
