using System;
using System.Threading;
using SimpleMessageBroker;
using Xunit;

namespace Test
{
    public class TestSimpleMessageBroker
    {
        private class ClassToSend
        {
            public ClassToSend(string data)
            {
                Data = data;
            }

            public string Data { get; }
        }

        [Fact]
        public void TestRegisterAsk()
        {
            //SETUP
            var broker = new MessageBroker();

            //ATTEMPT
            broker.RegisterProvider(x => new ClassToSend(x));
            var result = broker.AskFor<ClassToSend>("hello");

            //VERIFY
            Assert.Equal("hello", result.Data);
        }

        [Fact]
        public void TestRegisterAskEachDifferent()
        {
            //SETUP
            var broker = new MessageBroker();
            broker.RegisterProvider(x => new ClassToSend(DateTime.UtcNow.ToString("O")));

            //ATTEMPT
            var result1 = broker.AskFor<ClassToSend>();
            Thread.Sleep(10);
            var result2 = broker.AskFor<ClassToSend>();

            //VERIFY
            Assert.NotNull(result1);
            Assert.NotEqual(result1, result2);
        }
    }
}
