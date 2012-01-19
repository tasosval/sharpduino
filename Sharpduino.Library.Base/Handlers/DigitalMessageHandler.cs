using System;
using Sharpduino.Library.Base.Messages;

namespace Sharpduino.Library.Base.Handlers
{
	public class DigitalMessageHandler : BaseMessageHandler
	{
        private enum AnalogMessageHandlerState
        {
            StartEnd,
            LSB,
            MSB
        }

        private AnalogMessageHandlerState currentHandlerState;

        public const byte START_MESSAGE = 0x90;

        private DigitalMessage message;
        private byte LSBCache;

		public DigitalMessageHandler (IMessageBroker broker) : base(broker){}

		public override bool CanHandle (byte firstByte)
		{
			throw new NotImplementedException ();
		}

		public override bool Handle (byte messageByte)
		{
			throw new NotImplementedException ();
		}
	}
}

