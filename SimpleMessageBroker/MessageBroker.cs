// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;
using SimpleMessageBroker.Internal;

namespace SimpleMessageBroker
{
    /// <summary>
    /// This is a super-simple message broker that lets one side ask over a communication link
    /// and the registered provider will return data.
    /// </summary>
    [RegisterAsSingleton]
    public class MessageBroker : IMessageBroker
    {
        private readonly Dictionary<string, ProviderInfo> _providers = new Dictionary<string, ProviderInfo>();

        private readonly IServiceProvider _serviceProvider;

        public MessageBroker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// This created communication link and registers a function to provide data
        /// </summary>
        /// <typeparam name="TIn">The type </typeparam>
        /// <param name="commsName">The name of the communication link</param>
        /// <param name="getDataFunc">A function that will provide the data when asked</param>
        public void RegisterGetter<TIn>(string commsName, Func<string, TIn> getDataFunc) 
            where TIn : class
        {
            _providers[commsName] = new ProviderInfo(typeof(TIn), getDataFunc);
        }

        /// <summary>
        /// This allows you to register an interface/class that will be created via DI when an Ask is called
        /// </summary>
        /// <typeparam name="TIn">The type of the data that the getter will provide</typeparam>
        /// <typeparam name="TService">The interface/class to use to </typeparam>
        /// <param name="commsName"></param>
        public void RegisterGetterService<TIn,TService>(string commsName) 
            where TIn : class
            where TService : IGetterProvider
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException($"The IServiceProvider in the constructor is null.");
            _providers[commsName] = new ProviderInfo(typeof(TIn), null, typeof(TService));
        }

        /// <summary>
        /// This removes a provider for a communication link
        /// </summary>
        /// <param name="commsName">The name of the communication link</param>
        public void RemoveGetter(string commsName)
        {
            _providers.Remove(commsName);
        }

        /// <summary>
        /// This asks for an item of type T
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="commsName">The name of the communication link</param>
        /// <param name="dataString">optional: will be sent to the provider</param>
        /// <returns>Type with data. Simple error response is to return null</returns>
        public TOut AskFor<TOut>(string commsName, string dataString = null) where TOut : class
        {
            if (!_providers.ContainsKey(commsName))
                throw new ArgumentException(
                    $"There is no provider registered for {commsName}.");

            var info = _providers[commsName];

            object getterData;
            if (info.ServiceType != null)
            {
                if (_serviceProvider == null)
                    throw new InvalidOperationException($"The IServiceProvider in the constructor is null.");
                
                using var scope = _serviceProvider.CreateScope();
                var getter = (IGetterProvider) scope.ServiceProvider.GetService(info.ServiceType);
                if (getter == null)
                    throw new InvalidOperationException(
                        $"The {info.ServiceType.Name} wasn't registered with the DI provider.");
                getterData = getter.GetData(dataString);
            }
            else
            {
                getterData = info.GetDataFunc(dataString);
            }

            if (info.ProvidedType == typeof(TOut))
                return (TOut)getterData;

            //We turn the provider type into json and then deserialize to the required type
            var serialized = JsonSerializer.Serialize(
                //thanks to https://stackoverflow.com/questions/972636/casting-a-variable-using-a-type-variable for ChangeType
                Convert.ChangeType(getterData, info.ProvidedType));
            return JsonSerializer.Deserialize<TOut>(serialized);
        }
    }
}