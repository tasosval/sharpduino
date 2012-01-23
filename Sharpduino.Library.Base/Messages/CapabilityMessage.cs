using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;

namespace Sharpduino.Library.Base.Messages
{
    public class CapabilityMessage
    {
        public List<PinModes> SupportedModes { get; private set; }
        public byte PinNo { get; set; }
        public byte Resolution { get; set; }

        public CapabilityMessage()
        {
            SupportedModes = new List<PinModes>();
        }
    }
}
