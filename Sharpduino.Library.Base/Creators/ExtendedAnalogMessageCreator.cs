using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.Send;

namespace Sharpduino.Library.Base.Creators
{
    public class ExtendedAnalogMessageCreator : BaseMessageCreator<ExtendedAnalogMessage>
    {
        public override byte[] CreateMessage(ExtendedAnalogMessage message)
        {
            if (message == null)
                throw new MessageCreatorException("This is not an Extended Analog message");

            if (message.Pin > MessageConstants.MAX_PINS)
                throw new MessageCreatorException("Pin should be less than " + MessageConstants.MAX_PINS);

            var bytes = new List<byte> {MessageConstants.SYSEX_START, SysexCommands.EXTENDED_ANALOG,message.Pin};
            bytes.AddRange(BitHelper.GetBytesFromInt(message.Value));
            bytes.Add(MessageConstants.SYSEX_END);

            return bytes.ToArray();
        }
    }
}
