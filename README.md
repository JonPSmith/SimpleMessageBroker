# Simple ask/get Message Broker

For my series of articles on [modular monolith architecture](https://www.thereformedprogrammer.net/evolving-modular-monoliths-1-an-architecture-for-net/) I needed a message broker that could pass data between isolated sections of my modular monolith. I couldn't find a message broker that worked like a method call (most message broker use the publish/listen approach), so I build a super simple  ask/get message broker.

I haven't released this as a NuGet as I expect there is ask/get Message Broker that I haven't found yet. If you know of one please contact me via the [Contact page](https://www.thereformedprogrammer.net/contact/) in my web site.

MIT license.

## Example code

```c#
var broker = new MessageBroker();

//You register a communication link called "link-name"
//Which will receive a string from the asker and returns class 
broker.RegisterGetter("link-name", 
    fromAsker => new ClassToSend(fromAsker));

//To ask the getter on the same communication link name
//for a class, sending an optional string
var result = broker.AskFor<ClassToSend>("link-name", "hello");
```

Typically you would register as a singleton (that's important) with your DI provider. Then you should call `RegisterGetter` for all the getter methods you want to register.

## Current features

- ask/get for the same class
- ask/get for differnet classes by using sjon serialize/deserialize to map the get type to the ask type.

## Possible improvments

- Add Async support
- Add register interface and create an instance (via DI) of the Getter when called.