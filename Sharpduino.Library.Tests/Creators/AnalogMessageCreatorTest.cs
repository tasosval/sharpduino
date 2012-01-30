using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sharpduino.Library.Base;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Creators;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.TwoWay;

namespace Sharpduino.Library.Tests.Creators
{
    [TestFixture]
    public class AnalogMessageCreatorTest
    {
        [Test]
        public void Creates_Appropriate_Message()
        {
            var creator = new AnalogOutputMessageCreator();
            var bytes = creator.CreateMessage(new AnalogMessage {Pin = 3, Value = 5});
            byte lsb, msb;
            BitHelper.IntToBytes(5, out lsb, out msb);
            Assert.AreEqual(bytes[0],MessageConstants.ANALOG_MESSAGE | 3);
            Assert.AreEqual(bytes[1],lsb);
            Assert.AreEqual(bytes[2],msb);
        }

        [Test]
        public void Throws_Error_On_Wrong_Message()
        {
            var creator = new AnalogOutputMessageCreator();
            Assert.Throws<MessageCreatorException>(() => creator.CreateMessage(new DigitalMessage()));
        }
    }
}
