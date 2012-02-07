using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;

namespace Sharpduino.Library.Base.Messages.Send
{
    public class QueryCapabilityMessage : StaticMessage
    {
        public QueryCapabilityMessage() : 
            base(MessageConstants.SYSEX_START,SysexCommands.CAPABILITY_QUERY,MessageConstants.SYSEX_END)
        {}
    }
}
