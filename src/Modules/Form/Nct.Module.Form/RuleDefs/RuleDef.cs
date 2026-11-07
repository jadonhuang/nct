using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;
using Nct.Application;

namespace Nct.Application.Form.RuleDefs
{    
    [Table("Form_RuleDef")]
    public class RuleDef:IEntity,IHasCreateTime,IHasCreateUser,ISoftDelete
    {
       
		[Key]
		public int Id { get; set; }

       
		public string Name { get; set; }

       
		public string Code { get; set; }

       
		public string Cate { get; set; }

       
		public int Type { get; set; }

       
		public DateTime CreateTime { get; set; }

       
		public bool IsDeleted { get; set; }

       
		public string CreateUser { get; set; }

       
		public string Descpt { get; set; }

       
		public string Data { get; set; }


    }
}
