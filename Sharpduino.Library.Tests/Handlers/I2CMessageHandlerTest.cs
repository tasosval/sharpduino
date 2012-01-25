using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Handlers;

namespace Sharpduino.Library.Tests.Handlers
{
    [TestFixture]
    public class I2CMessageHandlerTest : BaseSysexMessageHandlerTest<I2CMessageHandler>
    {

        protected override byte SysexCommandByte
        {
            get { return SysexCommands.I2C_REPLY; }
        }

        protected override I2CMessageHandler CreateHandler()
        {
            return new I2CMessageHandler(mockBroker.Object);
        }

        [Test]
        public void Successful_Message()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void Successful_Message_Raises_Event_With_Right_Data()
        {
            throw new NotImplementedException();
        }

    }
}
