using System.Linq;
using Moq;
using NUnit.Framework;
using Sharpduino.Library.Base;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Handlers;
using Sharpduino.Library.Base.Messages;

namespace Sharpduino.Library.Tests.Handlers
{
    [TestFixture]
    public class AnalogMessageHandlerTest
    {
        private static byte[] GetMessage()
        {
            return new byte[]
                            {
                                AnalogMessageHandler.START_MESSAGE,
                                1, // Pin
                                0x01, //LSB bits 0-6
                                0x01, //MSB bits 0-6
                            };
        }

        [Test]
        public void Successfull_Analog_Message()
        {
            var bytes = GetMessage();

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

            // Check to see if the handler has reset and can handle a new analog message
            Assert.IsTrue(handler.CanHandle(bytes[0]));

            mockMessageBrocker.Verify(
                p => p.CreateEvent(
                    It.Is<AnalogMessage>(
                    mes => mes.Pin == 1 && mes.Value == BitHelper.Sevens2Fourteen(bytes[2],bytes[3]))),Times.Once());
        }

        [Test]
        public void Analog_Message_Fails_Due_To_Wrong_Pin()
        {
            var bytes = GetMessage();
            bytes[1] = 17;

            var mockMessageBrocker = new Mock<IMessageBroker>();

            var handler = new AnalogMessageHandler(mockMessageBrocker.Object);

            Assert.IsTrue(handler.CanHandle(bytes[0]));
            Assert.IsTrue(handler.Handle(bytes[0]));

            Assert.IsTrue(handler.CanHandle(bytes[1]));
            Assert.Throws<MessageHandlerException>(() => handler.Handle(bytes[1]));

            // Check to see if the handler has reset and can handle a new analog message
            Assert.IsTrue(handler.CanHandle(bytes[0]));
        }

        [Test]
        public void Does_Not_Handle_Other_Messages()
        {
            var mockBroker = new Mock<IMessageBroker>();
            var handler = new AnalogMessageHandler(mockBroker.Object);

            for (byte i = 0; i < byte.MaxValue; i++)
            {
                if ( i != AnalogMessageHandler.START_MESSAGE )
                    Assert.IsFalse(handler.CanHandle(i));
            }
        }
    }
}
