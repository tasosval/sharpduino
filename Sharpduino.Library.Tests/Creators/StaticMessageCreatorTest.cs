using NUnit.Framework;
using Sharpduino.Library.Base.Creators;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.Send;
using Sharpduino.Library.Base.Messages.TwoWay;

namespace Sharpduino.Library.Tests.Creators
{
    [TestFixture]
    public class StaticMessageCreatorTest
    {
        [Test, TestCaseSource("messages")]
        public void Creates_Appropriate_Message(StaticMessage message)
        {
            var creator = new StaticMessageCreator();
            var bytes = creator.CreateMessage(message);
            Assert.AreEqual(bytes,message.Bytes);
        }

        [Test]
        public void Throws_Error_On_Wrong_Message()
        {
            var creator = new StaticMessageCreator();
            Assert.Throws<MessageCreatorException>(() => creator.CreateMessage(new AnalogMessage()));
        }

        private static object[] messages
        {
            get
            {
                return new object[]
                    {
                        new ProtocolVersionRequestMessage(),
                        new QueryFirmwareMessage(),
                        new ResetMessage()
                    };
            }
        }
    }
}
