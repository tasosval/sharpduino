using System.Text;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Exceptions;

namespace Sharpduino.Library.Base.Handlers
{
    public abstract class SysexMessageHandler<T> : BaseMessageHandler<T> where T : new()
    {
        protected StringBuilder stringBuilder;
        protected int  currentByteCount;
        protected byte cacheChar;
        
        protected SysexMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
        {
            START_MESSAGE = MessageConstants.SYSEX_START;
        }

        protected override void ResetHandlerState()
        {
            base.ResetHandlerState();
            stringBuilder = new StringBuilder();
            currentByteCount = 0;
            cacheChar = 255;
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

            if (++currentByteCount > MessageConstants.MAX_DATA_BYTES)
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
                stringBuilder.Append((char)BitHelper.Sevens2Fourteen(cacheChar, charByte));
                cacheChar = 255;
            }
            else
                cacheChar = charByte;
        }
    }
}