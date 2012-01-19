using Moq;
using NUnit.Framework;
using Sharpduino.Library.Base;
using Sharpduino.Library.Base.Handlers;

namespace Sharpduino.Library.Tests
{
    public abstract class BaseMessageHandlerTest<T> where T : BaseMessageHandler
    {
        protected Mock<IMessageBroker> mockBroker;
        protected T handler;

        protected abstract T CreateHandler();
		
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
                if ((i & BaseMessageHandler.MESSAGETYPEMASK) != handler.START_MESSAGE)
                    Assert.IsFalse(handler.CanHandle(i));
                else
                    Assert.IsTrue(handler.CanHandle(i));
            }
        }
    }
}