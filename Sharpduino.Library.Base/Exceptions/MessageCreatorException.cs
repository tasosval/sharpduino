using System;

namespace Sharpduino.Library.Base.Exceptions
{
    public class MessageCreatorException : Exception
    {
        public MessageCreatorException(string message) : base(message){}
    }
}
