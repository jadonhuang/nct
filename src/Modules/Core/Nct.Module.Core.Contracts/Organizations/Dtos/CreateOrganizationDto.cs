using Nct.Validation;
using System.ComponentModel.DataAnnotations;

namespace Nct.Module.Core.Organizations.Dtos
{

    [Map(typeof(Organization))]
    public class CreateOrganizationDto:IEntityDto
    {
  

		public int Id { get; set; }

		[Required]
		[StringLength(50)]  
		[Display(Name ="名称")]
        [CustomValidate(ValidateType.ValidString)]
        public string Name { get; set; }
	
		public int ParentId { get; set; }



		[StringLength(200)]  
		[Display(Name ="描述")]
		public string Descpt { get; set; }


		public List<int> ManagerIds { get; set; }


    }
}
