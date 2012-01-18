using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpduino.Library.Base.Handlers
{
    public class AnalogMessageHandler : BaseMessageHandler
    {
        private enum AnalogMessageHandlerState
        {
            StartEnd,
            PinNumber,
            LSB,
            MSB
        }

        private AnalogMessageHandlerState currentState;

        public const byte START_MESSAGE = 0xE0;

        public AnalogMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
        {
            currentState = AnalogMessageHandlerState.StartEnd;
        }

        public override bool CanHandle(byte firstByte)
        {
            switch (currentState)
            {
                case AnalogMessageHandlerState.StartEnd:
                    break;
                case AnalogMessageHandlerState.PinNumber:
                    break;
                case AnalogMessageHandlerState.LSB:
                    break;
                case AnalogMessageHandlerState.MSB:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool Handle(byte messageByte)
        {
            throw new NotImplementedException();
        }
    }
}
