using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Sharpduino.Library.Base;
using Sharpduino.Library.Base.Handlers;
using Sharpduino.Library.Base.Messages;

namespace Sharpduino.Library.Tests
{
    [TestFixture]
    public class EventManagerTest
    {
        [Test]
        public void Manager_Should_Call_Created_Event_In_An_Interested_Handler()
        {
            var mockSysexFirmwareMessageHandler = new Mock<IHandle<object>>();
            //mockSysexFirmwareMessageHandler.Setup(p => p.Handle(It.IsAny<SysexFirmwareMessage>())).Verifiable();

            var manager = new EventManager();
            manager.Subscribe(mockSysexFirmwareMessageHandler.Object);

            manager.CreateEvent(new object());

            mockSysexFirmwareMessageHandler.Verify(p => p.Handle(It.IsAny<object>()),Times.Once());
        }

        [Test]
        public void Manager_Should_Call_Created_Event_In_All_Interested_Handlers()
        {
            var mockSysexFirmwareMessageHandler = new Mock<IHandle<object>>();
            var mockSysexFirmwareMessageHandler2 = new Mock<IHandle<object>>();

            var manager = new EventManager();
            manager.Subscribe(mockSysexFirmwareMessageHandler.Object);
            manager.Subscribe(mockSysexFirmwareMessageHandler2.Object);

            manager.CreateEvent(new object());

            mockSysexFirmwareMessageHandler.Verify(p => p.Handle(It.IsAny<object>()), Times.Once());
            mockSysexFirmwareMessageHandler2.Verify(p => p.Handle(It.IsAny<object>()), Times.Once());
        }

        [Test]
        public void Manager_Should_Ignore_Not_Interested_Handlers()
        {
            var mockMessageHandler = new Mock<IHandle<SysexFirmwareMessage>>();
            var mockObjectHanlder = new Mock<IHandle<object>>();

            var manager = new EventManager();
            manager.Subscribe(mockMessageHandler.Object);
            manager.Subscribe(mockObjectHanlder.Object);

            manager.CreateEvent(new object());

            mockMessageHandler.Verify(p => p.Handle(It.IsAny<SysexFirmwareMessage>()),Times.Never());
            mockObjectHanlder.Verify(p => p.Handle(It.IsAny<object>()),Times.Once());

            manager.CreateEvent(new SysexFirmwareMessage());

            mockMessageHandler.Verify(p => p.Handle(It.IsAny<SysexFirmwareMessage>()), Times.Once());
            mockObjectHanlder.Verify(p => p.Handle(It.IsAny<object>()), Times.Once());
        }

        [Test]
        public void Manager_Successfully_Unscubscribes_Handlers()
        {
            var mockMessageHandler = new Mock<IHandle<object>>();

            var manager = new EventManager();
            manager.Subscribe(mockMessageHandler.Object);
            manager.UnSubscribe(mockMessageHandler.Object);
            manager.CreateEvent(new object());

            mockMessageHandler.Verify(p => p.Handle(It.IsAny<object>()),Times.Never());
        }
    }
}
