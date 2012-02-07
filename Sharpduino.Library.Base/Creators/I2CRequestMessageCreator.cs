using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.Send;

namespace Sharpduino.Library.Base.Creators
{
    public class I2CRequestMessageCreator : BaseMessageCreator<I2CRequestMessage>
    {
        public override byte[] CreateMessage(I2CRequestMessage message)
        {
            if ( message == null )
                throw new MessageCreatorException("This is not a valid I2C Request Message");

            byte lsb, msb;
            BitHelper.IntToBytes(message.SlaveAddress,out lsb,out msb);
            var addressMode = (byte) (message.IsAddress10BitMode ? 0x20 : 0x00);
            addressMode |= (byte) message.ReadWriteOptions;

            var bytes = new List<byte>
                            {
                                MessageConstants.SYSEX_START, 
                                SysexCommands.I2C_REQUEST, 
                                lsb, 
                                (byte) (msb | addressMode)
                            };
            foreach (int t in message.Data)
            {
                BitHelper.IntToBytes(t,out lsb, out msb);
                bytes.Add(lsb);
                bytes.Add(msb);
            }
            bytes.Add(MessageConstants.SYSEX_END);

            return bytes.ToArray();
        }
    }
}
