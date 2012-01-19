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
            PinNumber,
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
                    return firstByte == START_MESSAGE;
                case AnalogMessageHandlerState.PinNumber:                    
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
                throw new MessageHandlerException("Error with the incoming byte. This is not a valid SysexMessage");
            }

            switch (currentHandlerState)
            {
                case AnalogMessageHandlerState.StartEnd:
                    analogMessage = new AnalogMessage();
                    currentHandlerState = AnalogMessageHandlerState.PinNumber;
                    return true;
                case AnalogMessageHandlerState.PinNumber:
                    if (messageByte > MAXANALOGPINS)
                        throw new MessageHandlerException(string.Format("The value {0} is not an analog pin", messageByte));
                    analogMessage.Pin = messageByte;
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
