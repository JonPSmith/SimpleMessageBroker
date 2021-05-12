using System;
using System.Threading;
using System.Threading.Tasks;
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
            broker.RegisterGetter("ClassToSend", x => new ClassToSend(x));
            var result = broker.AskFor<ClassToSend>("ClassToSend", "hello");

            //VERIFY
            Assert.Equal("hello", result.Data);
        }


        [Fact]
        public async Task TestRegisterAsyncValue()
        {
            //SETUP
            var broker = new MessageBroker();

            //ATTEMPT
            broker.RegisterGetter("ClassToSend", async x =>
            {
                await Task.Delay(1); 
                return new ClassToSend(x);
            });
            var result = broker.AskFor<ClassToSend>("ClassToSend", "hello");

            //VERIFY
            Assert.Equal("hello", result.Data);
        }

        [Fact]
        public void TestAskNoRegister()
        {
            //SETUP
            var broker = new MessageBroker();

            //ATTEMPT
            var ex = Assert.Throws<ArgumentException>(() => broker.AskFor<ClassToSend>("ClassToSend", "hello"));

            //VERIFY
            Assert.Equal("There is no provider registered for ClassToSend.", ex.Message);
        }

        [Fact]
        public void TestRemoveProvider()
        {
            //SETUP
            var broker = new MessageBroker();
            broker.RegisterGetter("ClassToSend", x => new ClassToSend(x));
            broker.AskFor<ClassToSend>("ClassToSend", "hello");

            //ATTEMPT
            broker.RemoveGetter("ClassToSend");
            var ex = Assert.Throws<ArgumentException>(() => broker.AskFor<ClassToSend>("ClassToSend", "hello"));

            //VERIFY
            Assert.Equal("There is no provider registered for ClassToSend.", ex.Message);
        }

        [Fact]
        public void TestRegisterAskEachDifferent()
        {
            //SETUP
            var broker = new MessageBroker();
            broker.RegisterGetter("ClassToSend", x => new ClassToSend(DateTime.UtcNow.ToString("O")));

            //ATTEMPT
            var result1 = broker.AskFor<ClassToSend>("ClassToSend");
            Thread.Sleep(10);
            var result2 = broker.AskFor<ClassToSend>("ClassToSend");

            //VERIFY
            Assert.NotNull(result1);
            Assert.NotEqual(result1, result2);
        }

        public class Class1
        {
            public Class1(int myInt, string data)
            {
                MyInt = myInt;
                Data = data;
                IgnoreThis = 123;
            }

            public int MyInt { get; set; }
            public string Data { get; set; }
            public int IgnoreThis { get; set; }
        }

        public class Class2
        {
            public int MyInt { get; set; }
            public string Data { get; set; }
        }

        [Fact]
        public void TestRegisterAskWithTypeChange()
        {
            //SETUP
            var broker = new MessageBroker();

            //ATTEMPT
            broker.RegisterGetter("DiffClasses", x => new Class1(999, x));
            var result = broker.AskFor<Class2>("DiffClasses", "hello");

            //VERIFY
            Assert.Equal(999, result.MyInt);
            Assert.Equal("hello", result.Data);
        }

        public class Class3
        {
            public int MyInt { get; set; }
            public string Data { get; set; }
            public DateTime ExtraDatetime { get; set; }
        }

        [Fact]
        public void TestRegisterAskTypeHasValueNotInProviderType()
        {
            //SETUP
            var broker = new MessageBroker();

            //ATTEMPT
            broker.RegisterGetter("DiffClasses", x => new Class1(999, x));
            var result = broker.AskFor<Class3>("DiffClasses", "hello");

            //VERIFY
            Assert.Equal(999, result.MyInt);
            Assert.Equal("hello", result.Data);
            Assert.Equal(default, result.ExtraDatetime);
        }
    }
}
