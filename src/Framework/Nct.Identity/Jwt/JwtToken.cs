using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Identity.Jwt
{
    public class JwtToken
    {

        public string AccessToken { get; set; }

        public double? ExpiresIn { get; set; }

        public string TokenType { get; set; } = "Bearer";


    }
}
