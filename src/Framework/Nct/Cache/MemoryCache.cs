using MsM = Microsoft.Extensions.Caching.Memory;
using Nct.Ioc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;

namespace Nct.Cache
{
    public class MemoryCache : ICache, ISingletonDependency
    {

        private readonly IMemoryCache _cache;

        public MemoryCache(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }


        public void Remove(string key)
        {
            _cache.Remove(key);

        }

        public void Clear()
        {

            MsM.MemoryCache memoryCache = _cache as MsM.MemoryCache;
            memoryCache.Clear();

        }


        private List<string> GetAllKeys()
        {

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

            var coherentState = _cache.GetType().GetField("_coherentState", flags).GetValue(_cache);
            var coherentType= coherentState.GetType();
            var entryFieldInfo = coherentType.GetField("_entries", flags) ?? coherentType.GetField("_stringEntries", flags);
            var entries = entryFieldInfo.GetValue(coherentState);
            var keys = new List<string>();
            if (entries is IDictionary cacheDic)
            {
                foreach (DictionaryEntry entry in cacheDic)
                {
                    keys.Add(entry.Key.ToString());
                }
            }
            return keys;

        }

        public void Clear(string prefix)
        {

            var keys = GetAllKeys();

            foreach (var key in keys)
            {
                if (key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    Remove(key);

                }
            }

        }


        public void Set(string key, object data)
        {
            _cache.Set(key, data);
        }

        public void Set(string key, object data, int expirationMinutes)
        {
            _cache.Set(key, data, TimeSpan.FromMinutes(expirationMinutes));

        }

        public void Set(string key, object data, DateTime absoluteExpiration)
        {
            _cache.Set(key, data, absoluteExpiration);

        }

        public T Get<T>(string key, Func<T> handler)
        {
            return GetValue(key, handler, 0);
        }

        public T Get<T>(string key, Func<T> handler, int expirationMinutes)
        {
            return GetValue(key, handler, expirationMinutes);

        }

        private T GetValue<T>(string key, Func<T> handler, int expirationMinutes)
        {

            if (!_cache.TryGetValue(key, out T cacheEntry))
            {
                cacheEntry = handler.Invoke();
                if (expirationMinutes > 0)
                {
                    Set(key, cacheEntry, expirationMinutes);
                }
                else
                {
                    Set(key, cacheEntry);
                }

            }
            return cacheEntry;

        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }
    }
}
