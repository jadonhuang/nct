using System;
using System.Collections.Generic;
using Nct.ObjectMapper;
using Nct.Application;

namespace Nct.Module.Form.DataSources.Dtos
{

    [Map(typeof(DataSource))]
    public class DataSourceDto : IEntityDto
    {

        public int Id { get; set; }


        public string Name { get; set; }


        public string Code { get; set; }


        public int Cate { get; set; }


        public DateTime CreateTime { get; set; }


        public string CreateUser { get; set; }


        public string Descpt { get; set; }


        public int Type { get; set; }



    }
}
