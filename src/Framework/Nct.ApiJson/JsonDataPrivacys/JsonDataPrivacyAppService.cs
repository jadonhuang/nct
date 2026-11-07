using Nct.ApiJson.JsonDataPrivacys.Dtos;

namespace Nct.ApiJson.JsonDataPrivacys
{


    public class JsonDataPrivacyAppService:CrudAppService<JsonDataPrivacy, JsonDataPrivacyDto, QueryDto, CreateJsonDataPrivacyDto>, IJsonDataPrivacyAppService
    {
            
		public JsonDataPrivacyAppService(IRepository<JsonDataPrivacy> repository) : base(repository)
		{


		}

        public Task<JsonDataPrivacy> GetByTableName(string tableName)
        {

            var model = Cache.Get(tableName.ToLower(),  () =>
            {

                return  Repository.FirstOrDefault(m => m.TableName == tableName);

            });

            return Task.FromResult(model);
        }

        public override async Task<JsonDataPrivacyDto> Update(CreateJsonDataPrivacyDto input)
        {
            var result= await base.Update(input);
            Cache.Remove(input.TableName);
            return result;
        }


    }
}
