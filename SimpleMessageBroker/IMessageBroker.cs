// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace SimpleMessageBroker
{
    public interface IMessageBroker
    {
        /// <summary>
        /// This registers a function that can be called by the AskFor
        /// </summary>
        /// <typeparam name="T">The type </typeparam>
        /// <param name="commsName">The name of the communication link</param>
        /// <param name="getDataFunc">A function that will provide the data when asked</param>
        void RegisterProvider<T>(string commsName, Func<string, T> getDataFunc) where T : class;

        /// <summary>
        /// This asks for an item of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commsName">The name of the communication link</param>
        /// <param name="dataString">optional: will be sent to the provider</param>
        /// <returns>Type with data. Simple error response is to return null</returns>
        T AskFor<T>(string commsName, string dataString = null) where T : class;
    }
}