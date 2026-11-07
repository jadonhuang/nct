using Castle.DynamicProxy;
using Microsoft.Extensions.Options;
using Nct.Async;
using Nct.Extentions;
using Nct.Http.ApiDescription;
using Nct.Ioc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nct.Http.ClientProxy
{
    public class ClientProxyInterceptor<T> : InternalAsyncInterceptor
    {
        private readonly ApiDescriptionFinder _apiDescriptionFinder;
        private readonly ClientProxyOptions _clientProxyOptions;
        private readonly CilentProxyBase _clientProxyBase;
        public ClientProxyInterceptor(ApiDescriptionFinder apiDescriptionFinder, IOptions<ClientProxyOptions> options, CilentProxyBase clientProxyBase)
        {
            _apiDescriptionFinder = apiDescriptionFinder;
            _clientProxyOptions = options.Value;
            _clientProxyBase = clientProxyBase;
        }

        public override void InterceptSynchronous(IInvocation invocation)
        {
            var context = BuildContext(invocation);
            var result = _clientProxyBase.RequestAsync<object>(context).Result;
            invocation.ReturnValue = result;

        }

        protected override Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            return Task.CompletedTask;
        }

        protected override async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {

            var context = BuildContext(invocation);
            var result = await _clientProxyBase.RequestAsync<TResult>(context);
            return result;
        }


        private ActionApiDescription FindActionApiDescription(IInvocation invocation,string configName, string baseUrl)
        {
            var method = invocation.Method;
            var descpt = _apiDescriptionFinder.FindAction<T>(method.Name, configName, baseUrl, invocation.Arguments.Length);

            if (descpt == null)
            {
                throw new Exception($" {typeof(T).FullName}.{method.Name} api description not found");
            }

            return descpt;

        }

        private ClientProxyRequestContext BuildContext(IInvocation invocation)
        {


            var clientProxyConfig = IocManager.Instance.Resolve<ClientProxyAssebleySettings>().GetOrDefault(typeof(T).Assembly);
            var configName = clientProxyConfig.ConfigName;

            var baseUrl = _clientProxyOptions.GetConfig(configName)?.BaseUrl;

            if (baseUrl.IsEmpty())
            {
                throw new Exception($"ClientProxy {configName} base url is null");
            }


            var context = new ClientProxyRequestContext();
            context.ResultType = invocation.Method.ReturnType;
            var apiActionDescpt = FindActionApiDescription(invocation, configName, baseUrl);
            var args = new Dictionary<string, object>();
            var parameters = apiActionDescpt.Parameters;

            for (int i = 0; i < parameters.Count; i++)
            {
                var name = parameters[i].Name;
                args[name] = invocation.Arguments[i];
            }

            context.Arguments = args;
            context.ActionDescription = apiActionDescpt;
            context.BaseUrl = baseUrl;
            return context;

        }



    }
}
