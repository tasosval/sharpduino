using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Handlers;

namespace Sharpduino.Library.Tests.Handlers
{
    [TestFixture]
    public class CapabilityMessageHandlerTest : BaseMessageHandlerTest<CapabilityMessageHandler>
    {
        private readonly byte[] sampleCapabilityBytes = new byte[] { 0x14, 0x76, 0x02 };

        private byte[] CreateMessageBytes(params byte[] capabilityBytes)
        {
            var bytes = new List<byte>
            {
                MessageConstants.SYSEX_START, // Start message
                SysexCommands.CAPABILITY_RESPONSE, // Message Type                
            };

            bytes.AddRange(capabilityBytes);
            bytes.Add(MessageConstants.SYSEX_END);

            return bytes.ToArray();
        }

        protected override CapabilityMessageHandler CreateHandler()
        {
            return new CapabilityMessageHandler(mockBroker.Object);
        }

        [Test,TestCaseSource("Cases")]
        public void Successful_Message(params byte[] capabilityBytes)
        {
            var bytes = CreateMessageBytes(capabilityBytes);
            throw new NotImplementedException();
        }

        [Test, TestCaseSource("Cases")]
        public void Successful_Message_Raises_Appropriate_Event(params byte[] capabilityBytes)
        {
            var bytes = CreateMessageBytes(capabilityBytes);
            throw new NotImplementedException();
        }

        [Test]
        public void Handler_Resets_After_Successful_Message()
        {
            throw new NotImplementedException();
        }

        [Test]
        public override void Ignores_All_Other_Messages()
        {
            base.Ignores_All_Other_Messages();
        }

        [Test]
        public override void Throws_Error_If_Forced_Other_Message()
        {
            base.Throws_Error_If_Forced_Other_Message();
        }

        static object[] Cases =
        {
            new byte[] { (byte) PinModes.Input, 1, (byte) PinModes.Output, 1,  (byte) PinModes.Analog, 10, MessageConstants.FINISHED_PIN_CAPABILITIES},
            new byte[] { (byte) PinModes.Input, 1, (byte) PinModes.Output, 1,  (byte) PinModes.Analog, 10, MessageConstants.FINISHED_PIN_CAPABILITIES, (byte) PinModes.Analog, 10, MessageConstants.FINISHED_PIN_CAPABILITIES },
            new byte[] {  } 
        };

        public class MyFactoryClass
        {
            public static IEnumerable TestCases
            {
                get
                {
                    yield return new TestCaseData(new byte[] { (byte)PinModes.Input, 1, (byte)PinModes.Output, 1, (byte)PinModes.Analog, 10, MessageConstants.FINISHED_PIN_CAPABILITIES }).Returns(1);
                    yield return new TestCaseData(new byte[] { (byte) PinModes.Input, 1, (byte) PinModes.Output, 1,  (byte) PinModes.Analog, 10, MessageConstants.FINISHED_PIN_CAPABILITIES, (byte) PinModes.Analog, 10, MessageConstants.FINISHED_PIN_CAPABILITIES }).Returns(2);
                    yield return new TestCaseData(12, 4).Returns(2);
                    yield return new TestCaseData(new byte[] { 0xFF, 0})
                      .Throws(typeof(MessageHandlerException))
                      .SetName("Send Wrong PinMode")
                      .SetDescription("An exception is expected");
                }
            }
        }
    }
}
