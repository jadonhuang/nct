using Nct.ObjectMapper;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Nct.Module.Form.FormDefs.Dtos
{

    [Map(typeof(FormDef))]
    public class FormDefDto : IEntityDto
    {

        [Key]
        public int Id { get; set; }


        public string Name { get; set; }


        public string Code { get; set; }


        public int Cate { get; set; }


        public DateTime CreateTime { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }


        public string CreateUser { get; set; }


        public string Descpt { get; set; }


        public int Type { get; set; }



    }
}
