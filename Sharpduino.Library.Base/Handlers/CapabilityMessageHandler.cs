using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Messages;

namespace Sharpduino.Library.Base.Handlers
{
    public class CapabilityMessageHandler : BaseMessageHandler<CapabilityMessage>
    {
        public CapabilityMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
        {}

        protected override void OnResetHandlerState()
        {
            throw new NotImplementedException();
        }

        public override bool CanHandle(byte firstByte)
        {
            throw new NotImplementedException();
        }

        public override bool Handle(byte messageByte)
        {
            throw new NotImplementedException();
        }
    }
}
