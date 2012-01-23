﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpduino.Library.Base.Constants
{
    public static class MessageConstants
    {
        /// <summary>
        /// Digital message. 
        /// It comes as a report or as a command
        /// </summary>
        public const byte DIGITAL_MESSAGE = 0x90;   // 0x90-0x9f

        /// <summary>
        /// The command that toggles the continuous sending of the 
        /// analog reading of the specified pin
        /// </summary>
        public const byte REPORT_ANALOG_PIN = 0xc0;   // 0xc0-0xcf

        /// <summary>
        /// The command that toggles the continuous sending of the 
        /// digital state of the specified port
        /// </summary>
        public const byte REPORT_DIGITAL_PORT = 0xd0;   // 0xd0-0xdf

        /// <summary>
        /// Analog message. 
        /// It comes as a report for analog in pins, or as a command for PWM
        /// </summary>
        public const byte ANALOG_MESSAGE = 0xe0;   // 0xe0-0xef

        /// <summary>
        /// A command to change the pin mode for the specified pin
        /// </summary>
        public const byte SET_PIN_MODE = 0xf4;        

        /// <summary>
        /// Protocol version message
        /// </summary>
        public const byte PROTOCOL_VERSION = 0xf9;

        /// <summary>
        /// This is for a message of system reset
        /// </summary>
        public const byte SYSTEM_RESET = 0xff;

        /// <summary>
        /// The end of a sysex message
        /// </summary>
        public const byte SYSEX_END = 0xf7;

        /// <summary>
        /// The start of a sysex message
        /// </summary>
        public const byte SYSEX_START = 0xf0;

        /// <summary>
        /// The maximum number of analog pins
        /// </summary>
        public const int MAX_ANALOG_PINS = 16;

        /// <summary>
        /// The maximum number of digital ports (8 pins => 1 port)
        /// </summary>
        public const int MAX_DIGITAL_PORTS = 16;

        /// <summary>
        /// The maximum number of digital pins
        /// </summary>
        public const int MAX_DIGITAL_PINS = 128;
        
        /// <summary>
        /// Maximum bytes in a sysex message
        /// </summary>
        public const int MAX_DATA_BYTES = 1024;

        /// <summary>
        /// This is a mask for analog and digital messages
        /// to differentiate the message type
        /// </summary>
        public const byte MESSAGETYPEMASK = 0xF0;

        /// <summary>
        /// This is a mask for analog and digital messages
        /// to differentiate the pin/port
        /// </summary>
        public const byte MESSAGEPINMASK = 0x0F;
    }
}