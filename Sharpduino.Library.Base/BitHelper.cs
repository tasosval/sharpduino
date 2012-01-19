using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpduino.Library.Base
{
    public static class BitHelper
    {
        /// <summary>
        /// Get the integer value that was sent using the 7-bit messages of the firmata protocol
        /// </summary>
        public static int Sevens2Fourteen(byte LSB, byte MSB)
        {
            return (MSB & 0x7f) << 7 | (LSB & 0x7f);
        }

        /// <summary>
        /// Split an integer value to two 7-bit parts so it can be sent using the firmata protocol
        /// </summary>
        public static void Fourteen2Sevens(int value, out byte LSB, out byte MSB)
        {
            LSB = (byte)(value & 0x7f);
            MSB = (byte)((value >> 7) & 0x7f);
        }

        /// <summary>
        /// Send a byte representing a port and get an array of boolean values indicating
        /// the state of each individual pin
        /// </summary>
        public static int[] PortVal2PinVals(byte val)
        {
            int[] pins = new int[8];

            for (int i = 0; i < pins.Length; i++)
            {
                pins[i] = (val >> i) & 0x01;
            }

            return pins;
        }

        /// <summary>
        /// Send an array of boolean values indicating the state of each individual 
        /// pin and get a byte representing a port 
        /// </summary>
        public static byte PinVals2PortVal(int[] pins)
        {
            byte port = 0;
            for (int i = 0; i < pins.Length; i++)
            {
                port |= (byte)(pins[i] << i);
            }

            return port;
        }
    }

}
