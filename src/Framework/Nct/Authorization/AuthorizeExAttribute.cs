using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Authorization
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AuthorizeExAttribute : Attribute
    {

        public string Roles { get; set; }

        public string Permissions { get; set; }

        public AuthorizeExAttribute(string permissions)
        {

            Permissions = permissions;
        }

        public AuthorizeExAttribute()
        {

        }

    }
}
