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
	public class SysexFirmwareMessageHandlerTest : BaseSysexMessageHandlerTest<SysexFirmwareMessageHandler>
	{
		byte[] messageBytes;
		
		private byte[] CreateMessageBytes()
		{
			const string name = "TEST";
			var message = new byte[5 + name.Length*2];
			

			message[0] = handler.START_MESSAGE;
			message[1] = SysexCommands.QUERY_FIRMWARE;
			message[2] = 2; // Major version
			message[3] = 3; // Minor version

			for (int i = 0; i < name.Length; i++)
			{
				byte lsb, msb;
				BitHelper.Fourteen2Sevens(name[i], out lsb, out msb);
				message[4 + 2*i] = lsb;
				message[4 + 2*i + 1] = msb;
			}
			message[message.Length-1] = MessageConstants.SYSEX_END;
			
			return message;
		}

		protected override SysexFirmwareMessageHandler CreateHandler()
		{
			return new SysexFirmwareMessageHandler(mockBroker.Object);			
		}

		public override void SetupEachTest()
		{
			base.SetupEachTest();
			messageBytes = CreateMessageBytes();
		}

        protected override byte SysexCommandByte
        {
            get { return SysexCommands.QUERY_FIRMWARE; }
        }

		[Test]
		public void Successfull_Sysex_Message()
		{
			for (int i = 0; i < messageBytes.Length-1; i++)
			{
				var messageByte = messageBytes[i];
				Assert.IsTrue(handler.CanHandle(messageByte));
				Assert.IsTrue(handler.Handle(messageByte));
			}

			Assert.IsTrue(handler.CanHandle(messageBytes.Last()));
			Assert.IsFalse(handler.Handle(messageBytes.Last()));
					
			// Make sure that the CreateEvent method is called with the arguments that we expectx
			mockBroker.Verify(p => p.CreateEvent(It.Is<SysexFirmwareMessage>(
					s => s.FirmwareName == "TEST" && 
					s.MajorVersion == 2 && 
					s.MinorVersion == 3)),Times.Once());
			 
			// See if the handler is reset and can again handle a new message
			Assert.IsTrue(handler.CanHandle(messageBytes[0]));
			Assert.IsFalse(handler.CanHandle(messageBytes[1]));
		}

		[Test]
		public void Failed_Sysex_Message_With_Wrong_Command_Byte()
		{
			messageBytes[1] = MessageConstants.SYSEX_END;

			// Give the first byte correctly
			Assert.IsTrue(handler.CanHandle(messageBytes[0]));
			Assert.IsTrue(handler.Handle(messageBytes[0]));

			// Try the second byte even though it should be wrong
			Assert.IsFalse(handler.CanHandle(messageBytes[1]));
			Assert.Throws<MessageHandlerException>(() => handler.Handle(messageBytes[1]));

			// See if the handler has reset
			Assert.IsTrue(handler.CanHandle(messageBytes[0]));
		}

		[Test]
		public void Failed_Sysex_Message_With_Exceeded_Bytecount()
		{
			int clamp = 0;
            for (int i = 0; i < MessageConstants.MAX_DATA_BYTES + 2; i++)
			{
				clamp = i > 1 ? 2 : i;
				Assert.IsTrue(handler.CanHandle(messageBytes[clamp]));
				Assert.IsTrue(handler.Handle(messageBytes[clamp]));
			}
			
			// Although the handler can handle the byte, it should exceed the maximum MessageLength
			Assert.IsTrue(handler.CanHandle(messageBytes[2]));
			Assert.Throws<MessageHandlerException>(() => handler.Handle(messageBytes[2]));

			// See if the handler was reset
			Assert.IsTrue(handler.CanHandle(messageBytes[0]));
		}

		[Test]
		public void Failed_Sysex_Message_With_Wrong_Firmware_Major_Version()
		{
			messageBytes[2] = 255; // Wrong major framework number

			for (int i = 0; i < 2; i++)
			{
				Assert.IsTrue(handler.CanHandle(messageBytes[i]));
				Assert.IsTrue(handler.Handle(messageBytes[i]));
			}

			Assert.IsTrue(handler.CanHandle(messageBytes[2]));
			Assert.Throws<MessageHandlerException>(() => handler.Handle(messageBytes[2]));
		}

		[Test]
		public void Failed_Sysex_Message_With_Wrong_Firmware_Minor_Version()
		{
			messageBytes[3] = 255; // Wrong major framework number

			for (int i = 0; i < 3; i++)
			{
				Assert.IsTrue(handler.CanHandle(messageBytes[i]));
				Assert.IsTrue(handler.Handle(messageBytes[i]));
			}

			Assert.IsTrue(handler.CanHandle(messageBytes[3]));
			Assert.Throws<MessageHandlerException>(() => handler.Handle(messageBytes[3]));
		}
	}
}
