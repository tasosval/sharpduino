using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Messages.Send;

namespace Sharpduino.Library.Base.Creators
{
    public class I2CRequestMessageCreator : BaseMessageCreator<I2CRequestMessage>
    {
        public override byte[] CreateMessage(I2CRequestMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
