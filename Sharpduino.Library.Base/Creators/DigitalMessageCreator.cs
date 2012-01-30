using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.TwoWay;

namespace Sharpduino.Library.Base.Creators
{
    public class DigitalMessageCreator : BaseMessageCreator<DigitalMessage>
    {
        public override byte[] CreateMessage(DigitalMessage message)
        {
            if (message == null)
            {
                throw new MessageCreatorException("This is not a digital message");
            }

            byte lsb, msb;
            BitHelper.IntToBytes(BitHelper.PinVals2PortVal(message.PinStates), out lsb, out msb);

            return new byte[]
                            {
                                (byte) (MessageConstants.DIGITAL_MESSAGE | message.Port),
                                lsb, 
                                msb
                            };
        }
    }
}
