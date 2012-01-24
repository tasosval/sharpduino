using System;
using System.Collections;
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
    public class CapabilityMessageHandlerTest : BaseMessageHandlerTest<CapabilityMessageHandler>
    {
        private readonly byte[] sampleCapabilityBytes = new byte[] {0x14, 0x76, 0x02};

        private byte[] CreateMessageBytes(params byte[] capabilityBytes)
        {
            var bytes = new List<byte>
                            {
                                MessageConstants.SYSEX_START,
                                // Start message
                                SysexCommands.CAPABILITY_RESPONSE,
                                // Message Type                
                            };

            bytes.AddRange(capabilityBytes);
            bytes.Add(MessageConstants.SYSEX_END);

            return bytes.ToArray();
        }

        protected override CapabilityMessageHandler CreateHandler()
        {
            return new CapabilityMessageHandler(mockBroker.Object);
        }

        [Test, TestCaseSource(typeof(MyFactoryClass), "CasesSuccess")]
        public void Successful_Message(params byte[] capabilityBytes)
        {
            var bytes = CreateMessageBytes(capabilityBytes);

            for (int i = 0; i < bytes.Length - 1; i++)
            {
                Assert.IsTrue(handler.CanHandle(bytes[i]));
                Assert.IsTrue(handler.Handle(bytes[i]));
            }

            Assert.IsTrue(handler.CanHandle(bytes.Last()));
            Assert.IsFalse(handler.Handle(bytes.Last()));
        }

        [Test, TestCaseSource(typeof (MyFactoryClass), "Cases")]
        public void Handler_Resets_After_Successful_Message(params byte[] capabilityBytes)
        {
            var bytes = CreateMessageBytes(capabilityBytes);

            for (int i = 0; i < bytes.Length; i++)
                handler.Handle(bytes[i]);

            Assert.IsTrue(handler.CanHandle(bytes[0]));
        }

        [Test, TestCaseSource(typeof(MyFactoryClass), "ReturnCases")]
        public int Successful_Message_Raises_Appropriate_Events(params byte[] capabilityBytes)
        {
            var bytes = CreateMessageBytes(capabilityBytes);
            var countMessage = 0;
            mockBroker.Setup(p => p.CreateEvent(It.IsAny<CapabilityMessage>())).Callback(() => countMessage++);

            for (int i = 0; i < bytes.Length; i++)
                handler.Handle(bytes[i]);

            return countMessage;
        }


        [Test]
        public void Successful_Message_Raises_Event_With_The_Right_Values()
        {
            var bytes = CreateMessageBytes(new byte[]
                                               {
                                                   (byte) PinModes.Input, 1, (byte) PinModes.Output, 1,
                                                   (byte) PinModes.Analog, 10,
                                                   MessageConstants.FINISHED_PIN_CAPABILITIES, 
                                                   (byte) PinModes.Analog, 10,
                                                   MessageConstants.FINISHED_PIN_CAPABILITIES
                                               });

            for (int i = 0; i < bytes.Length; i++)
                handler.Handle(bytes[i]);

            mockBroker.Verify(p => p.CreateEvent(
                It.Is<CapabilityMessage>(
                    mes => mes.PinNo == 0 &&
                           mes.Modes.Keys.Contains(PinModes.Input) && mes.Modes[PinModes.Input] == 1 &&
                           mes.Modes.Keys.Contains(PinModes.Output) && mes.Modes[PinModes.Output] == 1 &&
                           mes.Modes.Keys.Contains(PinModes.Analog) && mes.Modes[PinModes.Analog] == 10
                    )), Times.Once());

            mockBroker.Verify(p => p.CreateEvent(
                It.Is<CapabilityMessage>(
                    mes => mes.PinNo == 1 &&
                           mes.Modes.Keys.Contains(PinModes.Analog) && mes.Modes[PinModes.Analog] == 10
                    )), Times.Once());
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

        private static object[] Cases = {
                                            new []
                                                {
                                                    (byte) PinModes.Input, (byte) 1, (byte) PinModes.Output, (byte) 1,
                                                    (byte) PinModes.Analog, (byte) 10, MessageConstants.FINISHED_PIN_CAPABILITIES
                                                }
                                            ,
                                            new []
                                                {
                                                    (byte) PinModes.Input, (byte) 1, (byte) PinModes.Output, (byte) 1,
                                                    (byte) PinModes.Analog, (byte) 10, MessageConstants.FINISHED_PIN_CAPABILITIES,
                                                    (byte) PinModes.Analog, (byte) 10, MessageConstants.FINISHED_PIN_CAPABILITIES
                                                }
                                        };
    }

    public class MyFactoryClass
    {
        public static IEnumerable ReturnCases
        {
            get
            {
                yield return new TestCaseData(new byte[] { (byte)PinModes.Input, 1, (byte)PinModes.Output, 1, (byte)PinModes.Analog, 10, MessageConstants.FINISHED_PIN_CAPABILITIES }).Returns(1).SetName("One Pin");
                yield return new TestCaseData(new byte[] { (byte) PinModes.Input, 1, (byte) PinModes.Output, 1,  (byte) PinModes.Analog, 10, MessageConstants.FINISHED_PIN_CAPABILITIES, (byte) PinModes.Analog, 10, MessageConstants.FINISHED_PIN_CAPABILITIES }).Returns(2).SetName("Two Pins");
                yield return new TestCaseData(new byte[] { 0xFF, 0})
                    .Throws(typeof(MessageHandlerException))
                    .SetName("Send Wrong PinMode")
                    .SetDescription("An exception is expected").Returns(0);
                yield break;
            }
        }

        public static IEnumerable Cases
        {
            get
            {
                yield return new TestCaseData(
                                 new[]{
                                     (byte) PinModes.Input, (byte) 1, (byte) PinModes.Output, (byte) 1,
                                     (byte) PinModes.Analog, (byte) 10, MessageConstants.FINISHED_PIN_CAPABILITIES
                                 }).SetName("One Pin Simple");
                yield return new TestCaseData(new[]
                                 {
                                     (byte) PinModes.Input, (byte) 1, (byte) PinModes.Output, (byte) 1,
                                     (byte) PinModes.Analog, (byte) 10, MessageConstants.FINISHED_PIN_CAPABILITIES,
                                     (byte) PinModes.Analog, (byte) 10, MessageConstants.FINISHED_PIN_CAPABILITIES
                                 }).SetName("Two Pins Simple");
                yield break;
            }
        }

        public static IEnumerable CasesSuccess
        {
            get
            {
                yield return new TestCaseData(
                                 new[]{
                                     (byte) PinModes.Input, (byte) 1, (byte) PinModes.Output, (byte) 1,
                                     (byte) PinModes.Analog, (byte) 10, MessageConstants.FINISHED_PIN_CAPABILITIES
                                 }).SetName("One Pin Success");
                yield return new TestCaseData(new[]
                                 {
                                     (byte) PinModes.Input, (byte) 1, (byte) PinModes.Output, (byte) 1,
                                     (byte) PinModes.Analog, (byte) 10, MessageConstants.FINISHED_PIN_CAPABILITIES,
                                     (byte) PinModes.Analog, (byte) 10, MessageConstants.FINISHED_PIN_CAPABILITIES
                                 }).SetName("Two Pins Success");
                yield break;
            }
        }
    }
}
