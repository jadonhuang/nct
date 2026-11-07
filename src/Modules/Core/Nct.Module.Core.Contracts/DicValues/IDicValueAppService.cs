using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Application;
using Nct.Module.Core.DicValues.Dtos;

namespace Nct.Module.Core.DicValues
{

    public interface IDicValueAppService : ICrudAppService<DicValue, DicValueDto, QueryDto, CreateDicValueDto>
    {

        Task<RetResult> MultiCreate(MultiCreateDicValueDto dto);

        Task<List<DicValue>> GetValuesByCode(string code);

        Task<DicNameAndValuesDto> GetNameAndValuesByCode(string code);

    }
}
