using System;
using System.Collections.Generic;

namespace Sharpduino.SerialProviders
{
    public class DataReceivedEventArgs : EventArgs
    {
        public IEnumerable<byte> BytesReceived { get; set; }

        public DataReceivedEventArgs(IEnumerable<byte> bytesReceived)
        {
            BytesReceived = bytesReceived;
        }
    }
}