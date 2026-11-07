using Microsoft.Extensions.Localization;
using Nct.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Localization
{
    public class DefaultStringLocalizerFactory : IStringLocalizerFactory
    {

        private readonly ICache _cache;
        public DefaultStringLocalizerFactory(ICache cache)
        {
            _cache = cache;

        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new DefaultStringLocalizer(_cache);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new DefaultStringLocalizer(_cache);
        }


    }
}
