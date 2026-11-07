using System;
using System.Collections.Generic;
using System.Text;

namespace Nct.Cache.Redis
{
    public class RedisConfig
    {
        public string PrefixKey { get; set; }

        public int DBIndex { get; set; } = -1;

        public string ConnectionString { get; set; }
    }
}
