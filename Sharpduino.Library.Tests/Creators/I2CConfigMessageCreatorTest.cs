using NUnit.Framework;
using Sharpduino.Library.Base;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Creators;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.Send;
using Sharpduino.Library.Base.Messages.TwoWay;

namespace Sharpduino.Library.Tests.Creators
{
    [TestFixture]
    public class I2CConfigMessageCreatorTest : BaseMessageCreatorTest
    {
        [Test]
        public override void Creates_Appropriate_Message()
        {
            var bytes = new byte[]
                {
                    MessageConstants.SYSEX_START,
                    SysexCommands.I2C_CONFIG,
                    1,
                    120,
                    34,
                    MessageConstants.SYSEX_END
                };

            var message = new I2CConfigMessage();
            message.Delay = BitHelper.BytesToInt(120, 34);
            message.IsPowerPinOn = true;

            var creator = new I2CConfigMessageCreator();
            var newBytes = creator.CreateMessage(message);

            Assert.AreEqual(bytes,newBytes);
        }

        [Test]
        public override void Throws_Error_On_Wrong_Message()
        {
            var creator = new I2CConfigMessageCreator();
            Assert.Throws<MessageCreatorException>(() => creator.CreateMessage(new AnalogMessage()));
        }
    }
}