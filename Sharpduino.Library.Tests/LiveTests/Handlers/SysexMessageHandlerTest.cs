using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;
using Sharpduino.Library.Base;
using Sharpduino.Library.Base.Handlers;
using Sharpduino.Library.Base.Messages;

namespace Sharpduino.Library.Tests.LiveTests.Handlers
{
    [TestFixture]
    public class SysexMessageHandlerTest
    {
        private SerialPort port;

        [SetUp]
        public void Init()
        {
            port = new SerialPort("COM3", 57600, Parity.None, 8, StopBits.One);            
        }

        [TearDown]
        public void Finish()
        {
            port.Close();
            port.Dispose();
        }

        [Test]
        public void Receive_Firmware_Message_From_Live_Arduino_Running_Standard_Firmata_2_3()
        {
            var mockEventManager = new Mock<IEventManager>();
            mockEventManager.Setup(
                p =>
                p.CreateEvent(
                    It.Is<SysexFirmwareMessage>(
                        mes => mes.MajorVersion == 2 && mes.MinorVersion == 3))).
                        Verifiable();

            SysexFirmwareMessageHandler handler = new SysexFirmwareMessageHandler(mockEventManager.Object);

            port.Open();
            /*  0  START_SYSEX (0xF0)
                1  queryFirmware (0x79)
                2  END_SYSEX (0xF7)
             */
            port.Write(new byte[]{SysexFirmwareMessageHandler.START_SYSEX,SysexFirmwareMessageHandler.QUERY_FIRMWARE,SysexFirmwareMessageHandler.END_SYSEX},0,3);

            // Wait for the arduino to reply
            Thread.Sleep(100);

            while (port.BytesToRead > 0)
            {
                var incomingByte = (byte) port.ReadByte();
                Assert.IsTrue(handler.CanHandle(incomingByte));
                handler.Handle(incomingByte);
            }

            mockEventManager.Verify(p => p.CreateEvent(It.IsAny<SysexFirmwareMessage>()),Times.Once());
            mockEventManager.Verify();
        }
    }
}
