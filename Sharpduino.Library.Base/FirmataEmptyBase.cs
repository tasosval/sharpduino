using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Handlers;

namespace Sharpduino.Library.Base
{
    /// <summary>
    /// This is the base class for the firmata protocol. It is a bare bones implementation
    /// that should be overriden to provide any functionality. It has no message handling.
    /// Useful if you want to implement your own subset of the firmata protocol.
    /// </summary>
    public class FirmataEmptyBase : IDisposable
    {
        /// <summary>
        /// The available handlers for this instance
        /// </summary>
        protected List<IMessageHandler> AvailableHandlers;

        /// <summary>
        /// The current handler. The next byte goes to this handler
        /// </summary>
        protected IMessageHandler CurrentHandler;

        /// <summary>
        /// The serial port 
        /// </summary>
        protected SerialPort ComPort;

        protected Queue<byte> IncomingData;

        protected MessageBroker messageBroker;

        public FirmataEmptyBase(string portName)
        {
            AvailableHandlers = new List<IMessageHandler>();
            ComPort = new SerialPort(portName);
        }

        public virtual void Initialize()
        {
            throw new NotImplementedException();
            // TODO : open port
            // TODO : subscribe to port events
            AddExpansionMessageHandlers();
        }

        /// <summary>
        /// This method should be overriden to add any extension message handlers
        /// </summary>
        protected virtual void AddExpansionMessageHandlers(){}

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// This is a firmata base class that adds all known message handlers.
    /// It is useful for the full firmata implementation
    /// </summary>
    public class FirmataBase : FirmataEmptyBase
    {
        public FirmataBase(string portName) : base(portName){}

        public override void Initialize()
        {
            base.Initialize();
            AddBasicMessageHandlers();
        }

        private void AddBasicMessageHandlers()
        {
            AvailableHandlers.Add(new SysexFirmwareMessageHandler(messageBroker));

            
        }
    }
}
