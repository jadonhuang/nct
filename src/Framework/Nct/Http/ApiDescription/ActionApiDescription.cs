using System.Collections.Generic;

namespace Nct.Http.ApiDescription
{
    public class ActionApiDescription
    {
        public string Name { get; }

        public string Url { get; }

        public bool IsAllowAnonymous { get; }

        public IList<ParameterApiDescription> Parameters { get; }

        public string Method { get; }

        public ActionApiDescription(string name, string url, string method, IList<ParameterApiDescription> parameters, bool isAllowAnonymous)
        {
            Name = name;
            Url = url;
            Method = method;
            Parameters = parameters;
            IsAllowAnonymous = isAllowAnonymous;
        }
    }
}
