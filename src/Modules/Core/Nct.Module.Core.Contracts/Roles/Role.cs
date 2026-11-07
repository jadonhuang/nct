using Nct.Domain.Entities;
using Nct.Identity.Entity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nct.Module.Core.Roles
{
    [Table("Core_Role")]
    public class Role:RoleBase,IHasCreateTime,IHasCreateUser,ISoftDelete
    {
       
	
   
		public string Permissions { get; set; }

       
		public bool IsDeleted { get; set; }

       
		public string Descpt { get; set; }

       
		public string CreateUser { get; set; }

       
		public DateTime CreateTime { get; set; }


    }
}
