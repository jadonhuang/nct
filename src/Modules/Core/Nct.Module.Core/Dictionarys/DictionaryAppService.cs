using Nct.Authorization;
using Nct.LinqQuery;
using Nct.Module.Core.Dictionarys.Dtos;
using System.Data;

namespace Nct.Module.Core.Dictionarys
{

    [RemoteService("Dic")]
    public class DictionaryAppService : CrudAppService<Dictionary, DictionaryDto, Dictionary, CreateDictionaryDto>, IDictionaryAppService
    {
        private const string CacheKey = "diclist";
        public DictionaryAppService(IRepository<Dictionary> repository) : base(repository)
        {


        }

        protected override void SetPermissionNames()
        {
            CreatePermissionName = "adddic";
            DeletePermissionName = "deldic";
            UpdatePermissionName = "editdic";

        }



        public override async Task<DictionaryDto> Create(CreateDictionaryDto input)
        {

            if (!input.Code.IsEmpty())
            {
                var dic = await Repository.FirstOrDefaultAsync(m => m.Code == input.Code);
                if (dic != null)
                    throw new BusinessException($"编码[{input.Code}]已存在");
            }


            var dto = await base.Create(input);

            var depth = 0;
            var path = dto.Id.ToString();
            if (input.ParentId > 0)
            {
                path = (await Get(dto.ParentId)).Path;
                path = path.Trim(',') + "," + dto.Id;
                depth = path.SplitString().Length - 1;
            }

            path = $",{path},";

            await Repository.GetAll().Where(m => m.Id == dto.Id).UpdateAsync(m => new Dictionary { Depth = depth, Path = path });

            RemoveCache(dto.Code);

            return dto;

        }


        private void RemoveCache(string code)
        {
            Cache.Remove(CacheKey);

            var key = CacheKey + "_" + code;

            Cache.Remove(key);

        }



        public override async Task<DictionaryDto> Update(CreateDictionaryDto input)
        {
            if (!input.Code.IsEmpty())
            {
                var dic = await Repository.FirstOrDefaultAsync(m => m.Code == input.Code && m.Id != input.Id);
                if (dic != null)
                    throw new BusinessException($"编码[{input.Code}]已存在");
            }
            var dto = await base.Update(input);
            RemoveCache(dto.Code);
            return dto;
        }

        public override async Task<int> Delete(int id)
        {

            int num = await Repository.GetAll().Where(m => m.Path.Contains("," + id + ",")).DeleteAsync();
            Cache.Remove(CacheKey);
            return num;

        }

        public override Task<DictionaryDto> GetDto(int id)
        {
            return Task.FromResult(GetAllList().FirstOrDefault(m => m.Id == id));
        }


        public List<DictionaryDto> GetList(int pid)
        {

            return GetAllList().Where(m => m.ParentId == pid).OrderBy(m => m.Sort).ToList();

        }

        public List<DictionaryDto> GetAllList()
        {
            return Cache.Get(CacheKey, () =>
            {

                return Repository.GetAll().OrderBy(m => m.Sort).ToList().MapTo<DictionaryDto>();


            });
        }



        public IList<DictionaryDto> GetTree(Dictionary dic)
        {

            var allList = GetAllList();
            var pid = dic.ParentId;
            var items = allList;
            var hasCdt = false;
            if (dic.Code.IsNotEmpty())
            {
                items = items.Where(m => m.Code.IgnoreCaseContains(dic.Code)).ToList();
                hasCdt = true;
            }
            if (dic.Name.IsNotEmpty())
            {
                items = items.Where(m => m.Name.IgnoreCaseContains(dic.Name)).ToList();
                hasCdt = true;
            }
            if (hasCdt && items.Count>0)
            {
                var parents = allList.Where(m => items.Any(n => n.Id != m.Id && n.Path.Contains($",{m.Id},")));
                items.AddRange(parents);

            }
            if (dic.ParentId > 0)
            {
                items = items.Where(m => m.Id != pid && m.Path.Contains($",{pid},")).ToList();
            }

            return items.ToTree<int, DictionaryDto>();



        }


        public IList<DictionaryDto> GetTreeByParentCode(string parentCode)
        {

            var childItems = GetAllList(parentCode);

            return childItems.ToTree<int, DictionaryDto>();

        }

        public List<DictionaryDto> GetAllList(int pid)
        {

            return GetAllList().Where(m => m.Path.Contains("," + pid.ToString() + ",")).ToList();
        }

        public Task<int> GetCount()
        {
            return Repository.CountAsync();
        }

        public Task<Dictionary> GetByCode(string code)
        {

            var key = CacheKey + "_" + code;

            var entity = Cache.Get(key, () =>
             {
                 return Repository.FirstOrDefault(m => m.Code == code);
             });

            return Task.FromResult(entity);
        }


        public async Task<int> GetChildCount(string parenCode)
        {
            var parent = await Repository.FirstOrDefaultAsync(m => m.Code == parenCode);
            return await Repository.GetAll().Where(m => m.ParentId == parent.Id).CountAsync();

        }

        public List<DictionaryDto> GetAllList(string parentCode)
        {
            var allList = GetAllList();
            var parent = allList.FirstOrDefault(m => m.Code.IgnoreCaseEqual(parentCode));
            if (parent == null) return new List<DictionaryDto>();
            return allList.Where(m => m.Path.Contains("," + parent.Id + ",") && m.Id != parent?.Id).ToList();
        }
    }
}
