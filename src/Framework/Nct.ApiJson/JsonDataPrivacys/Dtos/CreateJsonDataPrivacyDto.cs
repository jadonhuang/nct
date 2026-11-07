using System;
using System.ComponentModel.DataAnnotations;
using Nct.ObjectMapper;
using Nct.Application;

namespace Nct.ApiJson.JsonDataPrivacys.Dtos
{    
  
    [Map(typeof(JsonDataPrivacy))]
    public class CreateJsonDataPrivacyDto:IEntityDto
    {
  
		[Display(Name ="Id")]
		public int Id { get; set; }

		[Required]
		[StringLength(50)]  
		[Display(Name ="数据表")]
		public string TableName { get; set; }

  
		[Display(Name ="SelectAllowAnonymous")]
		public bool SelectAllowAnonymous { get; set; }


		[StringLength(500)]  
		[Display(Name ="SelectAllowRoles")]
		public string SelectAllowRoles { get; set; }


		[StringLength(500)]  
		[Display(Name ="SelectNotAllowedFields")]
		public string SelectNotAllowedFields { get; set; }

  
		[Display(Name ="UpdateAllowAnonymous")]
		public bool UpdateAllowAnonymous { get; set; }


		[StringLength(500)]  
		[Display(Name ="UpdateAllowRoles")]
		public string UpdateAllowRoles { get; set; }


		[StringLength(500)]  
		[Display(Name ="UpdateNotAllowFields")]
		public string UpdateNotAllowFields { get; set; }

  
		[Display(Name ="DeleteAllowAnonymous")]
		public bool DeleteAllowAnonymous { get; set; }


		[StringLength(500)]  
		[Display(Name ="DeleteAllowRoles")]
		public string DeleteAllowRoles { get; set; }

  
		[Display(Name ="InsertAllowAnonymous")]
		public bool InsertAllowAnonymous { get; set; }


		[StringLength(500)]  
		[Display(Name ="InsertAllowRoles")]
		public string InsertAllowRoles { get; set; }


		[StringLength(500)]  
		[Display(Name ="InsertNotAllowedFields")]
		public string InsertNotAllowedFields { get; set; }

  
	
		public bool SelectEnabled { get; set; }

  

		public bool DeleteEnabled { get; set; }

  

		public bool UpdateEnabled { get; set; }


		public bool InsertEnabled { get; set; }


    }
}
