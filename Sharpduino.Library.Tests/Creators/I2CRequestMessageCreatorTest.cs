using NUnit.Framework;
using Sharpduino.Library.Base.Creators;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.Send;

namespace Sharpduino.Library.Tests.Creators
{
    [TestFixture]
    public class I2CRequestMessageCreatorTest : BaseMessageCreatorTest
    {
        [Test]
        public override void Creates_Appropriate_Message()
        {
            throw new System.NotImplementedException();
        }

        [Test]
        public override void Throws_Error_On_Wrong_Message()
        {
            var creator = new I2CRequestMessageCreator();
            Assert.Throws<MessageCreatorException>(() => creator.CreateMessage(new I2CConfigMessage()));
        }
    }
}