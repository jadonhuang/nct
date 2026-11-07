using Microsoft.Extensions.Options;
using Nct.Cache.Redis;
using Nct.Ioc;
using System;

namespace Nct.Cache.Configuration
{
    public class CacheConfiguration
    {
        public void UseRedis(Action<RedisConfig> action)
        {

            var redisOptions = IocManager.Instance.Resolve<IOptions<RedisOptions>>().Value;
            redisOptions.Redis ??= new RedisConfig();
            action(redisOptions.Redis);

            UseRedis();

        }
        public void UseRedis()
        {
            IocManager.Instance.RegisterIfNot<ICache, RedisCache>();

        }

    }
}
