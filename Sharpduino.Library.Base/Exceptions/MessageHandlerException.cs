using System;

namespace Sharpduino.Library.Base.Exceptions
{
    public class MessageHandlerException : Exception
    {
        public MessageHandlerException(string message) : base(message)
        {
        }
    }
}