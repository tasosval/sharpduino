using System.Collections.Generic;
using Sharpduino.Library.Base.Constants;

namespace Sharpduino.Library.Base.Messages.Receive
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
