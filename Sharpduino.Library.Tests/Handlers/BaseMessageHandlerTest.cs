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
    }
}