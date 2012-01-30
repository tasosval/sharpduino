using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using Sharpduino.Library.Base.Creators;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Handlers;
using Sharpduino.Library.Base.Messages.TwoWay;

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

        protected Dictionary<Type, IMessageCreator> MessageCreators;

        private bool processQueue;

        public FirmataEmptyBase(string portName)
        {
            AvailableHandlers = new List<IMessageHandler>();
            ComPort = new SerialPort(portName);
        }

        /// <summary>
        /// Initialize any 
        /// </summary>
        public virtual void Initialize()
        {
            throw new NotImplementedException();
            // TODO : open port
            // TODO : subscribe to port events
            AddExpansionMessageHandlers();
            processQueue = true;
            var t = new ThreadStart(ReceiveQueueThread);
            Thread thr = new Thread(t);
            thr.Start();
        }

        /// <summary>
        /// This method should be overriden to add any extension message handlers
        /// </summary>
        protected virtual void AddExpansionMessageHandlers() { }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void ReceiveQueueThread()
        {
            byte currentByte = 0x0;
            bool foundByteFlag = false;
            while (processQueue)
            {             
                lock (IncomingData)
                {
                    if (IncomingData.Count > 0)
                    {                    
                        currentByte = IncomingData.Dequeue();
                        foundByteFlag = true;
                    }
                }

                if ( foundByteFlag )
                    HandleByte(currentByte);
                else
                    Thread.Sleep(50);

                foundByteFlag = false;
            }
        }

        public void SendMessage<T>(T message)
        {
            var type = typeof (T);
            if (MessageCreators.ContainsKey(type))
            {
                var bytes = MessageCreators[type].CreateMessage(message);
                // TODO : Send bytes
                throw new NotImplementedException();
            }
            else
            {
                throw new FirmataException("Not recognizable message");
            }
        }

        private void HandleByte(byte currentByte)
        {
            throw new NotImplementedException();            
        }
    }

    /// <summary>
    /// This is a firmata base class that adds all known message handlers.
    /// It is useful for the full firmata implementation
    /// </summary>
    public abstract class FirmataBase : FirmataEmptyBase
    {
        public FirmataBase(string portName) : base(portName) { }

        public override void Initialize()
        {
            base.Initialize();
            AddBasicMessageHandlers();
        }

        private void AddBasicMessageHandlers()
        {
            AvailableHandlers.Add(new AnalogMappingMessageHandler(messageBroker));
            AvailableHandlers.Add(new AnalogMessageHandler(messageBroker));
            AvailableHandlers.Add(new CapabilityMessageHandler(messageBroker));
            AvailableHandlers.Add(new DigitalMessageHandler(messageBroker));
            AvailableHandlers.Add(new I2CMessageHandler(messageBroker));
            AvailableHandlers.Add(new PinStateMessageHandler(messageBroker));
            AvailableHandlers.Add(new ProtocolVersionMessageHandler(messageBroker));
            AvailableHandlers.Add(new SysexStringMessageHandler(messageBroker));
            AvailableHandlers.Add(new SysexFirmwareMessageHandler(messageBroker));
        }


    }
}
