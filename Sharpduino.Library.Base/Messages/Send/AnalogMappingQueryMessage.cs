using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;

namespace Sharpduino.Library.Base.Messages.Send
{
    public class AnalogMappingQueryMessage : StaticMessage
    {
        public AnalogMappingQueryMessage() :
            base(MessageConstants.SYSEX_START, SysexCommands.ANALOG_MAPPING_QUERY, MessageConstants.SYSEX_END)
        {}
    }
}
