using System;
using System.Collections.Generic;
using Nct.ObjectMapper;
using Nct.Application;

namespace Nct.Module.Form.DbTables.Dtos
{

    [Map(typeof(DbTable))]
    public class DbTableDto : IEntityDto
    {

        public int Id { get; set; }


        public string Name { get; set; }


        public int Cate { get; set; }


        public DateTime CreateTime { get; set; }


        public bool IsDeleted { get; set; }


        public string CreateUser { get; set; }


        public string Descpt { get; set; }


        public List<DbFieldDto> Fields { get; set; }


    }
}
