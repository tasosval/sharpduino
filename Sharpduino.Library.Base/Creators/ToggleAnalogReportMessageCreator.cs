using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.Send;

namespace Sharpduino.Library.Base.Creators
{
    public class ToggleAnalogReportMessageCreator : BaseMessageCreator<ToggleAnalogReportMessage>
    {
        public override byte[] CreateMessage(ToggleAnalogReportMessage message)
        {
            if ( message == null )
                throw new MessageCreatorException("This is not a valid Toggle Analog Report Message");

            if ( message.Pin > MessageConstants.MAX_ANALOG_PINS )
                throw new MessageCreatorException("The pin should be less than " + MessageConstants.MAX_ANALOG_PINS);

            return new byte[]
                       {
                           (byte) (MessageConstants.REPORT_ANALOG_PIN | message.Pin),
                           (byte) (message.ShouldBeEnabled ? 1 : 0)
                       };
        }
    }
}
