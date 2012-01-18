using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Sharpduino.Library.Base;
using Sharpduino.Library.Base.Handlers;
using Sharpduino.Library.Base.Messages;


namespace Sharpduino.Library.Tests.LiveTests.Handlers
{
    [TestFixture]
    public class AnalogMessageHandlerTest
    {
        [Test]
        public void Successfull_Analog_Message()
        {
            var bytes = new byte[]
                            {
                                AnalogMessageHandler.START_MESSAGE,
                                1, // Pin
                                0x01, //LSB bits 0-6
                                0x01, //MSB bits 0-6
                            };

            var mockMessageBrocker = new Mock<IMessageBroker>();

            var handler = new AnalogMessageHandler(mockMessageBrocker.Object);

            for (int index = 0; index < bytes.Length - 1; index++)
            {
                var b = bytes[index];
                Assert.IsTrue(handler.CanHandle(b));
                Assert.IsTrue(handler.Handle(b));
            }

            Assert.IsTrue(handler.CanHandle(bytes.Last()));
            Assert.IsFalse(handler.Handle(bytes.Last()));

            mockMessageBrocker.Verify(p => p.CreateEvent(It.IsAny<AnalogMessage>()),Times.Once());
        }
    }
}
