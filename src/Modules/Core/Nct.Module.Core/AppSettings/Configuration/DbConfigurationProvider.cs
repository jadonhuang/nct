using Microsoft.Extensions.Configuration;

namespace Nct.Module.Core.AppSettings
{
    public class DbConfigurationProvider : ConfigurationProvider
    {


        public override void Load()
        {
            
            if (IocManager.Instance.IsRegistered<IAppSettingAppService>())
            {

                IocManager.Instance.Using<IAppSettingAppService>(app =>
                {
                    Data = app.GetAllList().Result.ToDictionary(c => c.Id, c => c.Value);
                }
                );


            }
          
        }


    }
}
