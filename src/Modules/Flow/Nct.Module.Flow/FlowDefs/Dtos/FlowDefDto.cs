using System;
using System.Collections.Generic;
using Nct.ObjectMapper;
using Nct.Application;
using System.Text.Json.Serialization;

namespace Nct.Module.Flow.FlowDefs.Dtos
{

    [Map(typeof(FlowDef))]
    public class FlowDefDto : IEntityDto
    {

        public int Id { get; set; }


        public string Name { get; set; }


        public string Code { get; set; }


        public string Descpt { get; set; }


        public string Icon { get; set; }


        public string AdminIds { get; set; }


        public string Applicant { get; set; }


        [JsonIgnore]
        public bool IsDeleted { get; set; }


        public DateTime CreateTime { get; set; }


        public int Cate { get; set; }



        public string CreateUser { get; set; }


    }
}
