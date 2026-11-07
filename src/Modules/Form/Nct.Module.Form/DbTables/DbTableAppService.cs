
using Nct.Authorization;
using Nct.LinqQuery;
using Nct.Module.Form.DbFieldTypes;
using Nct.Module.Form.DbTables.Dtos;
using Nct.ObjectMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Form.DbTables
{


    public class DbTableAppService : CrudAppService<DbTable, DbTableDto, DbTableDto, CreateDbTableDto>, IDbTableAppService
    {


        private readonly IRepository<DbField> _dbFieldRep;
        private readonly IRepository<DbFieldType> _dbFieldTypeRep;
        private const string DbFiledCacheKey = "field_{0}";
        public DbTableAppService(IRepository<DbTable> repository, IRepository<DbField> dbFieldRep,
            IRepository<DbFieldType> dbFieldTypeRep

            ) : base(repository)
        {

            _dbFieldRep = dbFieldRep;
            _dbFieldTypeRep = dbFieldTypeRep;


        }

        protected override void SetPermissionNames()
        {
            CreatePermissionName = "addtable";
            UpdatePermissionName = "edittable";

        }

        protected override IQueryable<DbTable> CreateQuery(DbTableDto dto)
        {

            return base.CreateQuery(dto)
              .WhereIf(m => m.Descpt.Contains(dto.Descpt), dto.Descpt.IsNotEmpty())
               .WhereIf(m => m.Name.Contains(dto.Name), dto.Name.IsNotEmpty());
        }


        private static void CheckDbTableExist(string name)
        {

            var sql = "SELECT o.name  FROM sysobjects o , sysindexes i WHERE o.id = i.id AND o.Xtype = 'U' AND i.indid < 2 and o.category!=2 and o.name=@name";

            var tableName = Db.ExecuteScalar<string>(sql, new { name });

            if (tableName != null)
            {

                throw new BusinessException("数据表已存在");

            }


        }


        private static string GetFieldTypeStr(DbFieldDto dto, List<DbFieldType> fieldTypes)
        {
            var fieldType = fieldTypes.FirstOrDefault(m => m.Name.IgnoreCaseEqual(dto.FieldType)) ?? throw new BusinessException($"错误的字段类型:{dto.FieldType}");
            var lenStr = "";
            if (fieldType.Length > 0)
            {
                lenStr += $"({fieldType.Length}";

            }
            else if (fieldType.Length == -1)
            {
                lenStr += $"({dto.Length}";

            }
            if (fieldType.Precision > 0)
            {
                lenStr += $",{fieldType.Precision}";

            }
            if (lenStr.IsNotEmpty())
            {
                lenStr += ")";
            }

            return $"{fieldType.DataType}{lenStr}";

        }


        public async Task CreateIfFieldNotExists(DbFieldDto fieldDto, string table)
        {



            if (_dbFieldRep.GetAll().Where(m => m.ColName == fieldDto.Name).FirstOrDefault() == null)
            {
                var tableModel = await GetByName(table);

                var dbField = new DbField
                {
                    ColName = fieldDto.Name,
                    TableId = tableModel.Id,
                    FieldType = fieldDto.FieldType,
                };
                await _dbFieldRep.InsertAsync(dbField);


                var fields = GetDbFields(table);
                if (!fields.Any(m => m.Name.IgnoreCaseEqual(fieldDto.Name)))
                {
                    string sql = GetSqlServerUpdateTableSql(table, [], [fieldDto]);
                    Db.Execute(sql);
                }


            }

        }

        private string GetSqlServerCreateTableSql(string tableName, List<DbFieldDto> fields)
        {

            var fieldTypes = _dbFieldTypeRep.GetAllList();
            var sql = new StringBuilder();
            sql.Append($"CREATE TABLE {tableName} (");
            foreach (var f in fields)
            {
                var fieldTypeStr = GetFieldTypeStr(f, fieldTypes);
                sql.AppendLine($"{f.Name} {fieldTypeStr}");

                if (f.IsKey)
                {
                    sql.Append(" PRIMARY KEY");
                }
                else if (!f.IsNullable)
                {
                    sql.Append(" NOT NULL");
                }
                sql.Append(',');
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(')');

            foreach (var f in fields)
            {
                sql.AppendLine($"execute sp_addextendedproperty N'MS_Description',N'{f.Remark.SafeSql()}',N'Schema',N'dbo',N'table',N'{tableName}',N'column',N'{f.Name}';");
            }

            return sql.ToString();
        }


        private string GetSqlServerUpdateTableSql(string tableName, List<FieldInfo> oldFields, List<DbFieldDto> newFields)
        {

            var fieldTypes = _dbFieldTypeRep.GetAllList();
            var sql = new StringBuilder();

            foreach (var f in newFields)
            {
                var oldField = oldFields.FirstOrDefault(m => m.Name.IgnoreCaseEqual(f.Name));
                if (oldField == null)
                {

                    var fieldTypeStr = GetFieldTypeStr(f, fieldTypes);
                    sql.AppendLine($"alter table {tableName} add  [{f.Name}] {fieldTypeStr};");
                    sql.AppendLine($"exec sp_addextendedproperty N'MS_Description',N'{f.Remark.SafeSql()}',N'Schema',N'dbo',N'table',N'{tableName}',N'column',N'{f.Name}';");

                }
                else
                {
                    if (!f.FieldType.IgnoreCaseEqual(oldField.FieldType) || f.Length != oldField.Length)
                    {
                        var fieldTypeStr = GetFieldTypeStr(f, fieldTypes);
                        sql.AppendLine($"alter table {tableName} alter column {f.Name} {fieldTypeStr};");

                    }
                    if (f.Remark != oldField.Remark)
                    {
                        sql.AppendLine($"if( exists (SELECT   ep.value  FROM   sys.extended_properties ep  INNER JOIN      sys.columns c ON ep.major_id = c.object_id  INNER JOIN     sys.objects o ON c.object_id = o.object_id  WHERE o.name = '{tableName}' AND c.name = '{f.Name}' AND ep.minor_id = c.column_id   AND ep.name = 'MS_Description'))");
                        sql.AppendLine($"exec sp_updateextendedproperty N'MS_Description',N'{f.Remark.SafeSql()}',N'Schema',N'dbo',N'table',N'{tableName}',N'column',N'{f.Name}';");
                        sql.AppendLine("else");
                        sql.AppendLine($"exec sp_addextendedproperty N'MS_Description',N'{f.Remark.SafeSql()}',N'Schema',N'dbo',N'table',N'{tableName}',N'column',N'{f.Name}';");

                    }
                }
            }


            foreach (var f in oldFields)
            {
                var newField = newFields.FirstOrDefault(m => m.Name.IgnoreCaseEqual(f.Name));
                if (newField == null)
                {
                    sql.AppendLine($"exec sp_rename  '{tableName}.{f.Name}' , '{f.Name}_{DateTime.Now.ToString("yyyyMMddHHmmss")}', 'COLUMN';");
                }

            }
            return sql.ToString();
        }





        public List<FieldInfo> GetDbFields(string table)
        {

            var cacheKey = DbFiledCacheKey.FormatEx(table.ToLower());

            return Cache.Get(cacheKey, () =>
             {

                 string sql = "SELECT  a.name , b.name datatype , [length] = COLUMNPROPERTY(a.id, a.name, 'PRECISION') , " +
                    "[isidentity] =COLUMNPROPERTY(a.id, a.name, 'IsIdentity'), " +
                    "[iskey] = CASE WHEN EXISTS ( SELECT 1 FROM sysobjects WHERE xtype = 'PK' AND parent_obj = a.id AND name IN ( SELECT name FROM sysindexes WHERE indid IN ( SELECT indid FROM sysindexkeys WHERE id = a.id AND colid = a.colid ) ) ) THEN 1 ELSE 0 END ," +
                    "[isnullable] =a.isnullable , [remark] =g.[value]     FROM syscolumns a LEFT JOIN systypes b ON a.xusertype = b.xusertype INNER JOIN sysobjects d ON a.id = d.id AND d.xtype = 'U' AND " +
                    "d.name <> 'dtproperties' LEFT JOIN syscomments e ON a.cdefault = e.id LEFT JOIN sys.extended_properties g ON a.id = g.major_id AND a.colid = g.minor_id LEFT JOIN sys.extended_properties f ON d.id = f.major_id AND f.minor_id = 0" +
                    " WHERE d.name =@name ";


                 var list = Db.Query<FieldInfo>(sql, new { name = table });

                 return list;

             });



        }


        public List<FieldInfo> GetCustomFields(string table)
        {

            var fields = _dbFieldRep.GetAll();
            var tables = Repository.GetAll();
            var fTypes = _dbFieldTypeRep.GetAll();

            var dbFieldList = GetDbFields(table);

            var query = from field in fields
                        join tb in tables on field.TableId equals tb.Id
                        join fType in fTypes on field.FieldType equals fType.Name
                        where tb.Name == table
                        select new { field.FieldType, DataTypeName = fType.Descpt, field.ColName, field.IsSys };


            var list = query.ToList();

            var fieldList = new List<FieldInfo>();

            foreach (var field in list)
            {
                var dbField = dbFieldList.FirstOrDefault(m => m.Name.IgnoreCaseEqual(field.ColName));
                if (dbField == null) continue;

                dbField.FieldType = field.FieldType;
                dbField.FieldTypeName = field.DataTypeName;
                dbField.IsSys = field.IsSys;
                fieldList.Add(dbField);
            }

            return fieldList;


        }

        public override async Task<DbTableDto> GetDto(int id)
        {

            var dto = await base.GetDto(id);

            var fields = GetCustomFields(dto.Name);

            var fieldDtos = fields.Where(m => !m.IsSys).MapTo<DbFieldDto>();

            dto.Fields = fieldDtos;

            return dto;


        }

        private static void CheckFieldsValid(List<DbFieldDto> fieldDtos)
        {
            var names = fieldDtos.Select(m => m.Name.ToLower()).Distinct();
            if (fieldDtos.Count != names.Count())
                throw new BusinessException("有重复字段");

            foreach (var field in fieldDtos)
            {
                if (!field.Name.IsValidParamName())
                    throw new BusinessException($"[{field.Name}]格式不正确");

                if (Consts.SystemFields.ContainsKey(field.Name))
                    throw new BusinessException($"[{field.Name}]是系统字段将自动添加，无需设置");
            }


        }



        public override async Task<DbTableDto> Create(CreateDbTableDto dto)
        {

            if (!dto.Name.IsValidParamName())
            {
                throw new BusinessException("数据表名称格式不正确");
            }

            CheckFieldsValid(dto.Fields);

            var model = await Repository.GetAll().Where(m => m.Name == dto.Name).FirstOrDefaultAsync();
            if (model != null)
            {
                throw new BusinessException("数据表已存在");
            }

            CheckDbTableExist(dto.Name);

            var index = 0;
            foreach (var sysField in Consts.SystemFields)
            {
                var field = sysField.Key;
                var fieldDto = new DbFieldDto { Name = field, FieldType = sysField.Value };
                if (field == "Id")
                {
                    fieldDto.Length = 50;
                    fieldDto.IsKey = true;
                }
                dto.Fields.Insert(index++, fieldDto);
            }


            var sql = GetSqlServerCreateTableSql(dto.Name, dto.Fields);

            Db.Execute(sql);

            var result = await base.Create(dto);

            foreach (var field in dto.Fields)
            {
                var dbField = new DbField
                {
                    ColName = field.Name,
                    TableId = result.Id,
                    FieldType = field.FieldType,
                    IsSys = Consts.SystemFields.ContainsKey(field.Name)
                };
                await _dbFieldRep.InsertAsync(dbField);

            }

            return result;

        }


        public override async Task<DbTableDto> Update(CreateDbTableDto dto)
        {

            dto.Fields = dto.Fields.Where(m => !Consts.SystemFields.ContainsKey(m.Name)).ToList();

            CheckFieldsValid(dto.Fields);

            var model = Repository.Get(dto.Id);
            var name = model.Name;

            var oldFields = GetCustomFields(name).Where(m => !m.IsSys).ToList(); 

            var deletedNames = oldFields.Where(m => !dto.Fields.Any(n => m.Name.IgnoreCaseEqual(n.Name))).Select(m => m.Name).ToList();
            if (deletedNames.Count > 0)
            {
                _dbFieldRep.Delete(m => deletedNames.Contains(m.ColName));
            }

            var addedFields = dto.Fields.Where(m => !oldFields.Any(n => m.Name.IgnoreCaseEqual(n.Name)));

            foreach (var field in addedFields)
            {

                var dbField = new DbField
                {
                    ColName = field.Name,
                    TableId = model.Id,
                    FieldType = field.FieldType,
                    IsSys = false
                };
                await _dbFieldRep.InsertAsync(dbField);

            }

            var sql = GetSqlServerUpdateTableSql(name, oldFields, dto.Fields);
            if (sql.IsNotEmpty())
            {
                Db.Execute(sql);
            }

            dto.Name = name;

            Cache.Remove(DbFiledCacheKey.FormatEx(name.ToLower()));

            return await base.Update(dto);

        }


        [AuthorizeEx("addtable")]
        public async Task<RetResult> Import(string table)
        {

            var fields = GetDbFields(table);
            if (fields.Count == 0)
                return Error("数据表不存在");

            var model = await Repository.GetAll().Where(m => m.Name == table).FirstOrDefaultAsync();
            if (model != null)
            {
                return Error("数据表已存在");
            }

            var dto = new CreateDbTableDto
            {
                Name = table

            };

            var result = await base.Create(dto);

            foreach (var f in fields)
            {
                var dbField = new DbField
                {
                    ColName = f.Name,
                    TableId = result.Id,
                    FieldType = "str"

                };


                var dataType = f.DataType;
                if (dataType.IgnoreCaseEqual("varchar") || dataType.IgnoreCaseEqual("nvarchar"))
                {
                    dbField.FieldType = "str";
                    if (f.Length == -1)
                        dbField.FieldType = "text";
                }
                else if (dataType.IgnoreCaseEqual("text") || dataType.IgnoreCaseEqual("ntext"))
                {
                    dbField.FieldType = "text";
                }
                else if (dataType.IgnoreCaseEqual("int"))
                {
                    dbField.FieldType = "int";
                }
                else if (dataType.IgnoreCaseEqual("decimal"))
                {
                    dbField.FieldType = "money";
                }
                else if (dataType.IgnoreCaseEqual("float"))
                {
                    dbField.FieldType = "float";
                }
                else if (dataType.IgnoreCaseEqual("datetime"))
                {
                    dbField.FieldType = "datetime";
                }
                else if (dataType.IgnoreCaseEqual("date"))
                {
                    dbField.FieldType = "datetime";
                }
                else if (dataType.IgnoreCaseEqual("bit"))
                {
                    dbField.FieldType = "int";
                }

                if (Consts.SystemFields.ContainsKey(f.Name))
                {
                    dbField.IsSys = true;

                }

                await _dbFieldRep.InsertAsync(dbField);
            }

            return Success(result);



        }

        [RemoteService(false)]
        public override Task<int> Delete(int id)
        {
            return base.Delete(id);
        }


        public async Task<DbTable> GetByName(string name)
        {

            var model = await Repository.GetAll().Where(m => m.Name == name).FirstOrDefaultAsync();
            return model;

        }


    }
}
