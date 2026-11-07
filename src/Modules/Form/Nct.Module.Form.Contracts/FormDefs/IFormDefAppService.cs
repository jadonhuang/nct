using Nct.Module.Form.FormDefs.Dtos;

namespace Nct.Module.Form.FormDefs
{

    public interface IFormDefAppService: ICrudAppService<FormDef,FormDefDto, FormDefDto, CreateFormDefDto>
    {

        Task<List<FormDefDto>> GetAllList();

        RetResult SetItemValue(SetItemValueDto dto);

        DesignInfo GetDesignInfoByCode(string formCode, out FormDef formDef);

        Task<RetResult> SaveData(SaveDataDto dto);

    }
}
