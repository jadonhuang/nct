using Nct.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nct.ApiJson.JsonDataPrivacys
{
    [Table("Core_JsonDataPrivacy")]
    public class JsonDataPrivacy:IEntity
    {
       
		[Key]
		public int Id { get; set; }

       
		public string TableName { get; set; }

       
		public bool SelectAllowAnonymous { get; set; }

       
		public string SelectAllowRoles { get; set; }

       
		public string SelectNotAllowedFields { get; set; }

       
		public bool UpdateAllowAnonymous { get; set; }

       
		public string UpdateAllowRoles { get; set; }

       
		public string UpdateNotAllowFields { get; set; }

       
		public bool DeleteAllowAnonymous { get; set; }

       
		public string DeleteAllowRoles { get; set; }

       
		public bool InsertAllowAnonymous { get; set; }

       
		public string InsertAllowRoles { get; set; }

       
		public string InsertNotAllowedFields { get; set; }

       
		public bool SelectEnabled { get; set; }

       
		public bool DeleteEnabled { get; set; }

       
		public bool UpdateEnabled { get; set; }

       
		public bool InsertEnabled { get; set; }


    }
}
