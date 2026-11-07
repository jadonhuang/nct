using Nct.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Application
{




    public interface ICrudAppService<TEntity, TEntityDto> :
    ICrudAppService<TEntity, TEntityDto, QueryDto, TEntityDto>
         where TEntity : class, IEntity<int>
         where TEntityDto : class, IEntityDto<int>

    {

    }


    public interface ICrudAppService<TEntity, TEntityDto, TQueryDto> :
     ICrudAppService<TEntity, TEntityDto, TQueryDto, TEntityDto>
          where TEntity : class, IEntity<int>
          where TEntityDto : class, IEntityDto<int>

    {

    }



    public interface ICrudAppService<TEntity, TEntityDto, TQueryDto, TCreateOrUpdateDto> :
      ICrudAppService<TEntity, TEntityDto, int, TQueryDto, TCreateOrUpdateDto, TCreateOrUpdateDto>
        where TEntity : class, IEntity<int>
          where TEntityDto : class, IEntityDto<int>
          where TCreateOrUpdateDto : class, IEntityDto<int>

    {

    }

    public interface ICrudAppService<TEntity, TEntityDto, TPrimaryKey, TQueryDto, TCreateDto, TUpdateDto>:IApplicationService where TEntity : class, IEntity<TPrimaryKey>
           where TEntityDto : class, IEntityDto<TPrimaryKey>
            where TCreateDto : class, IEntityDto<TPrimaryKey>
            where TUpdateDto : class, IEntityDto<TPrimaryKey>
    {
        Task<TEntityDto> Create(TCreateDto input);
        Task<int> Delete(TPrimaryKey id);

        Task<int> MultiDelete(TPrimaryKey[] ids);

        Task<TEntity> Get(TPrimaryKey id);

        Task<TEntityDto> GetDto(TPrimaryKey id);

        Task<PageResult<TEntityDto>> GetList(PageQuery<TQueryDto> input);

        Task<TEntityDto> Update(TUpdateDto input);
    }
}
