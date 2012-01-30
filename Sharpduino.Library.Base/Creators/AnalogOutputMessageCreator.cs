using System;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.TwoWay;

namespace Sharpduino.Library.Base.Creators
{
    public class AnalogOutputMessageCreator : BaseMessageCreator<AnalogMessage>
    {
        public override byte[] CreateMessage(AnalogMessage message)
        {
            if (message == null)
            {
                throw new MessageCreatorException("This is not an analog message");
            }
            
            byte lsb, msb;
            BitHelper.IntToBytes(message.Value, out lsb, out msb);
            // TODO : see if the value should have any constraints
            return new byte[]
                       {
                           (byte) (MessageConstants.ANALOG_MESSAGE | message.Pin),
                           lsb, 
                           msb,
                       };
        }
    }
}