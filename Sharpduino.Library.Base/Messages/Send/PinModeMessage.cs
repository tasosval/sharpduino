using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;

namespace Sharpduino.Library.Base.Messages.Send
{
    public class PinModeMessage
    {
        public byte Pin { get; set; }
        public PinModes Mode { get; set; }
    }
}
