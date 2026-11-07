using Nct.Domain.Entities;
using Nct.Identity.Entity;
using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nct.Module.Core.Users
{

    public class User: UserBase, ISoftDelete,IHasCreateTime
    {
       
	
       
		public string RealName { get; set; }

       
		public DateTime? LastLoginTime { get; set; }

       
		public int UserType { get; set; }

       
		public string Pic { get; set; }

       
		public int Sex { get; set; }

       
		public string Address { get; set; }

       
		public DateTime? Birthday { get; set; }

       
		public string CreateUser { get; set; }

       
		public DateTime CreateTime { get; set; }

       
		public bool IsDeleted { get; set; }

        public decimal Balance { get; set; }

        public int LevelId { get; set; }

        public int ParentId { get; set; }

        public string GetDisplayName()
        {
            return RealName.IsEmpty() ? UserName : RealName;

        }

    }
}
