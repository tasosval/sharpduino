﻿using Moq;
using NUnit.Framework;
using Sharpduino.Library.Base;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Handlers;

namespace Sharpduino.Library.Tests
{
    public abstract class BaseMessageHandlerTest<T> where T : IMessageHandler
    {
        protected Mock<IMessageBroker> mockBroker;
        protected T handler;

        /// <summary>
        /// Method that should be overriden to create the handler that we want to test
        /// </summary>
        protected abstract T CreateHandler();
		
        /// <summary>
        /// Override this if you want a different setup than having
        /// a new mockbroker and instance of the handler for each test
        /// </summary>
        [SetUp]
        public virtual void SetupEachTest()
        {
            mockBroker = new Mock<IMessageBroker>();
            handler = CreateHandler();
        }

        /// <summary>
        /// This method checks to see if the handler ignores all other messages
        /// Override with the TestAttribute
        /// </summary>
        [Test]
        public virtual void Ignores_All_Other_Messages()
        {
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                if (i != handler.START_MESSAGE)
                    Assert.IsFalse(handler.CanHandle(i));
                else
                    Assert.IsTrue(handler.CanHandle(i));
            }
        }

        /// <summary>
        /// This method checks to see if the handler throws exceptions when forced other messages
        /// Override with the TestAttribute
        /// </summary>
        [Test]
        public virtual void Throws_Error_If_Forced_Other_Message()
        {
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                if (i != handler.START_MESSAGE)
                    Assert.Throws<MessageHandlerException>(() => handler.Handle(i));
                else
                    Assert.DoesNotThrow(() => handler.Handle(i));
            }
        }
    }
}