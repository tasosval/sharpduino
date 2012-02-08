using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpduino.Library.Base.Messages.Send
{
    public class SamplingIntervalMessage
    {
        /// <summary>
        /// The sampling interval in milliseconds
        /// </summary>
        public int Interval { get; set; }
    }
}
