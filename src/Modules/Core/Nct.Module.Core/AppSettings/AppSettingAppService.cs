using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Nct.OAuth.Weixin;
using System.Data;

namespace Nct.Module.Core.AppSettings
{


    public class AppSettingAppService : IAppSettingAppService
    {
        private readonly IRepository<AppSetting, string> _repository;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly IOptionsMonitor<WeixinAuthenticationOptions> _wxOptionMonitor;

        public AppSettingAppService(IRepository<AppSetting, string> repository, IOptionsMonitor<WeixinAuthenticationOptions> wxOptionMonitor, IConfiguration configuration)
        {

            _configurationRoot = (IConfigurationRoot)configuration;
            _wxOptionMonitor = wxOptionMonitor;
            _repository = repository;


        }

        public Task<List<AppSetting>> GetAllList()
        {
            return _repository.GetAllListAsync();
        }

        public Task<List<AppSetting>> GetListByModule(string module)
        {
            return _repository.GetAllListAsync(m => m.Module == module);
        }

        public async Task Save(Dictionary<string, string> settings)
        {

            if (settings != null)
            {
                foreach (var key in settings.Keys)
                {
                    await _repository.GetAll().Where(m => m.Id == key).UpdateAsync(m => new AppSetting { Value = settings[key] });

                }
            }
            _configurationRoot.Reload();
        }

        public async Task SaveWxSetting(Dictionary<string, string> settings)
        {

            var opts = _wxOptionMonitor.Get(WeixinAuthenticationDefaults.AuthenticationScheme);
            opts.ClientId = settings[Constants.Config.WxAppId];
            opts.ClientSecret = settings[Constants.Config.WxAppSecrect];
            await Save(settings);

        }
    }
}
