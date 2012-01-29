﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.Receive;

namespace Sharpduino.Library.Base.Handlers
{
    public class ProtocolVersionMessageHandler : BaseMessageHandler<ProtocolVersionMessage>
    {
        private HandlerState currentHandlerState;
        protected new const string BaseExceptionMessage = "Error with the incoming byte. This is not a valid DigitalMessage. ";

        private enum HandlerState
        {
            StartEnd,
            MajorVersion,
            MinorVersion
        }

        public ProtocolVersionMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
        {
            START_MESSAGE = 0xF9;
        }

        protected override void OnResetHandlerState()
        {
            currentHandlerState = HandlerState.StartEnd;
        }

        public override bool CanHandle(byte firstByte)
        {
            switch (currentHandlerState)
            {
                case HandlerState.StartEnd:
                    return firstByte == START_MESSAGE;
                case HandlerState.MajorVersion:
                case HandlerState.MinorVersion:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool Handle(byte messageByte)
        {

            if (!CanHandle(messageByte))
            {
                ResetHandlerState();
                throw new MessageHandlerException(BaseExceptionMessage);
            }

            switch (currentHandlerState)
            {
                case HandlerState.StartEnd:
                    currentHandlerState = HandlerState.MajorVersion;
                    return true;
                case HandlerState.MajorVersion:
                    if (messageByte > 127)
                    {
                        currentHandlerState = HandlerState.StartEnd;
                        throw new MessageHandlerException(BaseExceptionMessage + "Major Version should be < 128.");
                    }
                    message.MajorVersion = messageByte;                    
                    currentHandlerState = HandlerState.MinorVersion;
                    return true;
                case HandlerState.MinorVersion:
                    if (messageByte > 127)
                    {
                        currentHandlerState = HandlerState.StartEnd;
                        throw new MessageHandlerException(BaseExceptionMessage + "Minor Version should be < 128.");
                    }
                    message.MinorVersion = messageByte;
                    messageBroker.CreateEvent(message);
                    ResetHandlerState();
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
