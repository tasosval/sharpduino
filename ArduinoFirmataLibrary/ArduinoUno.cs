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
        private const int TOTALPINS = 24;

        public ArduinoUno(string portName, int baudRate, Parity parity, int databits, StopBits stopBits) : base(portName, baudRate, parity, databits, stopBits)
        {
            this.TotalPins = TOTALPINS;

            for (int i = 0; i < TotalPins; i++)
                Pins.Add(new Pin());

            // Reinitialize the board to default values );
            for (int i = 0; i < 6; i++)
            {
                this.SetPinMode(i, PinModes.Analog);    
            }

            this.SetPinMode(0, PinModes.Input);
            this.SetPinMode(1, PinModes.Input);
            for (int i = 2; i < 14; i++)
            {
                this.SetPinMode(i,PinModes.Output);
            }
        }

        public ArduinoUno(string portName) : this(portName,57600,Parity.None, 8, StopBits.One)
        {
        }

        public int AnalogRead(ArduinoUnoAnalogPins pin)
        {
            if (this.Pins[ArduinoUnoConstantsHelper.AnalogPinToPin((int) pin)].CurrentMode != PinModes.Analog )
                throw new ArduinoException(ArduinoErrorCodes.INVALIDPINSTATE);
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

        public void DigitalWrite(ArduinoUnoDigitalPins pin,bool value)
        {
            DigitalWrite((int)pin,value);
        }

        public override void SetPinMode(int pin, PinModes mode)
        {
            if (mode != PinModes.Analog)
            {
                base.SetPinMode(pin, mode);
            }
            else
            {
                base.SetPinMode(ArduinoUnoConstantsHelper.AnalogPinToPin(pin),mode);
            }
        }
    }
}
