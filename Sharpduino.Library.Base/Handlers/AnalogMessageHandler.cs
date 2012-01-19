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
        private enum HandlerState
        {
            StartEnd,
            LSB,
            MSB
        }

        private HandlerState currentHandlerState;
        
        private AnalogMessage analogMessage;
        private byte LSBCache;
            

        public AnalogMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
        {
            currentHandlerState = HandlerState.StartEnd;
            START_MESSAGE = 0xE0;
        }

        public override bool CanHandle(byte firstByte)
        {
            switch (currentHandlerState)
            {
                case HandlerState.StartEnd:
                    return (firstByte & MESSAGETYPEMASK) == START_MESSAGE;
                case HandlerState.LSB:
                case HandlerState.MSB:
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
                currentHandlerState = HandlerState.StartEnd;
                throw new MessageHandlerException("Error with the incoming byte. This is not a valid AnalogMessage");
            }

            switch (currentHandlerState)
            {
                case HandlerState.StartEnd:
                    analogMessage = new AnalogMessage();
					analogMessage.Pin = messageByte & MESSAGEPINMASK;
                    currentHandlerState = HandlerState.LSB;
                    return true;
                case HandlerState.LSB:
                    LSBCache = messageByte;
                    currentHandlerState = HandlerState.MSB;
                    return true;
                case HandlerState.MSB:
                    analogMessage.Value = BitHelper.Sevens2Fourteen(LSBCache, messageByte);
                    messageBroker.CreateEvent(analogMessage);
                    currentHandlerState = HandlerState.StartEnd;
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
