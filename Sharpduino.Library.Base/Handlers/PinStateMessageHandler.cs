using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages;

namespace Sharpduino.Library.Base.Handlers
{
    public class PinStateMessageHandler : SysexMessageHandler<PinStateMessage>
    {
        private const byte CommandByte = SysexCommands.PIN_STATE_RESPONSE;

        private enum HandlerState
        {
            StartEnd,
            Command,
            PinNo,
            PinMode,
            PinState
        }

        private HandlerState currentHandlerState;
        private int stateBytesReceived = 0;

        public PinStateMessageHandler(IMessageBroker messageBroker) : base(messageBroker){}
        
        protected override void OnResetHandlerState()
        {
            currentHandlerState = HandlerState.StartEnd;
            stateBytesReceived = 0;
        }

        public override bool CanHandle(byte firstByte)
        {
            switch (currentHandlerState)
            {
                case HandlerState.StartEnd:
                    return firstByte == START_MESSAGE;
                case HandlerState.Command:
                    return firstByte == CommandByte;
                case HandlerState.PinNo:                    
                case HandlerState.PinMode:
                case HandlerState.PinState:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override bool HandleByte(byte messageByte)
        {
            switch (currentHandlerState)
            {
                case HandlerState.StartEnd:
                    currentHandlerState = HandlerState.Command;
                    return true;
                case HandlerState.Command:
                    currentHandlerState = HandlerState.PinNo;
                    return true;
                case HandlerState.PinNo:
                    message.PinNo = messageByte;
                    currentHandlerState = HandlerState.PinMode;
                    return true;
                case HandlerState.PinMode:
                    message.Mode = (PinModes) messageByte;
                    currentHandlerState = HandlerState.PinState;
                    return true;
                case HandlerState.PinState:
                    if (messageByte == MessageConstants.SYSEX_END)
                    {
                        if ( stateBytesReceived == 0 )
                            throw new MessageHandlerException(BaseExceptionMessage + "There was no state in the message for pin " + message.PinNo);
                        messageBroker.CreateEvent(message);
                        ResetHandlerState();
                        return false;
                    }
                    message.State |= messageByte << ( stateBytesReceived * 7 );
                    stateBytesReceived++;                    
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
