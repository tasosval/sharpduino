using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sharpduino.Library.Base.Creators;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.TwoWay;

namespace Sharpduino.Library.Tests.Creators
{
    [TestFixture]
    public class DigitalMessageCreatorTest
    {
        [Test]
        public void Creates_Appropriate_Message()
        {
            throw new NotImplementedException();
        }

        public void Throws_Error_On_Wrong_Message()
        {
            var creator = new DigitalMessageCreator();
            Assert.Throws<MessageCreatorException>(() => creator.CreateMessage(new AnalogMessage()));
        }
    }
}
