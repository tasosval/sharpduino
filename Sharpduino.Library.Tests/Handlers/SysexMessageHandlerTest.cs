using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Sharpduino.Library.Base;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Handlers;
using Sharpduino.Library.Base.Messages;

namespace Sharpduino.Library.Tests.Handlers
{
    [TestFixture]
    public class SysexMessageHandlerTest : BaseMessageHandlerTest<SysexFirmwareMessageHandler>
    {
		byte[] messageBytes;
		
        private byte[] CreateMessageBytes()
        {
            return new byte[]
            {
                handler.START_MESSAGE,
                SysexFirmwareMessageHandler.QUERY_FIRMWARE,
                2, // Major version
                3, // Minor version
                Convert.ToByte('T'), // Firmware name
                Convert.ToByte('E'),
                Convert.ToByte('S'),
                Convert.ToByte('T'),
                SysexFirmwareMessageHandler.END_SYSEX
            };
        }

        protected override SysexFirmwareMessageHandler CreateHandler()
        {
            return new SysexFirmwareMessageHandler(mockBroker.Object);			
        }

        public override void SetupEachTest()
        {
            base.SetupEachTest();
            messageBytes = CreateMessageBytes();
        }

        [Test]
        public void Successfull_Sysex_Message()
        {
			for (int i = 0; i < messageBytes.Length-1; i++)
            {
                var messageByte = messageBytes[i];
                Assert.IsTrue(handler.CanHandle(messageByte));
                Assert.IsTrue(handler.Handle(messageByte));
            }

            Assert.IsTrue(handler.CanHandle(messageBytes.Last()));
            Assert.IsFalse(handler.Handle(messageBytes.Last()));
          			
            // Make sure that the CreateEvent method is called with the arguments that we expectx
            mockBroker.Verify(p => p.CreateEvent(It.Is<SysexFirmwareMessage>(
                    s => s.FirmwareName == "TEST" && 
					s.MajorVersion == 2 && 
					s.MinorVersion == 3)),Times.Once());
			 
            // See if the handler is reset and can again handle a new message
            Assert.IsTrue(handler.CanHandle(messageBytes[0]));
            Assert.IsFalse(handler.CanHandle(messageBytes[1]));
        }

        [Test]
        public void Failed_Sysex_Message_With_Wrong_Command_Byte()
        {
            messageBytes[1] = SysexFirmwareMessageHandler.END_SYSEX;

            // Give the first byte correctly
            Assert.IsTrue(handler.CanHandle(messageBytes[0]));
            Assert.IsTrue(handler.Handle(messageBytes[0]));

            // Try the second byte even though it should be wrong
            Assert.IsFalse(handler.CanHandle(messageBytes[1]));
            Assert.Throws<MessageHandlerException>(() => handler.Handle(messageBytes[1]));

            // See if the handler has reset
            Assert.IsTrue(handler.CanHandle(messageBytes[0]));
        }

        [Test]
        public void Failed_Sysex_Message_With_Exceeded_Bytecount()
        {
            int clamp = 0;
            for (int i = 0; i < BaseMessageHandler.MAXDATABYTES+2; i++)
            {
                clamp = i > 1 ? 2 : i;
                Assert.IsTrue(handler.CanHandle(messageBytes[clamp]));
                Assert.IsTrue(handler.Handle(messageBytes[clamp]));
            }
            
            // Although the handler can handle the byte, it should exceed the maximum MessageLength
            Assert.IsTrue(handler.CanHandle(messageBytes[2]));
            Assert.Throws<MessageHandlerException>(() => handler.Handle(messageBytes[2]));

            // See if the handler was reset
            Assert.IsTrue(handler.CanHandle(messageBytes[0]));
        }

        [Test]
        public override void Ignores_All_Other_Messages()
        {
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                if (i != handler.START_MESSAGE)
                    Assert.IsFalse(handler.CanHandle(i));
                else
                    Assert.IsTrue(handler.CanHandle(i));
            }
        }
    }
}
