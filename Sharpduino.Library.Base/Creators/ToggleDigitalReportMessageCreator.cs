using System;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.Send;

namespace Sharpduino.Library.Base.Creators
{
    public class ToggleDigitalReportMessageCreator : BaseMessageCreator<ToggleDigitalReportMessage>
    {
        public override byte[] CreateMessage(ToggleDigitalReportMessage message)
        {
            if (message == null)
                throw new MessageCreatorException("This is not a valid Toggle Digital Report Message");

            if (message.Port > MessageConstants.MAX_DIGITAL_PORTS)
                throw new MessageCreatorException("The port should be less than " + MessageConstants.MAX_DIGITAL_PORTS);

            return new byte[]
                       {
                           (byte) (MessageConstants.REPORT_DIGITAL_PORT | message.Port),
                           (byte) (message.ShouldBeEnabled ? 1 : 0)
                       };
        }
    }
}