using Sharpduino.Library.Base.Constants;

namespace Sharpduino.Library.Base.Messages.Send
{
    public class ResetMessage : StaticMessage
    {
        public ResetMessage() :base(MessageConstants.SYSTEM_RESET){}
    }
}
