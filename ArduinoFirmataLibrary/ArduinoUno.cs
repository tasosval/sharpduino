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

using System.IO.Ports;

namespace ArduinoFirmataLibrary
{
    public sealed class ArduinoUno : Arduino
    {
        public ArduinoUno(string portName, int baudRate, Parity parity, int databits, StopBits stopBits) : base(portName, baudRate, parity, databits, stopBits)
        {
        }

        public ArduinoUno(string portName) : base(portName)
        {
        }

        public int AnalogRead(ArduinoUnoAnalogPins pin)
        {
            return this.AnalogRead((int) pin);
        }
        
        public void AnalogWrite(ArduinoUnoPWMPins pin, int value)
        {
            this.AnalogWrite((int)pin,value);
        }

        public int DigitalRead(ArduinoUnoDigitalPins pin)
        {
            return DigitalRead((int) pin);
        }

        public void DigitalWrite(ArduinoUnoDigitalPins pin,int value)
        {
            DigitalWrite((int)pin,value);
        }
    }
}
