using System;

namespace Sharpduino.Library.Base.Messages
{
	public struct DigitalMessage
	{
		public int Port{get;set;}
		public int PinStates{get;set;}
	}
}

