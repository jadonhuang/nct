using Nct.ApiJson.JsonDataPrivacys.Dtos;

namespace Nct.ApiJson.JsonDataPrivacys
{

    public interface IJsonDataPrivacyAppService: ICrudAppService<JsonDataPrivacy,JsonDataPrivacyDto, QueryDto,CreateJsonDataPrivacyDto>
    {

         Task<JsonDataPrivacy> GetByTableName(string tableName);

    }
}
