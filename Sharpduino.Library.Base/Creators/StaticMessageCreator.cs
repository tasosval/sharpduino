using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.Send;

namespace Sharpduino.Library.Base.Creators
{
    public class StaticMessageCreator : BaseMessageCreator<StaticMessage>
    {
        public override byte[] CreateMessage(StaticMessage message)
        {
            if (message == null)
                throw new MessageCreatorException("This is not a valid static message");
            return message.Bytes;
        }
    }
}
