using Nct.AspNetCore.Configuration;
using Nct.Extentions;
using Nct.Ioc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nct.Application;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Nct.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Nct.AspNetCore.DynamicApi
{
    public class AppServiceConvention : IApplicationModelConvention
    {
        private readonly Lazy<AspNetCoreConfiguration> _configuration;



        private ControllerAssemblySetting GetControllerSettingOrNull(Type controllerType)
        {
            return _configuration.Value.ControllerAssemblySettings.GetOrNull(controllerType);
        }

        public AppServiceConvention(IServiceCollection services)
        {
            _configuration = new Lazy<AspNetCoreConfiguration>(() =>
            {
                return IocManager.Instance.Resolve<AspNetCoreConfiguration>();


            }, true);
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {

                var type = controller.ControllerType.AsType();

                if (!typeof(IApplicationService).IsAssignableFrom(type)) continue;

                var remoteServiceAttr = ReflectionHelper.GetSingleAttributeOrDefault<RemoteServiceAttribute>(type);
                if (remoteServiceAttr != null)
                {
                    if (!remoteServiceAttr.IsEnabled) continue;

                    if (remoteServiceAttr.Name != null)
                    {
                        controller.ControllerName = remoteServiceAttr.Name;
                    }
                }


                var config = GetControllerSettingOrNull(type);

                controller.ControllerName = controller.ControllerName.RemovePostFix(ApplicationService.Suffixs);

                ConfigureArea(controller, config);
                ConfigureSelector(controller, config);
                ConfigureParameters(controller);
            }
        }


        private static void ConfigureParameters(ControllerModel controller)
        {
            foreach (var action in controller.Actions)
            {
                foreach (var p in action.Parameters)
                {
                    if (p.BindingInfo != null)
                    {
                        continue;
                    }

                    if (!ReflectionHelper.IsPrimitiveExtendedIncludingNullable(p.ParameterInfo.ParameterType))
                    {
                        p.BindingInfo = BindingInfo.GetBindingInfo([new FromBodyAttribute()]);
                        break;

                    }
                }
            }
        }


        private bool IsServiceAction(ActionModel action)
        {
            var actionMethod = action.ActionMethod;

            if (actionMethod.DeclaringType == typeof(ApplicationService)) return false;

            var attr = ReflectionHelper.GetSingleAttributeOrDefault<RemoteServiceAttribute>(actionMethod);

            if (attr != null && !attr.IsEnabled) return false;

            if (attr != null && attr.Name != null)
            {
                action.ActionName = attr.Name;
            }

            return true;

        }

        private void ConfigureSelector(ControllerModel controller, ControllerAssemblySetting configuration)
        {
            var hasControllerRouteModel = controller.Selectors.Any(selector => selector.AttributeRouteModel != null);

            var actions = controller.Actions.Where(IsServiceAction).ToList();

            controller.Actions.Clear();

            foreach (var action in actions)
            {
                controller.Actions.Add(action);
            }

            var moduleName = configuration?.ModuleName;

            if (!moduleName.IsEmpty()) { moduleName += "/"; }

            foreach (var action in controller.Actions)
            {
                action.ApiExplorer.IsVisible ??= true;
                var selectors = action.Selectors;

                if (!selectors.Any())
                {
                    selectors.Add(new SelectorModel());
                }

                foreach (var selector in action.Selectors)
                {
                    if (!hasControllerRouteModel && selector.AttributeRouteModel == null)
                    {
                        selector.AttributeRouteModel = new AttributeRouteModel(
                              new RouteAttribute(
                                  $"api/{moduleName}{controller.ControllerName}/{action.ActionName}"
                              )
                         );
                    }

                    if (!selector.ActionConstraints.Any())
                    {
                        selector.ActionConstraints.Add(new HttpMethodActionConstraint(GetVerbsForMethodName(action)));

                    }

                }

            }
        }


        private static List<string> GetVerbsForMethodName(ActionModel actionModel)
        {
            var verbs = new List<string>();

            if (ReflectionHelper.GetSingleAttributeOrDefault<GetAttribute>(actionModel.ActionMethod) != null)
                verbs.Add("GET");

            if (ReflectionHelper.GetSingleAttributeOrDefault<PostAttribute>(actionModel.ActionMethod) != null)
                verbs.Add("POST");

            if (verbs.Count == 0)
            {
                bool isGetAction = actionModel.ActionName.StartsWith("Get", StringComparison.OrdinalIgnoreCase);
                bool hasComplexParams = actionModel.Parameters.Any(m =>
                    m.BindingInfo == null &&
                    !ReflectionHelper.IsPrimitiveExtendedIncludingNullable(m.ParameterInfo.ParameterType));

                verbs.Add(isGetAction ? (hasComplexParams ? "POST" : "GET") : "POST");
            }

            return verbs;

        }

        private static void ConfigureArea(ControllerModel controller, [MaybeNull] ControllerAssemblySetting configuration)
        {
            if (controller.RouteValues.ContainsKey("area") || (configuration?.ModuleName).IsEmpty())
            {
                return;
            }

            controller.RouteValues["area"] = configuration.ModuleName;
        }
    }
}
