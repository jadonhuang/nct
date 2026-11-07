using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;

namespace Nct.Identity.Entity
{    
    [Table("Core_UserRole")]
    public class UserRole:IEntity
    {
       
		[Key]
		public int Id { get; set; }

       
		public int UserId { get; set; }

       
		public int RoleId { get; set; }

        public UserRole() { }

        public UserRole( int userId, int roleId)
        {

            UserId = userId;
            RoleId = roleId;
        }

    }
}
