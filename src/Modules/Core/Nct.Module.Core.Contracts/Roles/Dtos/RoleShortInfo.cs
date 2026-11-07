using Nct.Identity.Entity;

namespace Nct.Module.Core.Roles.Dtos
{

    [Map(typeof(Role))]
    public class RoleShortInfo:RoleBase
    {

       
        public string Descpt { get; set; }

        public bool Checked { get; set; }




    }
}
