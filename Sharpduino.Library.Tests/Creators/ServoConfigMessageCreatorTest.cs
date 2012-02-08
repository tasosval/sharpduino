using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class ServoConfigMessageCreatorTest : BaseMessageCreatorTest
    {
        [Test]
        public override void Creates_Appropriate_Message()
        {
            var bytes = new byte[] { MessageConstants.SYSEX_START,
                                     SysexCommands.SERVO_CONFIG,
                                     53,                                //Pin
                                     53,43,                             //Min
                                     46,16,                             //Max
                                     52,1,                             //Angle
                                     MessageConstants.SYSEX_END};
            var message = new ServoConfigMessage
                              {
                                  Pin = 53,
                                  Angle = 180,
                                  MinPulse = BitHelper.BytesToInt(53, 43),
                                  MaxPulse = BitHelper.BytesToInt(46, 16)
                              };
            var creator = new ServoConfigMessageCreator();
            var newBytes = creator.CreateMessage(message);
            Assert.AreEqual(bytes,newBytes);
        }

        [Test]
        public override void Throws_Error_On_Wrong_Message()
        {
            var creator = new ServoConfigMessageCreator();
            Assert.Throws<MessageCreatorException>(() => creator.CreateMessage(new AnalogMessage()));
        }
    }
}
