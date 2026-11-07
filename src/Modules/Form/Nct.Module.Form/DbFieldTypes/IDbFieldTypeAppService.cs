using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Application;
using Nct.Module.Form.DbFieldTypes.Dtos;

namespace Nct.Module.Form.DbFieldTypes
{    

    public interface IDbFieldTypeAppService: ICrudAppService<DbFieldType,DbFieldTypeDto, QueryDto,CreateDbFieldTypeDto>
    {


    }
}
