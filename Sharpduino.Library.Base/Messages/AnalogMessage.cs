using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpduino.Library.Base.Messages
{
    public class AnalogMessage
    {
        public int Pin { get; set; }
        public int Value { get; set; }
    }
}
