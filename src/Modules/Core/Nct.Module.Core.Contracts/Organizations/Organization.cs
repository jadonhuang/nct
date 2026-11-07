using Nct.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nct.Module.Core.Organizations
{
    [Table("Core_Organization")]
    public class Organization:CreationAuditEntity,ISoftDelete
    {
       
	      
		public string Name { get; set; }

       
		public string Code { get; set; }

       
		public int ParentId { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

       
		public string Descpt { get; set; }


    }
}
