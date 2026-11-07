using Microsoft.Extensions.Options;
using Nct.Identity.Entity;
using Nct.Identity.Jwt.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Nct.Identity.Jwt
{
    public class DefaultJwtTokenGenerator : IJwtTokenGenerator, ITransientDependency
    {
        private readonly AuthenOptions authenOptions;
        public DefaultJwtTokenGenerator(IOptions<AuthenOptions> options)
        {
            authenOptions = options.Value;
        }

        public JwtToken Create(UserBase user)
        {
            var tokenModel = new JwtToken();
            var jwtConfigModel = authenOptions.Authentication.JwtBearer.ToJwtConfigurationModel();
            var claims = new List<Claim>();
            var uid = user.Id.ToString();
            claims.AddRange(new[]
            {

                new Claim (ClaimTypes.NameIdentifier, uid),
                new Claim (ClaimTypes.Name ,user.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, uid),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });
            var now = DateTime.UtcNow;
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: jwtConfigModel.Issuer,
                audience: jwtConfigModel.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(jwtConfigModel.Expiration),
                signingCredentials: jwtConfigModel.SigningCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            tokenModel.AccessToken = token;
            tokenModel.ExpiresIn = jwtConfigModel.Expiration.TotalSeconds;
            return tokenModel;
        }
    }
}
