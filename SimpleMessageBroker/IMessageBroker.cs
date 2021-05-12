// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace SimpleMessageBroker
{
    public interface IMessageBroker
    {
        /// <summary>
        /// This created communication link and registers a function to provide data
        /// </summary>
        /// <typeparam name="TIn">The type </typeparam>
        /// <param name="commsName">The name of the communication link</param>
        /// <param name="getDataFunc">A function that will provide the data when asked</param>
        void RegisterGetter<TIn>(string commsName, Func<string, TIn> getDataFunc) 
            where TIn : class;

        /// <summary>
        /// This allows you to register an interface/class that will be created via DI when an Ask is called
        /// </summary>
        /// <typeparam name="TIn">The type of the data that the getter will provide</typeparam>
        /// <typeparam name="TService">The interface/class to use to </typeparam>
        /// <param name="commsName"></param>
        void RegisterGetterService<TIn,TService>(string commsName) 
            where TIn : class
            where TService : IGetterProvider;

        /// <summary>
        /// This removes a provider for a communication link
        /// </summary>
        /// <param name="commsName">The name of the communication link</param>
        void RemoveGetter(string commsName);

        /// <summary>
        /// This asks for an item of type T
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="commsName">The name of the communication link</param>
        /// <param name="dataString">optional: will be sent to the provider</param>
        /// <returns>Type with data. Simple error response is to return null</returns>
        TOut AskFor<TOut>(string commsName, string dataString = null) where TOut : class;
    }
}