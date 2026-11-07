using Nct.Authorization;
using Nct.Domain.Entities;
using Nct.Domain.Repositories;
using Nct.Extentions;
using Nct.Ioc;
using Nct.LinqQuery;
using Nct.ObjectMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Nct.Application
{



    public abstract class CrudAppService<TEntity, TEntityDto> :
      CrudAppService<TEntity, TEntityDto, QueryDto>
     where TEntity : class, IEntity<int>
     where TEntityDto : class, IEntityDto<int>


    {
        public CrudAppService(IRepository<TEntity, int> repository) : base(repository)
        {


        }

    }





    public abstract class CrudAppService<TEntity, TEntityDto, TQueryDto> :
       CrudAppService<TEntity, TEntityDto, TQueryDto, TEntityDto>
    where TEntity : class, IEntity<int>
      where TEntityDto : class, IEntityDto<int>


    {
        public CrudAppService(IRepository<TEntity, int> repository) : base(repository)
        {


        }

    }


    public abstract class CrudAppService<TEntity, TEntityDto, TQueryDto, TCreateOrUpdateDto> :
    CrudAppService<TEntity, TEntityDto, int, TQueryDto, TCreateOrUpdateDto, TCreateOrUpdateDto>
      where TEntity : class, IEntity<int>
        where TEntityDto : class, IEntityDto<int>
        where TCreateOrUpdateDto : class, IEntityDto<int>

    {
        public CrudAppService(IRepository<TEntity, int> repository) : base(repository)
        {


        }

    }


    public abstract class CrudAppService<TEntity, TEntityDto, TPrimaryKey, TQueryDto, TCreateDto, TUpdateDto> :
        ApplicationService,
        ICrudAppService<TEntity, TEntityDto, TPrimaryKey, TQueryDto, TCreateDto, TUpdateDto>
         where TEntity : class, IEntity<TPrimaryKey>
          where TEntityDto : class, IEntityDto<TPrimaryKey>
           where TCreateDto : class, IEntityDto<TPrimaryKey>
           where TUpdateDto : class, IEntityDto<TPrimaryKey>
    {




        public CrudAppService(IRepository<TEntity, TPrimaryKey> repository)
        {

            Repository = repository;
            SetPermissionNames();
        }

        protected readonly IRepository<TEntity, TPrimaryKey> Repository;

        protected virtual string GetPermissionName { get; set; }

        protected virtual string GetAllPermissionName { get; set; }

        protected virtual string CreatePermissionName { get; set; }

        protected virtual string UpdatePermissionName { get; set; }

        protected virtual string DeletePermissionName { get; set; }



        protected virtual void  SetPermissionNames()
        {
           
        }

        protected virtual void CheckPermission(string permissionName)
        {

            if (permissionName.IsEmpty()) return;

            IocManager.Instance.Using<IPermissionChecker>(checker =>
            {
             
                bool valid = checker.Authorize(permissionName.SplitString());

                if (!valid)
                    throw new AuthorizationException($"无权限({permissionName})");

            });


        }

        protected virtual TEntityDto MapToEntityDto(TEntity entity)
        {
            return entity.MapTo<TEntityDto>();
        }


        protected virtual TEntity MapToEntity(TCreateDto dto)
        {
            return dto.MapTo<TEntity>();
        }


        protected virtual void MapToEntity(TUpdateDto dto, TEntity entity)
        {

            entity.MapFrom(dto);

        }


        protected virtual IQueryable<TEntity> CreateQuery(TQueryDto dto)
        {
            Check.NotNull(dto, nameof(dto));
            return Repository.GetAll();
        }




        public virtual async Task<TEntityDto> Create(TCreateDto dto)
        {

            CheckPermission(CreatePermissionName);

            var entity = MapToEntity(dto);

            await Repository.InsertAsync(entity);

            return MapToEntityDto(entity);

        }

        public virtual async Task<int> Delete(TPrimaryKey id)
        {
            CheckPermission(DeletePermissionName);
            int rows = await Repository.DeleteAsync(m => m.Id.Equals(id));
            return rows;
        }


        public virtual async Task<int> MultiDelete(TPrimaryKey[] ids)
        {
            CheckPermission(DeletePermissionName);
            int rows = await Repository.DeleteAsync(m => ids.Contains(m.Id));
            return rows;
        }


        public virtual async Task<TEntity> Get(TPrimaryKey id)
        {
            CheckPermission(GetPermissionName);
            var entity = await Repository.GetAsync(id);
            return entity;
        }


        public virtual async Task<PageResult<TEntityDto>> GetList(PageQuery<TQueryDto> pageQuery)
        {

            CheckPermission(GetAllPermissionName);

            var query = CreateQuery(pageQuery.Query);
            long count = await query.LongCountAsync();

            query = query.PageBy(pageQuery);
            if (pageQuery.OrderBy.IsEmpty())
            {
                query = query.OrderByDescending(m => m.Id);
            }
            else
            {
                query = query.OrderBy(pageQuery.OrderBy);
            }

            var rows = await query.ToListAsync();

            var pageResult = new PageResult<TEntityDto>
            {
                Total = count,
                Rows = rows.MapTo<TEntityDto>(),
                PageIndex = pageQuery.PageIndex,
                PageSize = pageQuery.PageSize


            };
            return pageResult;

        }

        public virtual async Task<TEntityDto> GetDto(TPrimaryKey id)
        {
            CheckPermission(GetPermissionName);
            var entity = await Repository.GetAsync(id);
            return MapToEntityDto(entity);
        }

        public virtual async Task<TEntityDto> Update(TUpdateDto dto)
        {

            CheckPermission(UpdatePermissionName);

            var entity = await Get(dto.Id);

            MapToEntity(dto, entity);

            await Repository.UpdateAsync(entity);

            return MapToEntityDto(entity);

        }


    }
}
