﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;
using Sharpduino.Library.Base;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Handlers;
using Sharpduino.Library.Base.Messages.Receive;

namespace Sharpduino.Library.Tests.LiveTests.Handlers
{
    [TestFixture]
    public class LiveSysexFirmwareMessageHandlerTest : LiveBaseMessageHandlerTest<SysexFirmwareMessageHandler>
    {
        protected override SysexFirmwareMessageHandler CreateHandler()
        {
            return new SysexFirmwareMessageHandler(mockBroker.Object);
        }

        [Test]
        public void Receive_Firmware_Message_From_Live_Arduino_Running_Standard_Firmata_2_3()
        {
            port.Open();
            /*  0  START_MESSAGE (0xF0)
                1  queryFirmware (0x79)
                2  END_SYSEX (0xF7)
             */
            port.Write(new byte[]{handler.START_MESSAGE,SysexCommands.QUERY_FIRMWARE,MessageConstants.SYSEX_END},0,3);

            // Wait for the arduino to reply
            Thread.Sleep(100);

            while (port.BytesToRead > 0)
            {
                var incomingByte = (byte) port.ReadByte();
                Assert.IsTrue(handler.CanHandle(incomingByte));
                handler.Handle(incomingByte);
            }

            mockBroker.Verify( p => p.CreateEvent(
                   It.Is<SysexFirmwareMessage>(mes => mes.MajorVersion == 2 && mes.MinorVersion == 3)),Times.Once());
        }
    }
}
