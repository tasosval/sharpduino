using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;

namespace Sharpduino.Library.Base.Messages.Send
{
    public class QueryFirmwareMessage : StaticMessage
    {
        public QueryFirmwareMessage() : 
            base(MessageConstants.SYSEX_START,SysexCommands.QUERY_FIRMWARE,MessageConstants.SYSEX_END)
        {}
    }
}
