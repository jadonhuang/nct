using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nct.Http.ContentRequest
{
    public interface IContentRequest
    {


        Task<HttpContent> RequestAsync(string url, HttpMethod method, HttpContent content, IDictionary<string, string> headers = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

        Task<T> PostAsync<T>(string url, object data, bool isFormData = false, IDictionary<string, string> headers = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

        Task<T> GetAsync<T>(string url, IDictionary<string, string> headers = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    }
}
