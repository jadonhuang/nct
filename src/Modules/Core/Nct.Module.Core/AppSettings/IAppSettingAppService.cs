using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Application;
using Nct.Ioc;


namespace Nct.Module.Core.AppSettings
{

    public interface IAppSettingAppService:ITransientDependency
    {

        Task<List<AppSetting>> GetListByModule(string module);

        Task<List<AppSetting>> GetAllList();

        Task SaveWxSetting (Dictionary<string, string> settings);

        Task Save(Dictionary<string, string> settings);

    }
}
