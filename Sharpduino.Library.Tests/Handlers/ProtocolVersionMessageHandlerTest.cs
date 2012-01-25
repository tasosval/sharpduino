using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Handlers;
using Sharpduino.Library.Base.Messages;

namespace Sharpduino.Library.Tests.Handlers
{
    [TestFixture]
    public class ProtocolVersionMessageHandlerTest : BaseMessageHandlerTest<ProtocolVersionMessageHandler>
    {
        private byte[] CreateMessage()
        {
            return new byte[]
                       {
                           handler.START_MESSAGE, //Start message
                           0x02, // Major Version
                           0x03 // MinorVersion
                       };
        }

        protected override ProtocolVersionMessageHandler CreateHandler()
        {
            return new ProtocolVersionMessageHandler(mockBroker.Object);
        }

        [Test]
        public void Successful_Message()
        {
            var bytes = CreateMessage();

            Assert.IsTrue(handler.CanHandle(bytes[0]));
            Assert.IsTrue(handler.Handle(bytes[0]));
            Assert.IsTrue(handler.CanHandle(bytes[1]));
            Assert.IsTrue(handler.Handle(bytes[1])); 
            Assert.IsTrue(handler.CanHandle(bytes[2]));
            Assert.IsFalse(handler.Handle(bytes[2]));

            mockBroker.Verify(p => p.CreateEvent(It.Is<ProtocolVersionMessage>(mes =>
                mes.MajorVersion == bytes[1] && mes.MinorVersion == bytes[2])));

            Assert.IsTrue(handler.CanHandle(bytes[0]));
        }

        [Test]
        public void Failed_Message_Due_To_Major_Version()
        {
            var bytes = CreateMessage();
            bytes[1] = 128;

            Assert.IsTrue(handler.CanHandle(bytes[0]));
            Assert.IsTrue(handler.Handle(bytes[0]));
            Assert.IsTrue(handler.CanHandle(bytes[1]));
            Assert.Throws<MessageHandlerException>(() => handler.Handle(bytes[1]));
            
            // Check to see if the handler has reset
            Assert.IsTrue(handler.CanHandle(bytes[0])); 
        }

        [Test]
        public void Failed_Message_Due_To_Minor_Version()
        {
            var bytes = CreateMessage();
            bytes[2] = 128;

            Assert.IsTrue(handler.CanHandle(bytes[0]));
            Assert.IsTrue(handler.Handle(bytes[0]));
            Assert.IsTrue(handler.CanHandle(bytes[1]));
            Assert.IsTrue(handler.Handle(bytes[1]));
            Assert.IsTrue(handler.CanHandle(bytes[2]));
            Assert.Throws<MessageHandlerException>(() => handler.Handle(bytes[2]));

            // Check to see if the handler has reset
            Assert.IsTrue(handler.CanHandle(bytes[0]));
        }
    }
}
