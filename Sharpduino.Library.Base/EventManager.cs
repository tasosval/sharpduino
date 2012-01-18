using System;
using System.Collections.Generic;
using NLog;

namespace Sharpduino.Library.Base
{
    public interface IEventManager
    {
        void Subscribe<T>(IHandle<T> handler);
        void UnSubscribe<T>(IHandle<T> handler);
        void CreateEvent<T>(T message);
    }

    public class EventManager : IEventManager
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<Type,List<IHandle>> handlers;

        public EventManager()
        {
            log.Debug("Initializing EventManager");
            this.handlers = new Dictionary<Type, List<IHandle>>();
        }

        public void Subscribe<T>(IHandle<T> handler)
        {
            if (!handlers.ContainsKey(typeof(T)))
            {
                log.Debug("There was no handler for type {0}",typeof(T).ToString());
                handlers[typeof(T)] = new List<IHandle>();
            }

            handlers[typeof(T)].Add(handler);

            log.Info("Handler for message {0} subscribed", typeof(T).ToString() );
            
        }

        public void UnSubscribe<T>(IHandle<T> handler)
        {
            if (handlers == null || !handlers.ContainsKey(typeof(T)) || !handlers[typeof(T)].Contains(handler))
            {
                log.Warn("Tried to unsubscribe a non existing handler");
                return;
            }

            handlers[typeof (T)].Remove(handler);
            log.Info("Handler for messages of type {0} was removed",typeof(T).ToString());

            if (handlers[typeof(T)].Count == 0)
            {
                handlers.Remove(typeof (T));
                log.Debug("There were no other handlers for type {0}, so the list was removed",typeof(T).ToString());
            }
        }

        public void CreateEvent<T>(T message)
        {
            if (handlers == null || !handlers.ContainsKey(typeof(T)) || handlers[typeof(T)].Count == 0)
            {
                log.Warn("There was no handler for message of type {0}", typeof(T).ToString());
                return;
            }

            foreach (var handler in handlers[typeof(T)])
            {
                try
                {
                    ((IHandle<T>)handler).Handle(message);
                }
                catch (InvalidCastException ex)
                {
                    log.ErrorException("The handler was not of the right type. This should not have happened at all...",ex);
                    throw;
                }                
            }
        }
    }

    

    public interface IHandle
    {
        //void Handle(object message);
    }

    public interface IHandle<T> : IHandle
    {
        void Handle(T message);
    }
}