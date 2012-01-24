using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;

namespace Sharpduino.Library.Base.Messages
{
    public class CapabilityMessage
    {
        /// <summary>
        /// This is a dictionary of the supported modes as keys
        /// which point to the appropriate resolutions
        /// </summary>
        public Dictionary<PinModes,int> Modes { get; private set; }
        public byte PinNo { get; set; }

        public CapabilityMessage()
        {
            Modes = new Dictionary<PinModes, int>();
        }
    }
}
