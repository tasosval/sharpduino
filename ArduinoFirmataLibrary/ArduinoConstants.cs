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

namespace ArduinoFirmataLibrary
{
    public static class FirmataCommands
    {
        /// <summary>
        /// The command that toggles the continuous sending of the 
        /// analog reading of the specified pin
        /// </summary>
        public const byte TOGGLEANALOGREPORT = 0xC0;
        /// <summary>
        /// The distinctive value that states that this message is an analog message. 
        /// It comes as a report for analog in pins, or as a command for PWM
        /// </summary>
        public const byte ANALOGMESSAGE = 0xE0;
        /// <summary>
        /// The command that toggles the continuous sending of the 
        /// digital state of the specified port
        /// </summary>
        public const byte TOGGLEDIGITALREPORT = 0xD0;
        /// <summary>
        /// The distinctive value that states that this message is a digital message. 
        /// It comes as a report or as a command
        /// </summary>
        public const byte DIGITALMESSAGE = 0x90;
        /// <summary>
        /// A command to change the pin mode for the specified pin
        /// </summary>
        public const byte SETPINMODE = 0xF4;
    }

    public static class PinModes
    {
        public const byte INPUT = 0x00;
        public const byte OUTPUT = 0x01;
        /// <summary>
        /// This is not implemented in the standard firmata program
        /// </summary>
        public const byte ANALOG = 0x02;
        public const byte PWM = 0x03;
        /// <summary>
        /// This is not implemented in the standard firmata program
        /// </summary>
        public const byte SERVO = 0x04;
    }

    public enum ArduinoLibraryStates
    {
        StandBy,
        OneShotOperation,
        ContinuousOperation
    }

    public static class ATMegaPorts
    {
        /// <summary>
        /// This port represents digital pins 8..13. 14 and 15 are for the crystal
        /// </summary>
        public const byte PORTB = 1;

        /// <summary>
        /// This port represents analog input pins 0..5
        /// </summary>
        public const byte PORTC = 2;

        /// <summary>
        /// This port represents digital pins 0..7. Pins 0 and 1 are reserved for communication
        /// </summary>
        public const byte PORTD = 0;
    }

    public enum ArduinoErrorCodes
    {
        CURRENTSTATEDOESNOTPERMITOPERATION = -1,
        UNKNOWNERROR = -2,
        INVALIDPIN = -3,
        INVALIDVALUE = -4
    }
}
