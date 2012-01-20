using System.Text;
using Sharpduino.Library.Base.Exceptions;

namespace Sharpduino.Library.Base.Handlers
{
    public abstract class SysexMessageHandler<T> : BaseMessageHandler<T> where T : new()
    {
        protected StringBuilder firmwareName;
        protected int  currentByteCount;
        protected byte cacheChar;
        public readonly byte END_SYSEX = 0xF7;

        protected SysexMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
        {
            ResetHandlerState();
            START_MESSAGE = 0xF0;
        }

        protected virtual void ResetHandlerState()
        {
            currentByteCount = 0;
            cacheChar = 255;
            message = new T();
            firmwareName = new StringBuilder();
        }

        protected abstract bool HandleByte(byte messageByte);

        public override bool Handle(byte messageByte)
        {
            if (!CanHandle(messageByte))
            {
                // Reset the state of the handler
                ResetHandlerState();
                throw new MessageHandlerException(BaseExceptionMessage);
            }

            if (++currentByteCount > MAXDATABYTES)
            {
                // Reset the state of the handler
                ResetHandlerState();
                throw new MessageHandlerException(BaseExceptionMessage + "Max message length was exceeded.");
            }

            return HandleByte(messageByte);
        }

        protected void HandleChar(byte charByte)
        {
            if (cacheChar != 255)
            {
                firmwareName.Append((char)BitHelper.Sevens2Fourteen(cacheChar, charByte));
                cacheChar = 255;
            }
            else
                cacheChar = charByte;
        }
    }
}