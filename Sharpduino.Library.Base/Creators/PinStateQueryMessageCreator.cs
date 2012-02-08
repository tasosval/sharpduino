using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.Send;

namespace Sharpduino.Library.Base.Creators
{
    public class PinStateQueryMessageCreator : BaseMessageCreator<PinStateQueryMessage>
    {
        public override byte[] CreateMessage(PinStateQueryMessage message)
        {
            if (message == null)
                throw new MessageCreatorException("This is not a valid PinState Query Message");

            if (message.Pin > MessageConstants.MAX_PINS)
                throw new MessageCreatorException("Pin should be less than " + MessageConstants.MAX_PINS);

            return new byte[]
                       {
                           MessageConstants.SYSEX_START,
                           SysexCommands.PIN_STATE_QUERY,
                           message.Pin,
                           MessageConstants.SYSEX_END
                       };
        }
    }
}
