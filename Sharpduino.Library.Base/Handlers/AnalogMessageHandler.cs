using System;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages;

namespace Sharpduino.Library.Base.Handlers
{
	public class AnalogMessageHandler : BaseMessageHandler<AnalogMessage>
	{
		private enum HandlerState
		{
			StartEnd,
			LSB,
			MSB
		}

		private HandlerState currentHandlerState;
		
		private byte LSBCache;
			

		public AnalogMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
		{
			START_MESSAGE = 0xE0;
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
                    return (firstByte & MessageConstants.MESSAGETYPEMASK) == START_MESSAGE;
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
				ResetHandlerState();
				throw new MessageHandlerException("Error with the incoming byte. This is not a valid AnalogMessage");
			}

			switch (currentHandlerState)
			{
				case HandlerState.StartEnd:
                    message.Pin = messageByte & MessageConstants.MESSAGEPINMASK;
					currentHandlerState = HandlerState.LSB;
					return true;
				case HandlerState.LSB:
					LSBCache = messageByte;
					currentHandlerState = HandlerState.MSB;
					return true;
				case HandlerState.MSB:
					message.Value = BitHelper.Sevens2Fourteen(LSBCache, messageByte);
					messageBroker.CreateEvent(message);
                    ResetHandlerState();
					return false;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
