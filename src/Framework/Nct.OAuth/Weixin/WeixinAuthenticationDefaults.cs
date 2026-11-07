using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.OAuth.Weixin
{
   public class WeixinAuthenticationDefaults
    {

        public const string AuthenticationScheme = "Weixin";
      
        public static readonly string DisplayName = "Weixin";

        public static readonly string CallbackPath = "/Weixin";

       
        public static readonly string Issuer = "Weixin";

    
        public static readonly string AuthorizationEndpoint = "https://open.weixin.qq.com/connect/qrconnect";

      
        public static readonly string TokenEndpoint = "https://api.weixin.qq.com/sns/oauth2/access_token";
                                            
    
        public static readonly string UserInformationEndpoint = "https://api.weixin.qq.com/sns/userinfo";

    }
}
