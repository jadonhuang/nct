using Microsoft.Extensions.Options;
using Nct.Extentions;
using Nct.Json;
using StackExchange.Redis;
using System;

namespace Nct.Cache.Redis
{
    public class RedisCache : ICache
    {


        private readonly IDatabase _db = null;
        private readonly RedisConfig _config;
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;

        public RedisCache(IOptions<RedisOptions> options)
        {
            _config = options.Value.Redis;
            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(() =>
            {

                return ConnectionMultiplexer.Connect(_config.ConnectionString);

            });

            _db = _connectionMultiplexer.Value.GetDatabase(_config.DBIndex);

        }

        private string GetKey(string key)
        {
            if (_config.PrefixKey.IsEmpty())
                return key;
            return $"{_config.PrefixKey}_{key}";

        }

        public void Clear()
        {
            if (_config.PrefixKey.IsEmpty())
                _db.Execute("FLUSHDB");
            else
                Clear(string.Empty);

        }

        public void Clear(string prefix)
        {
            var server = _connectionMultiplexer.Value.GetServer(_connectionMultiplexer.Value.GetEndPoints()[0]);
            foreach (var key in server.Keys(pattern: $"{GetKey(prefix)}*"))
            {
                _db.KeyDelete(key);

            }
        }

        public T Get<T>(string key)
        {
            var newKey = GetKey(key);
            string jsonValue = _db.StringGet(newKey);
            if (jsonValue != null)
            {
                return jsonValue.ToObject<T>();
            }
            return default(T);
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

            var newKey = GetKey(key);
            string jsonValue = _db.StringGet(newKey);
            if (jsonValue != null)
            {
                return jsonValue.ToObject<T>();
            }

            else
            {
                var cacheEntry = handler.Invoke();
                if (expirationMinutes > 0)
                {
                    Set(newKey, cacheEntry, expirationMinutes);
                }
                else
                {
                    Set(newKey, cacheEntry);
                }
                return cacheEntry;
            }

        }

        public void Remove(string key)
        {
            _db.KeyDelete(GetKey(key));
        }

        public void Set(string key, object data)
        {
            var jsonValue = data.ToJson();
            _db.StringSet(GetKey(key), jsonValue);
        }

        public void Set(string key, object data, int expirationMinutes)
        {
            var jsonValue = data.ToJson();
            _db.StringSet(GetKey(key), jsonValue, TimeSpan.FromMinutes(expirationMinutes));
        }

        public void Set(string key, object data, DateTime absoluteExpiration)
        {
            var newKey = GetKey(key);
            var jsonValue = data.ToJson();
            _db.StringSet(newKey, jsonValue);
            _db.KeyExpire(newKey, absoluteExpiration);

        }
    }
}
