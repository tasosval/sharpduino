using System;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages;

namespace Sharpduino.Library.Base.Handlers
{
	public class DigitalMessageHandler : BaseMessageHandler<DigitalMessage>
	{
		private enum HandlerState
		{
			StartEnd,
			LSB,
			MSB
		}

		private HandlerState currentHandlerState;        
		private byte LSBCache;
			

		public DigitalMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
		{
			currentHandlerState = HandlerState.StartEnd;
			START_MESSAGE = 0x90;
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
				throw new MessageHandlerException("Error with the incoming byte. This is not a valid DigitalMessage");
			}

			switch (currentHandlerState)
			{
				case HandlerState.StartEnd:
					message = new DigitalMessage();
					message.Port = messageByte & MESSAGEPINMASK;
					currentHandlerState = HandlerState.LSB;
					return true;
				case HandlerState.LSB:
					LSBCache = messageByte;
					currentHandlerState = HandlerState.MSB;
					return true;
				case HandlerState.MSB:
					message.PinStates = BitHelper.PortVal2PinVals((byte) BitHelper.Sevens2Fourteen(LSBCache, messageByte));
					messageBroker.CreateEvent(message);
					currentHandlerState = HandlerState.StartEnd;
					return false;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}

