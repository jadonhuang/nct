using System;
using System.Collections.Generic;
using Nct.ObjectMapper;
using Nct.Application;

namespace Nct.Application.Form.RuleDefs.Dtos
{

    [Map(typeof(RuleDef))]
    public class RuleDefDto : IEntityDto
    {

        public int Id { get; set; }


        public string Name { get; set; }


        public string Code { get; set; }


        public string Cate { get; set; }

        public string CateName { get; set; }

        public int Type { get; set; }


        public DateTime CreateTime { get; set; }


        public bool IsDeleted { get; set; }


        public string CreateUser { get; set; }


        public string Descpt { get; set; }




    }
}
