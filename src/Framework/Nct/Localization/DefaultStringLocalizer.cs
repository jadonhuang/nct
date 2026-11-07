using Microsoft.Extensions.Localization;
using Nct.Cache;
using Nct.Domain.Repositories;
using Nct.Ioc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Localization
{
    public class DefaultStringLocalizer : IStringLocalizer
    {
        private readonly ICache _cache;
        public const string CachePrefix = "culture_";
        public DefaultStringLocalizer(ICache cache)
        {
            _cache = cache;


        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var str = GetString(name);
                var value = string.Format(str ?? name, arguments);
                return new LocalizedString(name, value, str == null);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var culture = CultureInfo.CurrentUICulture.Name;
            var resources = LoadResources(culture);

            return resources.Select(r => new LocalizedString(r.Name, r.Value, true));
        }

        private string GetString(string name)
        {
            var culture = CultureInfo.CurrentUICulture.Name;
            var resources = LoadResources(culture);
            var value = resources.SingleOrDefault(r => r.Name == name)?.Value;
            return value;
        }


        private IList<CultureResource> LoadResources(string culture)
        {

            return _cache.Get(CachePrefix + culture, () =>
              {

                  List<CultureResource> cultures = null;

                  IocManager.Instance.Using<IRepository<CultureResource>>(rep =>
                  {

                      cultures = rep.GetAllList(m => m.CultureId == culture);

                  });

                  return cultures;

              });



        }

    }
}
