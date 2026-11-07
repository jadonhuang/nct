using Nct.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Http.ClientProxy
{

    public class ClientProxyOptions
    {

        public ClientProxyConfigDictionary ClientProxy {  get; set; }

        public ClientProxyConfig GetConfig(string name)
        { 
          return ClientProxy.GetOrDefault(name);
        }

    }



    public class ClientProxyConfigDictionary : Dictionary<string, ClientProxyConfig>
    {
        public ClientProxyConfig Default
        {
            get
            {
                return this.GetOrDefault(ClientProxyAssemblySetting.DefaultName);
            }
        }
    }


    public class ClientProxyConfig : Dictionary<string, string>
    {
        public string BaseUrl
        {
            get => this.GetOrDefault(nameof(BaseUrl));
        }

    }

}
