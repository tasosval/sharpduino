using Moq;
using NUnit.Framework;
using Sharpduino.Library.Base;
using Sharpduino.Library.Base.Constants;
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
        /// Override with the TestAttribute and a call to base.Ignores_All_Other_Messages() to use it
        /// </summary>
        public virtual void Ignores_All_Other_Messages()
        {
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                if ((i & MessageConstants.MESSAGETYPEMASK) != handler.START_MESSAGE)
                    Assert.IsFalse(handler.CanHandle(i));
                else
                    Assert.IsTrue(handler.CanHandle(i));
            }
        }
    }
}