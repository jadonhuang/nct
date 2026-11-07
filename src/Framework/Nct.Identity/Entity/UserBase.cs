using Nct.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nct.Identity.Entity
{
    [Table("Core_User")]
    public class UserBase: IEntity
    {


        public int Id { get; set; }


        public string UserName { get; set; }


        public string Password { get; set; }


        public bool IsActive { get; set; }

        public string Email { get; set; }

        public string Tel { get; set; }


        public bool IsEmailConfirmed { get; set; }


        public bool IsTelConfirmed { get; set; }


        public DateTime? LockoutEndDate { get; set; }


        public bool IsLockoutEnabled { get; set; }


        public string SecurityStamp { get; set; }



        public int AccessFailedCount { get; set; }




    }
}
