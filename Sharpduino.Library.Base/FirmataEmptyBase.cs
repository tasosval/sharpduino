using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
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

        /// <summary>
        /// Incoming Data as a Queue of bytes, which is suitable for the handling mechanism
        /// </summary>
        protected Queue<byte> IncomingData;

        /// <summary>
        /// The messagebroker that handles all the incoming Message event creation
        /// </summary>
        protected MessageBroker MessageBroker;

        /// <summary>
        /// The messageCreators dictionary that will help create any message we want to send
        /// </summary>
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
        protected FirmataBase(string portName) : base(portName) { }

        public override void Initialize()
        {
            base.Initialize();
            AddBasicMessageHandlers();
            AddBasicMessageCreators();
        }

        private void AddBasicMessageCreators()
        {
            string @namespace = "Sharpduino.Library.Base.Creators";
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && !t.IsAbstract &&                     //We are searching for a non-abstract class 
                        t.Namespace == @namespace &&                        //in the namespace we provide                        
                        t.GetInterfaces().Any(x => x.GetGenericTypeDefinition() == typeof(IMessageCreator<>)) //that implements IMessageCreator<>
                    select t;
            q.ToList().ForEach(
                t => MessageCreators[t.GetGenericTypeDefinition()] = (IMessageCreator) Activator.CreateInstance(t));
        }

        private void AddBasicMessageHandlers()
        {
            AvailableHandlers.Add(new AnalogMappingMessageHandler(MessageBroker));
            AvailableHandlers.Add(new AnalogMessageHandler(MessageBroker));
            AvailableHandlers.Add(new CapabilityMessageHandler(MessageBroker));
            AvailableHandlers.Add(new DigitalMessageHandler(MessageBroker));
            AvailableHandlers.Add(new I2CMessageHandler(MessageBroker));
            AvailableHandlers.Add(new PinStateMessageHandler(MessageBroker));
            AvailableHandlers.Add(new ProtocolVersionMessageHandler(MessageBroker));
            AvailableHandlers.Add(new SysexStringMessageHandler(MessageBroker));
            AvailableHandlers.Add(new SysexFirmwareMessageHandler(MessageBroker));
        }


    }
}
