// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using NetCore.AutoRegisterDi;

namespace SimpleMessageBroker
{
    /// <summary>
    /// This is a super-simple message broker that lets one side ask for a certain class
    /// and the registered provider will return it.
    /// </summary>
    [RegisterAsSingleton]
    public class MessageBroker : IMessageBroker
    {
        private readonly Dictionary<Type, Func<string, object>> _providers = new Dictionary<Type, Func<string, object>>();

        /// <summary>
        /// This registers a function that can be called by the AskFor
        /// </summary>
        /// <typeparam name="T">The type </typeparam>
        /// <param name="getDataFunc"></param>
        public void RegisterProvider<T>(Func<string, T> getDataFunc) where T : class
        {
            if (_providers.ContainsKey(typeof(T)))
                throw new ArgumentException(
                    $"A provider for type {typeof(T).Name} has already be registered .");

            _providers[typeof(T)] = getDataFunc;
        }

        /// <summary>
        /// This asks for an item of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataString">optional: will be sent to the provider</param>
        /// <returns>Type with data. Simple error response is to return null</returns>
        public T AskFor<T>(string dataString = null) where T : class
        {
            if (!_providers.ContainsKey(typeof(T)))
                throw new ArgumentException(
                    $"There is no provider registered for the type {typeof(T).Name}.");

            return (T)_providers[typeof(T)](dataString);
        }
    }
}