using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.Send;

namespace Sharpduino.Library.Base.Creators
{
    public class PinModeMessageCreator : BaseMessageCreator<PinModeMessage>
    {
        public override byte[] CreateMessage(PinModeMessage message)
        {
            if (message == null)
            {
                throw new MessageCreatorException("This is not a valid PinMode message");
            }

            if (message.Pin > MessageConstants.MAX_PINS)
                throw new MessageCreatorException("The pin should be less or equal to " + MessageConstants.MAX_PINS);

            return new byte[]
                       {
                           MessageConstants.SET_PIN_MODE,
                           message.Pin,
                           (byte) message.Mode
                       };
        }
    }
}
