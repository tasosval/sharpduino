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

        private byte[] CreateMessageBytes()
        {
            const string name = "TEST";
            var message = new byte[5 + name.Length * 2];


            message[0] = handler.START_MESSAGE;
            message[1] = SysexCommands.STRING_DATA;

            for (int i = 0; i < name.Length; i++)
            {
                byte lsb, msb;
                BitHelper.Fourteen2Sevens(name[i], out lsb, out msb);
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
    }
}
