using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Application;
using Nct.Module.Form.DbTables.Dtos;

namespace Nct.Module.Form.DbTables
{    

    public interface IDbTableAppService: ICrudAppService<DbTable,DbTableDto, DbTableDto, CreateDbTableDto>
    {
        List<FieldInfo> GetCustomFields(string table);

        Task<DbTable> GetByName(string name);

        List<FieldInfo> GetDbFields(string table);

        Task<RetResult> Import(string table);

        Task CreateIfFieldNotExists(DbFieldDto fieldDto, string table);

    }
}
