using System;

namespace Sharpduino.Library.Base.Exceptions
{
    public class InvalidFirmataMessageException : Exception
    {
        public InvalidFirmataMessageException(string message) : base(message)
        {
            
        }
    }
}