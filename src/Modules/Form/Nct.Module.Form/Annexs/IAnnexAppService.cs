using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Application;
using Nct.Module.Form.Annexs.Dtos;

namespace Nct.Module.Form.Annexs
{    

    public interface IAnnexAppService: ICrudAppService<Annex,AnnexDto, string,QueryDto,CreateAnnexDto, CreateAnnexDto>
    {

        Task<List<Annex>> UploadFiles(string fileId, bool isLocal = false);

        Task<List<Annex>> GetListByIds(string[] ids);

    }
}
