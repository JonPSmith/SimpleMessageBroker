// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using NetCore.AutoRegisterDi;

namespace SimpleMessageBus
{
    /// <summary>
    /// This service must be registered as a singleton
    /// </summary>
    [RegisterAsSingleton]
    public class MessageBus
    {
        private readonly Dictionary<Type, Func<string, object>> _providers = new Dictionary<Type, Func<string, object>>();

        /// <summary>
        /// This registers a function that can be called by the AskFor
        /// </summary>
        /// <typeparam name="T">The type </typeparam>
        /// <param name="getDataFunc"></param>
        public void RegisterProvider<T>(Func<string, T> getDataFunc) where T : class
        {
            var providerTypeName = typeof(T).FullName;
            if (_providers.ContainsKey(typeof(T)))
                throw new ArgumentException(
                    $"A provider for type {typeof(T).Name} has already be registered .");

            var providerAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
            _providers[typeof(T)] = getDataFunc;
        }

        public T AskFor<T>(string dataString = null) where T : class
        {
            if (!_providers.ContainsKey(typeof(T)))
                throw new ArgumentException(
                    $"There is no provider registered for the type {typeof(T).Name}.");

            return (T)_providers[typeof(T)](dataString);
        }
    }
}