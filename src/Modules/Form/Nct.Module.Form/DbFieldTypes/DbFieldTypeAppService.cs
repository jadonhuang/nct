using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Nct.Domain.Repositories;
using Nct.Module.Form.DbFieldTypes.Dtos;
using Nct.Extentions;
using Nct.Domain.Entities;
using Nct.ObjectMapper;
using Nct.Json;
using Nct.Application;
using Nct.LinqQuery;
using Nct.Module.Form.DbTables;
using Nct.Module.Form.DataSources.Dtos;
using Nct.Authorization;

namespace Nct.Module.Form.DbFieldTypes
{


    public class DbFieldTypeAppService : CrudAppService<DbFieldType, DbFieldTypeDto, QueryDto, CreateDbFieldTypeDto>, IDbFieldTypeAppService
    {
        private readonly IRepository<DbField> _dbFieldRep;
        public DbFieldTypeAppService(IRepository<DbFieldType> repository, IRepository<DbField> dbFieldRep) : base(repository)
        {
            _dbFieldRep = dbFieldRep;

        }

        protected override void SetPermissionNames()
        {
            CreatePermissionName = "addfieldtype";
            DeletePermissionName = "delfieldtype";
            UpdatePermissionName = "editfieldtype";

        }


        private void CheckNameExist(CreateDbFieldTypeDto dto)
        {
            var model = Repository.GetAll().Where(m => m.Name == dto.Name && m.Id != dto.Id).FirstOrDefault();
            if (model != null)
                throw new BusinessException("自定义类型已存在");


        }

        public override Task<DbFieldTypeDto> Create(CreateDbFieldTypeDto dto)
        {
            CheckNameExist(dto);
            return base.Create(dto);
        }



        public override async Task<DbFieldTypeDto> Update(CreateDbFieldTypeDto dto)
        {
            var model = await Get(dto.Id);
            dto.Name = model.Name;
            return await base.Update(dto);
        }


        public override async Task<int> Delete(int id)
        {
            var model = await Get(id);
            var field = await _dbFieldRep.FirstOrDefaultAsync(x => x.FieldType == model.Name);
            if (field != null)
                throw new BusinessException("此数据类型已使用");

            return await base.Delete(id);
        }

    }
}
