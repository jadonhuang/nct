using Nct.AspNetCore.WebClientInfo;
using System;

namespace Nct.Module.Core
{
    public static class WebHelper
    {

        private static readonly string[] MobileAgents = { "iphone", "android", "phone", "mobile", "wap", "netfront", "java", "opera mobi", "opera mini", "ucweb", "windows ce", "symbian", "series", "webos", "sony", "blackberry", "dopod", "nokia", "samsung", "palmsource", "xda", "pieplus", "meizu", "midp", "cldc", "motorola", "foma", "docomo", "up.browser", "up.link", "blazer", "helio", "hosin", "huawei", "novarra", "coolpad", "webos", "techfaith", "palmsource", "alcatel", "amoi", "ktouch", "nexian", "ericsson", "philips", "sagem", "wellcom", "bunjalloo", "maui", "smartphone", "iemobile", "spice", "bird", "zte-", "longcos", "pantech", "gionee", "portalmmm", "jig browser", "hiptop", "benq", "haier", "^lct", "320x320", "240x320", "176x220", "w3c ", "acs-", "alav", "alca", "amoi", "audi", "avan", "benq", "bird", "blac", "blaz", "brew", "cell", "cldc", "cmd-", "dang", "doco", "eric", "hipt", "inno", "ipaq", "java", "jigs", "kddi", "keji", "leno", "lg-c", "lg-d", "lg-g", "lge-", "maui", "maxo", "midp", "mits", "mmef", "mobi", "mot-", "moto", "mwbp", "nec-", "newt", "noki", "oper", "palm", "pana", "pant", "phil", "play", "port", "prox", "qwap", "sage", "sams", "sany", "sch-", "sec-", "send", "seri", "sgh-", "shar", "sie-", "siem", "smal", "smar", "sony", "sph-", "symb", "t-mo", "teli", "tim-", "tosh", "tsm-", "upg1", "upsi", "vk-v", "voda", "wap-", "wapa", "wapi", "wapp", "wapr", "webc", "winw", "winw", "xda", "xda-", "Googlebot-Mobile" };


    

        private static string GetUserAgent()
        {
            string userAgent = null;
            IocManager.Instance.Using<IWebClientInfoProvider>(t =>
            {

                userAgent = t.BrowserInfo;

            });

            return userAgent;

        }

        public static bool IsMobileRequest()
        {

            string userAgent = GetUserAgent();
           
            if (userAgent == null) return false;

            return MobileAgents.Any(m => userAgent.IndexOf(m, StringComparison.OrdinalIgnoreCase) != -1);

        }

        public static bool IsWxRequest()
        {

            string userAgent = GetUserAgent();

            if (userAgent == null) return false;

            return userAgent.IndexOf("MicroMessenger", StringComparison.OrdinalIgnoreCase) != -1;
        }

        public static bool IsAliPayRequest()
        {

            string userAgent = GetUserAgent();

            if (userAgent == null) return false;

            return userAgent.IndexOf("Alipay", StringComparison.OrdinalIgnoreCase) != -1;
        }




     

    }
}
