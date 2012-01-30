using System;

namespace Sharpduino.Library.Base.Exceptions
{
    public class FirmataException : Exception
    {
        public FirmataException(string message) : base(message){}
    }
}