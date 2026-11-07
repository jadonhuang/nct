using Microsoft.AspNetCore.Http;
using Nct.Ioc;

namespace Nct.AspNetCore.WebClientInfo
{
    public class HttpContextWebClientInfoProvider : IWebClientInfoProvider, ITransientDependency
    {

        protected IHttpContextAccessor HttpContextAccessor { get; }

        public HttpContextWebClientInfoProvider(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public string BrowserInfo => GetBrowserInfo();

        public string ClientIpAddress => GetClientIpAddress();

        public HostString Host => GetHost();

        protected virtual HostString GetHost()
        {
            return HttpContextAccessor.HttpContext.Request.Host;
        }


        protected virtual string GetBrowserInfo()
        {

            return HttpContextAccessor.HttpContext?.Request.Headers.UserAgent;
        }

        protected virtual string GetClientIpAddress()
        {
            try
            {
                return HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.MapToIPv4().ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
