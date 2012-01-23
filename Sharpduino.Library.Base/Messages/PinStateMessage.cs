using System;
using Sharpduino.Library.Base.Constants;

namespace Sharpduino.Library.Base.Messages
{
    public class PinStateMessage
    {
        public int PinNo { get; set; }
        public int State { get; set; }
        public PinModes Mode { get; set; }

        public PinStateMessage()
        {
            State = 0;
            PinNo = 0;
        }
    }
}