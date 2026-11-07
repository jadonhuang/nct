using Nct.Module.Core.Users.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Nct.Module.Core.Organizations.Dtos
{
	[Map(typeof(OrganizationDto))]
    public class EditOrganizationDto:IEntityDto
    {
  
		public int Id { get; set; }

        public string Name { get; set; }
	
		public int ParentId { get; set; }

		[StringLength(200)]  
		[Display(Name ="描述")]
		public string Descpt { get; set; }


		public List<UserDto> Managers { get; set; }


    }
}
