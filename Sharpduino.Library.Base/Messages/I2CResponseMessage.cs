using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpduino.Library.Base.Messages
{
    public class I2CResponseMessage
    {
        public int SlaveAddress { get; set; }
        public int Register { get; set; }
        public List<int> Data { get; private set; }

        public I2CResponseMessage()
        {
            Data = new List<int>();
        }
    }
}
