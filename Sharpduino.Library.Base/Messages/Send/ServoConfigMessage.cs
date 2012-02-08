using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpduino.Library.Base.Messages.Send
{
    public class ServoConfigMessage
    {
        public const int DefaultMinPulse = 544;
        public const int DefaultMaxPulse = 2400;

        /// <summary>
        /// The pin where we want to attach the servo
        /// </summary>
        public byte Pin { get; set; }

        /// <summary>
        /// The minimum pulse for the servo in uSeconds
        /// </summary>
        public int MinPulse { get; set; }
        
        /// <summary>
        /// The maximum pulse for the servo in uSeconds
        /// </summary>
        public int MaxPulse { get; set; }

        /// <summary>
        /// The angle in degrees
        /// </summary>
        public int Angle { get; set; }

        /// <summary>
        /// Instansiates a ServoConfigMessage with the default values for Min and Max Pulse
        /// </summary>
        public ServoConfigMessage()
        {
            MinPulse = DefaultMinPulse;
            MaxPulse = DefaultMaxPulse;
            Angle = Pin = 0;
        }
    }
}
