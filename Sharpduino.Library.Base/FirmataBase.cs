using System;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using Sharpduino.Library.Base.Creators;
using Sharpduino.Library.Base.Handlers;
using Sharpduino.Library.Base.SerialProviders;

namespace Sharpduino.Library.Base
{
    /// <summary>
    /// This is a firmata base class that adds all known message handlers.
    /// It is useful for the full firmata implementation
    /// </summary>
    public abstract class FirmataBase : FirmataEmptyBase
    {
        protected FirmataBase(ISerialProvider serialProvider) : base(serialProvider)
        {
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