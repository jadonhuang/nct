using Nct.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nct.Module.Form.FormDefs
{


    [Table("Form_FormDef")]
    public class FormDef:IEntity,IHasCreateTime,IHasCreateUser,ISoftDelete
    {
       
		[Key]
		public int Id { get; set; }

       
		public string Name { get; set; }

       
		public string Code { get; set; }

       
		public int Cate { get; set; }

       
		public DateTime CreateTime { get; set; }

       
		public bool IsDeleted { get; set; }

       
		public string CreateUser { get; set; }

       
		public string Descpt { get; set; }

       
		public int Type { get; set; }

       
		public string Data { get; set; }
      
		public string ClientRule { get; set; }

        public string ServerRule { get; set; }


    }
}
