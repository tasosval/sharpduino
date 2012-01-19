using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages;

namespace Sharpduino.Library.Base.Handlers
{
    public class AnalogMessageHandler : BaseMessageHandler
    {
        private enum AnalogMessageHandlerState
        {
            StartEnd,
            LSB,
            MSB
        }

        private AnalogMessageHandlerState currentHandlerState;

        public const byte START_MESSAGE = 0xE0;

        private AnalogMessage analogMessage;
        private byte LSBCache;
            

        public AnalogMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
        {
            currentHandlerState = AnalogMessageHandlerState.StartEnd;
        }

        public override bool CanHandle(byte firstByte)
        {
            switch (currentHandlerState)
            {
                case AnalogMessageHandlerState.StartEnd:
                    return (firstByte & MESSAGETYPEMASK) == START_MESSAGE;
                case AnalogMessageHandlerState.LSB:
                case AnalogMessageHandlerState.MSB:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool Handle(byte messageByte)
        {
            if (!CanHandle(messageByte))
            {
                // Reset the state of the handler
                currentHandlerState = AnalogMessageHandlerState.StartEnd;
                throw new MessageHandlerException("Error with the incoming byte. This is not a valid AnalogMessage");
            }

            switch (currentHandlerState)
            {
                case AnalogMessageHandlerState.StartEnd:
                    analogMessage = new AnalogMessage();
					analogMessage.Pin = messageByte & MESSAGEPINMASK;
                    currentHandlerState = AnalogMessageHandlerState.LSB;
                    return true;
                case AnalogMessageHandlerState.LSB:
                    LSBCache = messageByte;
                    currentHandlerState = AnalogMessageHandlerState.MSB;
                    return true;
                case AnalogMessageHandlerState.MSB:
                    analogMessage.Value = BitHelper.Sevens2Fourteen(LSBCache, messageByte);
                    messageBroker.CreateEvent(analogMessage);
                    currentHandlerState = AnalogMessageHandlerState.StartEnd;
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
