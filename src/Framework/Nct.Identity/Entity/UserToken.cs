using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;

namespace Nct.Identity.Entity
{    
    [Table("Core_UserToken")]
    public class UserToken:IEntity
    {
       
		[Key]
		public int Id { get; set; }

       
		public int UserId { get; set; }

       
		public string LoginProvider { get; set; }

       
		public string Name { get; set; }

       
		public string Value { get; set; }


    }
}
