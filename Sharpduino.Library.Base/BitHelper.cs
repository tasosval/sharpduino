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
        public static int BytesToInt(byte LSB, byte MSB)
        {
            return (MSB & 0x7f) << 7 | (LSB & 0x7f);
        }

        /// <summary>
        /// Split an integer value to two 7-bit parts so it can be sent using the firmata protocol
        /// </summary>
        public static void IntToBytes(int value, out byte LSB, out byte MSB)
        {
            LSB = (byte)(value & 0x7f);
            MSB = (byte)((value >> 7) & 0x7f);
        }

        public static byte[] IntArrayToBytesArray(int[] values)
        {
            var bytes = new List<byte>();
            foreach (var value in values)
            {
                byte lsb, msb;
                IntToBytes(value,out lsb, out msb);
                bytes.Add(lsb);
                bytes.Add(msb);
            }

            return bytes.ToArray();
        }

        /// <summary>
        /// Send a byte representing a port and get an array of boolean values indicating
        /// the state of each individual pin
        /// </summary>
        public static bool[] PortVal2PinVals(byte val)
        {
            var pins = new bool[8];

            for (int i = 0; i < pins.Length; i++)
            {
                pins[i] = ( (val >> i) & 0x01 ) == 1;
            }

            return pins;
        }

        /// <summary>
        /// Send an array of boolean values indicating the state of each individual 
        /// pin and get a byte representing a port 
        /// </summary>
        public static byte PinVals2PortVal(bool[] pins)
        {
            byte port = 0;
            for (int i = 0; i < pins.Length; i++)
            {
                port |= (byte)( (pins[i] ? 1 : 0 ) << i);
            }

            return port;
        }
    }

}
