﻿using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sharpduino.Library.Base;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Handlers;
using Sharpduino.Library.Base.Messages;

namespace Sharpduino.Library.Tests.Handlers
{
    [TestFixture]
    public class SysexStringMessageHandlerTest : BaseMessageHandlerTest<SysexStringMessageHandler>
    {
        byte[] messageBytes;
        readonly string stringMessage = "TEST";

        private byte[] CreateMessageBytes()
        {
            var message = new byte[5 + stringMessage.Length * 2];

            message[0] = handler.START_MESSAGE;
            message[1] = SysexCommands.STRING_DATA;

            for (int i = 0; i < stringMessage.Length; i++)
            {
                byte lsb, msb;
                BitHelper.Fourteen2Sevens(stringMessage[i], out lsb, out msb);
                message[2 + 2 * i] = lsb;
                message[2 + 2 * i + 1] = msb;
            }
            message[message.Length - 1] = MessageConstants.SYSEX_END;

            return message;
        }

        protected override SysexStringMessageHandler CreateHandler()
        {
            return new SysexStringMessageHandler(mockBroker.Object);
        }

        public override void SetupEachTest()
        {
            base.SetupEachTest();
            messageBytes = CreateMessageBytes();
        }

        [Test]
        public void Successful_String_Message()
        {
            for (int i = 0; i < messageBytes.Length-1; i++)
            {
                Assert.IsTrue(handler.CanHandle(messageBytes[i]));
                Assert.IsTrue(handler.Handle(messageBytes[i]));
            }

            Assert.IsTrue(handler.CanHandle(messageBytes.Last()));
            Assert.IsFalse(handler.Handle(messageBytes.Last()));            
        }

        [Test]
        public void Handler_Creates_The_Appropriate_Message_Event()
        {
            // Get a full message
            for (int i = 0; i < messageBytes.Length; i++)
                handler.Handle(messageBytes[i]);

            mockBroker.Verify(p => p.CreateEvent(It.Is<SysexStringMessage>(
                mes => mes.Message.Equals(stringMessage,StringComparison.InvariantCultureIgnoreCase))), Times.Once());
        }

        [Test]
        public void Handler_Resets_After_Successful_Message()
        {
            // Get a full message
            for (int i = 0; i < messageBytes.Length; i++)
                handler.Handle(messageBytes[i]);
            
            Assert.IsTrue(handler.CanHandle(messageBytes[0]));
        }

        [Test]
        public void Handler_Creates_Error_On_Invalid_Command()
        {
            handler.Handle(messageBytes[0]);
            Assert.Throws<MessageHandlerException>(() => handler.Handle(0x04));

            // Check to see if it resets after this exception
            Assert.IsTrue(handler.CanHandle(messageBytes[0]));
        }

        [Test]
        public override void Throws_Error_If_Forced_Other_Message()
        {
            base.Throws_Error_If_Forced_Other_Message();
        }

        [Test]
        public override void Ignores_All_Other_Messages()
        {
            base.Ignores_All_Other_Messages();
        }
    }
}
