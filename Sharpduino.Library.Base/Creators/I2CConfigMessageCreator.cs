using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.Send;

namespace Sharpduino.Library.Base.Creators
{
    public class I2CConfigMessageCreator : BaseMessageCreator<I2CConfigMessage>
    {
        public override byte[] CreateMessage(I2CConfigMessage message)
        {
            if ( message == null )
                throw new MessageCreatorException("This is not an I2CConfigMessage");

            byte lsb, msb;
            BitHelper.IntToBytes(message.Delay,out lsb,out msb);
            
            return new byte[]
                       {
                           MessageConstants.SYSEX_START,
                           SysexCommands.I2C_CONFIG,
                           (byte) (message.IsPowerPinOn ? 1 : 0),
                           lsb, msb,
                           MessageConstants.SYSEX_END
                       };
        }
    }
}
