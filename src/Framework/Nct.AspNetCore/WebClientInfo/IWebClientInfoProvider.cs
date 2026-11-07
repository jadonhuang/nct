using Microsoft.AspNetCore.Http;

namespace Nct.AspNetCore.WebClientInfo
{
    public interface IWebClientInfoProvider
    {
        string BrowserInfo { get; }

        string ClientIpAddress { get; }

        HostString Host { get; }

    }
}
