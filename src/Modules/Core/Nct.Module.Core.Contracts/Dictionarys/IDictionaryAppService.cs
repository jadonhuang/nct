using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Application;
using Nct.Models;
using Nct.Module.Core.Dictionarys.Dtos;

namespace Nct.Module.Core.Dictionarys
{    

    public interface IDictionaryAppService: ICrudAppService<Dictionary,DictionaryDto, Dictionary, CreateDictionaryDto>
    {
       List<DictionaryDto> GetList(int pid);

        List<DictionaryDto> GetAllList(int pid);

        List<DictionaryDto> GetAllList(string parentCode);

        List<DictionaryDto> GetAllList();

        IList<DictionaryDto> GetTreeByParentCode(string parentCode);

        IList<DictionaryDto> GetTree(Dictionary dic);

        Task<Dictionary> GetByCode(string code);

        Task<int> GetCount();

        Task<int> GetChildCount(string parenCode);

    }
}
