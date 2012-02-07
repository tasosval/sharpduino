using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Messages.Send;

namespace Sharpduino.Library.Base.Creators
{
    public class I2CConfigMessageCreator : BaseMessageCreator<I2CConfigMessage>
    {
        public override byte[] CreateMessage(I2CConfigMessage message)
        {
            var bytes = new List<byte>();

            bytes.Add(MessageConstants.SYSEX_START);
            throw new NotImplementedException();
            bytes.Add(MessageConstants.SYSEX_END);

            return bytes.ToArray();
        }
    }
}
