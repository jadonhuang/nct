using Nct.Identity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Identity.Jwt
{
    public interface IJwtTokenGenerator
    {
        JwtToken Create(UserBase user);
    }
}
