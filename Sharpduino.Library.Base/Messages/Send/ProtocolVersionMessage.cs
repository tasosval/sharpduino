using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;

namespace Sharpduino.Library.Base.Messages.Send
{
    public class ProtocolVersionRequestMessage : StaticMessage
    {
        public ProtocolVersionRequestMessage() : 
            base(new byte[]{MessageConstants.PROTOCOL_VERSION})
        {}
    }
}
