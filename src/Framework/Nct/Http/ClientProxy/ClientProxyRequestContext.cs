using Nct.Http.ApiDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Http.ClientProxy
{
    public class ClientProxyRequestContext
    {
        public ActionApiDescription ActionDescription { get; set; }

        public string BaseUrl { get; set; }

        public Dictionary<string,object> Arguments { get; set; }


        public Type ResultType { get; set; }


    }
}
