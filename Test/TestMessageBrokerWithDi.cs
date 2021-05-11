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

        private interface IBrokerProvider
        {
            void SetData(string data);
        }

        private class BrokerProvider : IBrokerProvider
        {
            public ClassToSend SendsThis { get; private set; }

            public BrokerProvider(IMessageBroker messageBroker)
            {
                messageBroker.RegisterProvider("Test", _ => SendsThis);
            }

            public void SetData(string data)
            {
                SendsThis = new ClassToSend(data);
            }
        }

        [Fact]
        public void TestRegisterMultipleTimes()
        {
            //SETUP
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IMessageBroker>(new MessageBroker());
            serviceCollection.AddTransient<IBrokerProvider, BrokerProvider>();
            var diProvider = serviceCollection.BuildServiceProvider();

            var broker = diProvider.GetRequiredService<IMessageBroker>();

            //ATTEMPT
            using (var scope = diProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                diProvider.GetRequiredService<IBrokerProvider>().SetData("hello");
            }
            using (var scope = diProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                diProvider.GetRequiredService<IBrokerProvider>().SetData("goodbye");
            }


            //VERIFY
            Assert.Equal("goodbye", broker.AskFor<ClassToSend>("Test").Data);
        }

    }
}
