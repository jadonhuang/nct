using Nct.Authorization;
using Nct.Extentions;
using Nct.Http.ContentRequest;
using Nct.Ioc;
using Nct.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Nct.Http.ClientProxy
{
    public class CilentProxyBase : ITransientDependency
    {
        public const string ClientProxyClientName = "nctclient";
        private readonly IContentRequest _contentRequest;
        public CilentProxyBase(IContentRequest contentRequest)
        {
            _contentRequest = contentRequest;

        }

        public virtual async Task<T> RequestAsync<T>(ClientProxyRequestContext context)
        {

            T result;
            if (typeof(T) == typeof(object))
            {
                if (context.ResultType == typeof(Stream))
                {
                    var steam = await RequestContentAsync<Stream>(context);
                    return (T)(object)steam;
                }
                var stringContent = await RequestContentAsync<string>(context);
                result = (T)stringContent.ToObject(context.ResultType);
            }
            else
            {
                result = await RequestContentAsync<T>(context);

            }
            return result;
        }


        protected virtual async Task<T> RequestContentAsync<T>(ClientProxyRequestContext context)
        {

            var actionDescpt = context.ActionDescription;
            var url = context.BaseUrl.EnsureEndsWith('/') + BuildUrl(context);
            var header = BuildHeader(context);
            T result;
            if (actionDescpt.Method == "POST")
            {
                var body = GetBody(context);
                result = await _contentRequest.PostAsync<T>(url, body, false, header);
            }
            else
            {
                result = await _contentRequest.GetAsync<T>(url, header);
            }

            return result;
        }


        private static string BuildUrl(ClientProxyRequestContext context)
        {
            var url = context.ActionDescription.Url;

            var args = context.ActionDescription.Parameters.Where(m => m.IsSimpleType).ToArray();

            bool isFirst = true;

            foreach (var arg in args)
            {
                var value = context.Arguments.GetOrDefault(arg.Name);
                var strValue = value == null ? "" : value.ToString().UrlEncode();

                url += (isFirst ? "?" : "&") + arg + "=" + strValue;
                isFirst = false;
            }

            return url;

        }

        private static object GetBody(ClientProxyRequestContext context)
        {
            var args = context.ActionDescription.Parameters.Where(m => !m.IsSimpleType).ToArray();

            if (args.Length < 1)
                return null;

            if (args.Length > 1)
                throw new InvalidOperationException("only surport one complex argument");

            return context.Arguments[args[0].Name];

        }


        private static Dictionary<string, string> BuildHeader(ClientProxyRequestContext context)
        {
            var header = new Dictionary<string, string>();

            if (!context.ActionDescription.IsAllowAnonymous)
            {

                IocManager.Instance.Using<ITokenProvider>(provider =>
                {
                    var token = provider.GetToken();
                    if (token.IsNotEmpty())
                    {
                        header["Authorization"] = token;
                    }

                });

            }

            header["User-Agent"] = ClientProxyClientName;

            return header;
        }





    }
}
