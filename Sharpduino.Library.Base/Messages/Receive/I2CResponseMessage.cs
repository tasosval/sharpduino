using System.Collections.Generic;

namespace Sharpduino.Library.Base.Messages.Receive
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
