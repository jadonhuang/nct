using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Nct.OAuth.Weixin.WeixinAuthenticationConstants;

namespace Nct.OAuth.Weixin
{
  public  class WeixinAuthenticationOptions: OAuthOptions
    {

        public WeixinAuthenticationOptions()
        {


            ClaimsIssuer = WeixinAuthenticationDefaults.Issuer;
            CallbackPath = WeixinAuthenticationDefaults.CallbackPath;

            AuthorizationEndpoint = WeixinAuthenticationDefaults.AuthorizationEndpoint;
            TokenEndpoint = WeixinAuthenticationDefaults.TokenEndpoint;
            UserInformationEndpoint = WeixinAuthenticationDefaults.UserInformationEndpoint;

            Scope.Add("snsapi_login");
            Scope.Add("snsapi_userinfo");

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "openid");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "nickname");
            ClaimActions.MapJsonKey(ClaimTypes.Gender, "sex");
            ClaimActions.MapJsonKey(ClaimTypes.Country, "country");
            ClaimActions.MapJsonKey(Claims.UnionId, "unionid");
            ClaimActions.MapJsonKey(Claims.Province, "province");
            ClaimActions.MapJsonKey(Claims.City, "city");
            ClaimActions.MapJsonKey(Claims.HeadImgUrl, "headimgurl");
            ClaimActions.MapCustomJson(Claims.Privilege, user =>
            {
                if (!user.TryGetProperty("privilege", out var value) || value.ValueKind != System.Text.Json.JsonValueKind.Array)
                {
                    return null;
                }

                return string.Join(',', value.EnumerateArray().Select(element => element.GetString()));
            });

        }

    }
}
