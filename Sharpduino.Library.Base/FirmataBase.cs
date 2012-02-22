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
            var messageCreators = (from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && !t.IsAbstract &&                     //We are searching for a non-abstract class 
                          t.Namespace == @namespace &&                        //in the namespace we provide                        
                          t.GetInterfaces().Any(x => x.GetGenericTypeDefinition() == typeof(IMessageCreator<>)) //that implements IMessageCreator<>
                    select t).ToList();

            // Create an instance for each type we found and add it to the MessageCreators with 
            // the Message Type that it creates as a key
            messageCreators.ForEach(
                t => MessageCreators[t.BaseType.GetGenericArguments()[0]] = (IMessageCreator)Activator.CreateInstance(t));
        }

        private void AddBasicMessageHandlers()
        {
            string @namespace = "Sharpduino.Library.Base.Handlers";
            var messageCreators = (from t in Assembly.GetExecutingAssembly().GetTypes()
                                   where t.IsClass && !t.IsAbstract &&                     //We are searching for a non-abstract class 
                                         t.Namespace == @namespace &&                        //in the namespace we provide
                                         t.GetInterfaces().Any(x => x == typeof(IMessageHandler)) //that implements IMessageHandler
                                   select t).ToList();

            // Create an instance for each type we found and add it to the AvailableHandlers\
            messageCreators.ForEach(
                t => AvailableHandlers.Add((IMessageHandler)Activator.CreateInstance(t,MessageBroker)));
        }
    }
}