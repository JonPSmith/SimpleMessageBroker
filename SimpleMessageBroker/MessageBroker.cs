// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json;
using NetCore.AutoRegisterDi;
using SimpleMessageBroker.Internal;

namespace SimpleMessageBroker
{
    /// <summary>
    /// This is a super-simple message broker that lets one side ask for a certain class
    /// and the registered provider will return it.
    /// </summary>
    [RegisterAsSingleton]
    public class MessageBroker : IMessageBroker
    {
        private readonly Dictionary<string, ProviderInfo> _providers = new Dictionary<string, ProviderInfo>();

        /// <summary>
        /// This registers a function that can be called by the AskFor
        /// </summary>
        /// <typeparam name="T">The type </typeparam>
        /// <param name="commsName">The name of the communication link</param>
        /// <param name="getDataFunc">A function that will provide the data when asked</param>
        public void RegisterProvider<T>(string commsName, Func<string, T> getDataFunc) where T : class
        {
            if (_providers.ContainsKey(commsName))
                throw new ArgumentException(
                    $"A provider for {commsName} has already be registered .");

            _providers[commsName] = new ProviderInfo(typeof(T), getDataFunc);
        }

        /// <summary>
        /// This asks for an item of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commsName">The name of the communication link</param>
        /// <param name="dataString">optional: will be sent to the provider</param>
        /// <returns>Type with data. Simple error response is to return null</returns>
        public T AskFor<T>(string commsName, string dataString = null) where T : class
        {
            if (!_providers.ContainsKey(commsName))
                throw new ArgumentException(
                    $"There is no provider registered for {commsName}.");

            var info = _providers[commsName];

            if (info.ProvidedType == typeof(T))
                return (T) info.GetDataFunc(dataString);

            //We turn the provider type into json and then deserialize to the required type
            var serialized = JsonSerializer.Serialize(
                Convert.ChangeType(info.GetDataFunc(dataString), info.ProvidedType));
            return JsonSerializer.Deserialize<T>(serialized);
        }
    }
}