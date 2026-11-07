using Microsoft.AspNetCore.Http;
using Nct.Authorization;
using Nct.DataAccess.Json;
using System.IO;
using System.Text;

namespace Nct.ApiJson
{

    [Anonymous]
    public class JsonAppService : ApplicationService
    {

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IResultBuilder _resultBuilder;

        public JsonAppService(IHttpContextAccessor httpContextAccessor, IResultBuilder resultBuilder)
        {
            _contextAccessor = httpContextAccessor;
            _resultBuilder = resultBuilder;
        }


        private async Task<RetResult> GetResult(StatementType statement)
        {
            string json = string.Empty;
            using (var reader = new StreamReader(_contextAccessor.HttpContext.Request.Body, Encoding.UTF8))
            {
                json = await reader.ReadToEndAsync();
            }

            IDictionary<string, object> result;
            try
            {
                result = await _resultBuilder.Build(json, statement);
            }
            catch (JsonException ex)
            {
                return Error(ex.Message, 400);
            }
            return Success(result);
        }


        [Post]
        public Task<RetResult> Get()
        {
            return GetResult(StatementType.Select);

        }

        public Task<RetResult> Add()
        {
            return GetResult(StatementType.Insert);
        }

        public Task<RetResult> Edit()
        {
            return GetResult(StatementType.Update);
        }

        public Task<RetResult> Delete()
        {
            return GetResult(StatementType.Delete);
        }

        public Task<RetResult> Mix()
        {
            return GetResult(StatementType.None);
        }

    }
}
