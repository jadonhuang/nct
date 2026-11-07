using System;
using System.Collections.Generic;
using System.Text;

namespace Nct.Cache
{
    public interface ICache
    {
        T Get<T>(string key);
        void Remove(string key);
        void Clear();
        void Clear(string prefix);
        void Set(string key, object data);
        void Set(string key, object data, int expirationMinutes);
        void Set(string key, object data, DateTime absoluteExpiration);

        T Get<T>(string key, Func<T> handler);
        T Get<T>(string key, Func<T> handler, int expirationMinutes);

    

    }
}
