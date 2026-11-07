using Nct.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nct.Module.Core.Contracts.Organizations
{

    [Table("Core_OrgManager")]
    public class OrgManager: IEntity
    {
        [Key]
        public int Id { get; set; }


        public int OrgId { get; set; }


        public int UserId { get; set; }


    }
}
