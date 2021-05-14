# Simple request/reply Message Broker

For my series of articles on [modular monolith architecture](https://www.thereformedprogrammer.net/evolving-modular-monoliths-1-an-architecture-for-net/) I needed a request/reply message broker that could pass data between isolated sections of my modular monolith.

At first I couldn't find a message broker that worked like a method call (most message broker use the publish/listen approach), so I build a simple request/reply message broker, but I now have found a possible request/reply broker in RabbitMQ (but it comes with a lot of other features I don't need).

RabbitMQ has a [remote procedure call feature](https://www.rabbitmq.com/tutorials/tutorial-six-dotnet.html) that uses a queue, but [this page](https://www.rabbitmq.com/direct-reply-to.html) shows how to turn off the queue.

MIT license.

## Example of simple usage

My simple message broker can be used without dependency injection (DI) as shown below. This isn't the normal use but is a simple example to understand waht is going on.

```c#
var broker = new MessageBroker(null);

//You register a communication link called "link-name"
//Which will receive a string from the asker and returns class 
broker.RegisterGetter("link-name", 
    fromAsker => new ClassToSend(fromAsker));

//To ask the getter on the same communication link name
//for a class, sending an optional string
var result = broker.AskFor<ClassToSend>("link-name", "hello");
```

## Example of usage with DI

In real applications you would use DI, with the message broker register as a singleton (that's important) with your DI provider. Then you should register all your getter services using the `RegisterGetterService` method. This allows your Getter class to use DI to get other services, such as access to the database.

When the `AskFor` is called the broker will create a scoped instance of your class to get the data and then dispose of it.

The code below shows this in action.

```c#
var broker = //You get the singleton message broker via DI
//You register a getter service with the interface IGetService
broker.RegisterGetterService<ClassToSend, IGetService>("Test");

//When you ask for the get data the broker will create a scoped instance to get the data
var result = broker.AskFor<ClassToSend>("Test", "hello").Data;
```

See the `TestMessageBrokerWithDi` unit tests for more information.

## Current features

- request/reply for the same class
- request/reply for differnet classes by using json serialize/deserialize to map the get type to the ask type.
- Add register interface and create an instance (via DI) of the Getter when `AskFor` method is called.

## Possible improvments

- Add Async support
