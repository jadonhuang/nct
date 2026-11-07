using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Nct.Authorization;
using Nct.Extentions;
using Nct.Helpers;
using Nct.Http.ApiDescription;
using Nct.Ioc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nct.AspNetCore.ApiDescription
{
    public class ApiDescriptionModelProvider : IApiDescriptionModelProvider, ISingletonDependency
    {

        private ApplicationApiDescription applicationApiDescription;
        private readonly IApiDescriptionGroupCollectionProvider _descptProvider;

        public ApiDescriptionModelProvider(IApiDescriptionGroupCollectionProvider descptProvider)
        {
            _descptProvider = descptProvider;
        }

        public ApplicationApiDescription Create()
        {
            if (applicationApiDescription != null)
                return applicationApiDescription;

            applicationApiDescription = new ApplicationApiDescription();

            foreach (var group in _descptProvider.ApiDescriptionGroups.Items)
            {
                foreach (var apiDescpt in group.Items)
                {

                    ControllerActionDescriptor controllerActionDescriptor = apiDescpt.ActionDescriptor.AsControllerActionDescriptor();

                    var moduleName = GetModuleName(controllerActionDescriptor);
                    var module = applicationApiDescription.GetOrAddModule(moduleName);

                    var controllName = controllerActionDescriptor.ControllerName.ToCamelCase();
                    var interfaces = controllerActionDescriptor.ControllerTypeInfo.GetInterfaces().Where(m=>!m.IsGenericType).Select(m => m.FullName).ToArray();
                    var controller = module.GetOrAddController(controllName, interfaces);

                    var actionName = controllerActionDescriptor.ActionName.ToCamelCase();
                    var url = apiDescpt.RelativePath;
                    var parameters = GetParameters(controllerActionDescriptor);
                    var method = apiDescpt.HttpMethod;
                    var methodInfo = controllerActionDescriptor.MethodInfo;
                    var isAllowAnonymous = ReflectionHelper .GetAttributesOfMemberAndType(methodInfo, methodInfo.DeclaringType) .OfType<AnonymousAttribute>().Any();

                    var action = new ActionApiDescription(actionName, url, method, parameters, isAllowAnonymous);
                    controller.AddAction(action);


                }

            }

            return applicationApiDescription;


        }


        private static string GetModuleName(ControllerActionDescriptor descriptor)
        {
            var asmName = descriptor.ControllerTypeInfo.Assembly.GetName().Name;
            var index = asmName.LastIndexOf('.');
            if (index == -1) return asmName.ToCamelCase();

            return asmName.Substring(index + 1).ToCamelCase();

        }


        private static List<ParameterApiDescription> GetParameters(ControllerActionDescriptor descriptor)
        {

            var parameters = new List<ParameterApiDescription>();

            foreach (var p in descriptor.Parameters)
            {
                var isSimple = ReflectionHelper.IsPrimitiveExtendedIncludingNullable(p.ParameterType);
                var parameter = new ParameterApiDescription(p.Name, isSimple);
                parameters.Add(parameter);
            }
            return parameters;

        }


    }
}
