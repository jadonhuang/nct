using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;
using Nct.Application;
using System.Threading.Tasks;

namespace Nct.Module.Form.DbTables
{    
    [Table("Form_DbTable")]
    public class DbTable:IEntity, ISoftDelete, IHasCreateTime, IHasCreateUser
    {
       
		[Key]
		public int Id { get; set; }

       
		public string Name { get; set; }

       
		public int Cate { get; set; }

       
		public DateTime CreateTime { get; set; }

       
		public bool IsDeleted { get; set; }

       
		public string CreateUser { get; set; }

       
		public string Descpt { get; set; }

    }
}
