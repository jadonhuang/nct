using System;
using System.Collections.Generic;
using Nct.Application;
using Nct.Identity.Entity;
using Nct.ObjectMapper;

namespace Nct.Module.Core.Roles.Dtos
{

    [Map(typeof(Role))]
    public class RoleDto : RoleBase, IEntityDto
    {

    
        //public string Permissions { get; set; }


        //public bool IsDeleted { get; set; }


        public string Descpt { get; set; }


        public string CreateUser { get; set; }


        public DateTime? CreateTime { get; set; }


    }
}
