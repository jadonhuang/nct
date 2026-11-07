using Nct.Models;
using Nct.Module.Core.Dictionarys.Dtos;
using System;

namespace Nct.Module.Core.Organizations.Dtos
{

    [Map(typeof(Organization))]
    public class OrganizationDto : IEntityDto, ITreeNode<int, OrganizationDto>
    {

        public int Id { get; set; }


        public string Name { get; set; }


        public string Code { get; set; }


        public int ParentId { get; set; }


        public string CreateUser { get; set; }


        public DateTime CreateTime { get; set; }


        public string Descpt { get; set; }


        public IList<OrganizationDto> Children { get; set; }


    }
}
