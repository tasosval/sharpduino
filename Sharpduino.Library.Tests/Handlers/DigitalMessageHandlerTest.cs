using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Moq;
using Sharpduino.Library.Base;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Handlers;
using Sharpduino.Library.Base.Messages;

namespace Sharpduino.Library.Tests.Handlers
{
	[TestFixture]
	public class DigitalMessageHandlerTest : BaseMessageHandlerTest<DigitalMessageHandler>
	{
        protected override DigitalMessageHandler CreateHandler()
        {
            return new DigitalMessageHandler(mockBroker.Object);
        }

		[Test]
		public void Succesful_Digital_Message()
		{
		    var bytes = new byte[]
		                    {
		                        // Start Digital Message              Pin
		                        (byte) (handler.START_MESSAGE | 0x05),
		                        0x7F, // 0-6 bitmask
                                0x01, // 7-13 bitmask
		                    };

		    for (int i = 0; i < bytes.Length-1; i++)
		    {
		        Assert.IsTrue(handler.CanHandle(bytes[i]));
                Assert.IsTrue(handler.Handle(bytes[i]));
		    }

            Assert.IsTrue(handler.CanHandle(bytes.Last()));
		    Assert.IsFalse(handler.Handle(bytes.Last()));

            mockBroker.Verify(p => p.CreateEvent(It.Is<DigitalMessage>(mes =>
                mes.Port == (bytes[0] & BaseMessageHandler.MESSAGEPINMASK) &&
                mes.PinStates == BitHelper.Sevens2Fourteen(bytes[1],bytes[2]))),
                Times.Once());

            // Check if the handler has reset
            Assert.IsTrue(handler.CanHandle(bytes[0]));
		}		

        [Test]
        public override void Ignores_All_Other_Messages()
        {
            base.Ignores_All_Other_Messages();
        }
        

        [Test]
        public void Throws_Exception_When_Given_Other_Message()
        {
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                if ((i & BaseMessageHandler.MESSAGETYPEMASK) != handler.START_MESSAGE)
                    Assert.Throws<MessageHandlerException>(() => handler.Handle(i));
            }
        }
	}
}

