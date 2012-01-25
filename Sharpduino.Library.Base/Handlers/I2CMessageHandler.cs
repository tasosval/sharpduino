using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Messages;

namespace Sharpduino.Library.Base.Handlers
{
    public class I2CMessageHandler : SysexMessageHandler<I2CResponseMessage>
    {
        public I2CMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
        {}

        protected override void OnResetHandlerState()
        {
            throw new NotImplementedException();
        }

        public override bool CanHandle(byte firstByte)
        {
            throw new NotImplementedException();
        }

        protected override bool HandleByte(byte messageByte)
        {
            throw new NotImplementedException();
        }
    }
}
