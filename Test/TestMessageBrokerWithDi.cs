using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using SimpleMessageBroker;
using Xunit;

namespace Test
{
    public class TestMessageBrokerWithDi
    {
        private class ClassToSend
        {
            public ClassToSend(string data)
            {
                Data = data;
            }

            public string Data { get; }
        }

        private interface IGetService : IGetterProvider {}

        private class GetService : IGetService
        {
            public object GetData(string dataString)
            {
                return new ClassToSend(dataString);
            }
        }

        [Fact]
        public void TestIMessageBrokerCreatedWithDi()
        {
            //SETUP
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IMessageBroker, MessageBroker>();
            serviceCollection.AddTransient<IGetService, GetService>();
            var diProvider = serviceCollection.BuildServiceProvider();


            //ATTEMPT
            var broker = diProvider.GetRequiredService<IMessageBroker>();

            //VERIFY
        }

        [Fact]
        public void TestRegisterGetterService()
        {
            //SETUP
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IMessageBroker, MessageBroker>();
            serviceCollection.AddTransient<IGetService, GetService>();
            var diProvider = serviceCollection.BuildServiceProvider();

            //ATTEMPT
            var broker = diProvider.GetRequiredService<IMessageBroker>();
            broker.RegisterGetterService<ClassToSend, IGetService>("Test");

            //VERIFY
            var result = broker.AskFor<ClassToSend>("Test", "XXX").Data;
            Assert.Equal("XXX", result);
        }

    }
}
