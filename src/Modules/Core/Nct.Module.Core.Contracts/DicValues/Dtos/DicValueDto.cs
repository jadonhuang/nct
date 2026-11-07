using System;
using System.Collections.Generic;
using Nct.ObjectMapper;
using Nct.Application;

namespace Nct.Module.Core.DicValues.Dtos
{

    [Map(typeof(DicValue))]
    public class DicValueDto : IEntityDto
    {

        public int Id { get; set; }


        public int DicId { get; set; }


        public string Value { get; set; }


        public string Name { get; set; }

        public string Color { get; set; }

        public string Descpt { get; set; }


        public int Sort { get; set; }


    }


    public class DicNameAndValuesDto
    {
        public int DicId { get; set; }

        public string Name { get; set; }

        public  List<DicValue> Values { get; set; }
    }

}
