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

namespace ArduinoFirmataLibrary
{
    internal static class BitHelper
    {
        /// <summary>
        /// Get the integer value that was sent using the 7-bit messages of the firmata protocol
        /// </summary>
        internal static int Sevens2Fourteen(byte LSB, byte MSB)
        {
            return (MSB & 0x7f) << 7 | (LSB & 0x7f);
        }

        /// <summary>
        /// Split an integer value to two 7-bit parts so it can be sent using the firmata protocol
        /// </summary>
        internal static void Fourteen2Sevens(int value, out byte LSB, out byte MSB)
        {
            LSB = (byte)(value & 0x7f);
            MSB = (byte)((value >> 7) & 0x7f);
        }

        /// <summary>
        /// Send a byte representing a port and get an array of boolean values indicating
        /// the state of each individual pin
        /// </summary>
        internal static int[] PortVal2PinVals(byte val)
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
        internal static byte PinVals2PortVal(int[] pins)
        {
            byte port = 0;
            for (int i = 0; i < pins.Length; i++)
            {
                port |= (byte) (pins[i] << i);
            }

            return port;
        }
    }
}
