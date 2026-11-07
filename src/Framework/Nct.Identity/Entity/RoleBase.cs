using Nct.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nct.Identity.Entity
{
    [Table("Core_Role")]
    public class RoleBase : IEntity
    {
        public int Id { get; set ; }

        public string Code { get; set; }

        public string Name { get; set; }

    }
}
