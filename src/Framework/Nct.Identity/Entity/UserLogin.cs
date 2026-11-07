using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;

namespace Nct.Identity.Entity
{    
    [Table("Core_UserLogin")]
    public class UserLogin:IEntity
    {
       
		[Key]
		public int Id { get; set; }

       
		public int UserId { get; set; }

       
		public string LoginProvider { get; set; }

       
		public string ProviderKey { get; set; }


        public UserLogin( int userId, string loginProvider, string providerKey)
        {
           
            UserId = userId;
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
        }

    }
}
