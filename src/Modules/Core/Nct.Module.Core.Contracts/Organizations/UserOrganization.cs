using Nct.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nct.Module.Core.Organizations
{
    [Table("Core_UserOrganization")]
    public class UserOrganization:IEntity
    {
       
		[Key]
		public int Id { get; set; }

       
		public int OrgId { get; set; }

       
		public int UserId { get; set; }


    }
}
