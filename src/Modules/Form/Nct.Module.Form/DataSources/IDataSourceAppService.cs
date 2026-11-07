using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Application;
using Nct.Models;
using Nct.Module.Form.DataSources.Dtos;

namespace Nct.Module.Form.DataSources
{    

    public interface IDataSourceAppService: ICrudAppService<DataSource,DataSourceDto, DataSourceDto, CreateDataSourceDto>
    {

        DataTable GetDataTable(DsDataQueryDto dto);

        PageTableResult GetDataTablePage(DsDataPageQueryDto dto);

        List<NameValue> GetColumns(string code);
    }
}
