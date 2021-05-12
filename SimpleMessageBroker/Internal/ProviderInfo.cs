// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace SimpleMessageBroker.Internal
{
    public class ProviderInfo
    {
        public ProviderInfo(Type providedType, Func<string, object> getDataFunc, Type serviceType = null)
        {
            ProvidedType = providedType;
            GetDataFunc = getDataFunc;
            ServiceType = serviceType;
        }

        public Type ProvidedType { get; }
        public Func<string, object> GetDataFunc { get; }
        public Type ServiceType { get; }
    }
}