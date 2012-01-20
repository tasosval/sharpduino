using System;

namespace Sharpduino.Library.Base.Messages
{
	public class DigitalMessage
	{
		public int Port{get;set;}
		public bool[] PinStates{get;set;}
	}
}

