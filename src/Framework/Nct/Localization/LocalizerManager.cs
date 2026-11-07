using Nct.Domain.Repositories;
using Nct.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Localization
{
    public class LocalizerManager
    {

        public static List<Culture> GetAllCultures()
        {
            List<Culture> cultures = null;
            IocManager.Instance.Using<IRepository<Culture, string>>(rep =>
             {
                 cultures = rep.GetAllList();

             });

            return cultures;

        }
    }
}
