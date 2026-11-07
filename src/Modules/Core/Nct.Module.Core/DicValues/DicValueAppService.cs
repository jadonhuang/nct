using Nct.Authorization;
using Nct.Module.Core.Dictionarys;
using Nct.Module.Core.Dictionarys.Dtos;
using Nct.Module.Core.DicValues.Dtos;

namespace Nct.Module.Core.DicValues
{


    public class DicValueAppService : CrudAppService<DicValue, DicValueDto, QueryDto, CreateDicValueDto>, IDicValueAppService
    {

        private const string CacheKeyPrefix = "dicvalue_";
        private readonly IDictionaryAppService _dicAppService;
        public DicValueAppService(IRepository<DicValue> repository, IDictionaryAppService dicAppService) : base(repository)
        {

            _dicAppService = dicAppService;
        }

      
        [AuthorizeEx("adddic,editdic,adddicvalue,editdicvalue")]
        public async Task<RetResult> MultiCreate(MultiCreateDicValueDto dto)
        {


            if (dto.DicId == 0)
            {
                if (dto.Name.IsEmpty()) throw new BusinessException("名称不能为空");

                var dic = await _dicAppService.GetByCode("dic");
                var dictionaryDto = new CreateDictionaryDto
                {
                    Name = dto.Name,
                    Code = CommonHelper.GetRandom(8),
                    ParentId = dic.Id
                };
                var result = await _dicAppService.Create(dictionaryDto);
                dto.DicId = result.Id;

            }
            else
            {
                if (dto.Name.IsNotEmpty())
                {
                    var dic = await _dicAppService.Get(dto.DicId);
                    var dictionaryDto = dic.MapTo<CreateDictionaryDto>();
                    dictionaryDto.Name = dto.Name;
                    await _dicAppService.Update(dictionaryDto);

                }
            }

            var dicModel = await _dicAppService.Get(dto.DicId);
            if (!dto.Values.Any())
            {
                await Repository.DeleteAsync(m => m.DicId == dto.DicId);
                Cache.Remove(CacheKeyPrefix + dto.DicId);
                return Success(dicModel);
            }

            var index = 1;

            foreach (var item in dto.Values)
            {
                item.Sort = index++;
            }

            var values = Repository.GetAllList(m => m.DicId == dto.DicId);

            var updateList = dto.Values.Where(m => m.Id > 0);

            var deleteList = values.Where(m => !updateList.Any(n => n.Id == m.Id));

            foreach (var item in deleteList)
            {
                await Repository.DeleteAsync(m => m.Id == item.Id);

            }

            foreach (var item in updateList)
            {

                var entity = item.MapTo<DicValue>();
                entity.DicId = dto.DicId;
                await Repository.UpdateAsync(entity);

            }

            var insertList = dto.Values.Where(m => m.Id == 0);
            foreach (var item in insertList)
            {

                var entity = item.MapTo<DicValue>();
                entity.DicId = dto.DicId;
                await Repository.InsertAsync(entity);

            }

            Cache.Remove(CacheKeyPrefix + dto.DicId);

            return Success(dicModel);

        }

        public async Task<List<DicValue>> GetValuesByCode(string code)
        {

            var dic = await _dicAppService.GetByCode(code);

            var key = CacheKeyPrefix + dic.Id;

            return Cache.Get(key, () =>
             {

                 var values = Repository.GetAllList(m => m.DicId == dic.Id);
                 return values;

             });


        }


        public async Task<DicNameAndValuesDto> GetNameAndValuesByCode(string code)
        {


            var dic = await _dicAppService.GetByCode(code);
            var dto = new DicNameAndValuesDto
            {
                DicId = dic.Id,
                Name = dic.Name,
                Values = await GetValuesByCode(code)
            };
            return dto;

        }





    }
}
