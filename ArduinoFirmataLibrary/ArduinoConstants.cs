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
    public static class Firmata
    {
        /// <summary>
        /// The distinctive value that states that this message is a digital message. 
        /// It comes as a report or as a command
        /// </summary>
        public const byte DIGITAL_MESSAGE     = 0x90;   // 0x90-0x9f
        /// <summary>
        /// The command that toggles the continuous sending of the 
        /// analog reading of the specified pin
        /// </summary>
        public const byte REPORT_ANALOG_PIN   = 0xc0;   // 0xc0-0xcf
        /// <summary>
        /// The command that toggles the continuous sending of the 
        /// digital state of the specified port
        /// </summary>
        public const byte REPORT_DIGITAL_PORT = 0xd0;   // 0xd0-0xdf
        /// <summary>
        /// The distinctive value that states that this message is an analog message. 
        /// It comes as a report for analog in pins, or as a command for PWM
        /// </summary>
        public const byte ANALOG_MESSAGE      = 0xe0;   // 0xe0-0xef
        public const byte SYSEX_START         = 0xf0;
        /// <summary>
        /// A command to change the pin mode for the specified pin
        /// </summary>
        public const byte SET_PIN_MODE        = 0xf4;
        public const byte SYSEX_END           = 0xf7;
        public const byte PROTOCOL_VERSION    = 0xf9;
        public const byte SYSTEM_RESET        = 0xff;

        #region SYSEX messages
        public const byte SYSEX_ANALOG_MAPPING_QUERY    = 0x69;
        public const byte SYSEX_ANALOG_MAPPING_RESPONSE = 0x6a;
        public const byte SYSEX_CAPBILITY_QUERY         = 0x6b;
        public const byte SYSEX_CAPBILITY_RESPONSE      = 0x6c;
        public const byte SYSEX_PIN_STATE_QUERY         = 0x6d;
        public const byte SYSEX_PIN_STATE_RESPONSE      = 0x6e;
        public const byte SYSEX_SERVO_CONFIG            = 0x70;
        public const byte SYSEX_STRING_DATA             = 0x71;
        public const byte SYSEX_PULSE_DATA              = 0x74;
        public const byte SYSEX_SHIFT_DATA              = 0x75;
        public const byte SYSEX_I2C_REQUEST             = 0x76;
        public const byte SYSEX_I2C_REPLY               = 0x77;
        public const byte SYSEX_I2C_CONFIG              = 0x78;
        public const byte SYSEX_QUERY_FIRMWARE          = 0x79;
        public const byte SYSEX_SAMPLING_INTERVAL       = 0x7a;
        #endregion

        public const int MAX_ANALOG_PINS = 16;
        public const int MAX_DIGITAL_PORTS = 16;
        public const int MAX_DIGITAL_PINS = 128;
        public const int MAX_DATA_BYTES = 1024; // XXX
        public const int PIN_MODES_NUM = 7;
    }

    
}
