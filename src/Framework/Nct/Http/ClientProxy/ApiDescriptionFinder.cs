using Nct.Extentions;
using Nct.Http.ApiDescription;
using Nct.Http.ContentRequest;
using Nct.Ioc;
using Nct.Json;
using System.Collections.Generic;
using System.Linq;

namespace Nct.Http.ClientProxy
{
    public class ApiDescriptionFinder : ISingletonDependency
    {


        private readonly Dictionary<string, ApplicationApiDescription> appApiDescriptions;
        private readonly IContentRequest _contentRequest;
        public ApiDescriptionFinder(IContentRequest contentRequest)
        {
            appApiDescriptions = [];
            _contentRequest = contentRequest;
        }

        public ApplicationApiDescription GetApplicationApiDescription(string baseUrl)
        {
            var url = baseUrl.EnsureEndsWith('/') + "apidescription";
            ApplicationApiDescription applicationApiDescription = null;
            IocManager.Instance.Using<IContentRequest>(req =>
            {
                var content = _contentRequest.GetAsync<string>(url).ConfigureAwait(false).GetAwaiter().GetResult();
                applicationApiDescription =content.ToObject<ApplicationApiDescription>();

            });
          
            return applicationApiDescription;

        }

        public ActionApiDescription FindAction<T>(string methodName, string configName, string baseUrl, int paramNum)
        {

            var fullName = typeof(T).FullName;
            var appApiDescription = appApiDescriptions.GetOrDefault(configName);
            if (appApiDescription == null)
            {
                appApiDescription = GetApplicationApiDescription(baseUrl);
                appApiDescriptions.TryAdd(configName, appApiDescription);
            }

            foreach (var module in appApiDescription.Modules.Values)
            {
                foreach (var controller in module.Controllers.Values)
                {
                    if (controller.Interfaces.Any(m => m == fullName))
                    {

                        return controller.Actions.Values.FirstOrDefault(m => m.Parameters.Count == paramNum && m.Name.IgnoreCaseEqual(methodName));
                    }

                }

            }

            return null;

        }

    }
}
