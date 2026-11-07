using System;
using System.Collections.Generic;
using Nct.ObjectMapper;
using Nct.Application;

namespace Nct.Module.Form.DbFieldTypes.Dtos
{

    [Map(typeof(DbFieldType))]
    public class DbFieldTypeDto : IEntityDto
    {


        public int Id { get; set; }


        public string DataType { get; set; }


        public string Name { get; set; }

        public string Descpt { get; set; }

        public int Length { get; set; }

        public int Precision { get; set; }

    }
}
