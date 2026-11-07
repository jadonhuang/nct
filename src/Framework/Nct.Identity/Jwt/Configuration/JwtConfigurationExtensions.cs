using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Identity.Jwt.Configuration
{
    public static class JwtConfigurationExtensions
    {
        public static JwtConfigurationModel ToJwtConfigurationModel(this JwtConfiguration config)
        {

            var model = new JwtConfigurationModel
            {
                SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.SecurityKey)),
                Issuer = config.Issuer,
                Audience = config.Audience
            };
            model.SigningCredentials = new SigningCredentials(model.SecurityKey, SecurityAlgorithms.HmacSha256);
            model.Expiration = TimeSpan.FromMinutes(config.Expire.Value);
            return model;

        }
    }
}
