using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Localization
{
    public static class LocalizerExtensions
    {
        private const string DefaultCulture = "zh-CN";
        public static void UseCustomRequestLocalization(this IApplicationBuilder app)
        {
            var supportedCultures = LocalizerManager.GetAllCultures().Select(m => m.Id).ToArray();
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(DefaultCulture)
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            var zh = localizationOptions.SupportedCultures.Where(m => m.Name == DefaultCulture).FirstOrDefault();
            if (zh != null)
            {
                 zh.DateTimeFormat = new DateTimeFormatInfo { ShortDatePattern = "yyyy/MM/dd", FullDateTimePattern = "yyyy/MM/dd HH:mm:ss", LongTimePattern = "HH:mm:ss"};
            }

            app.UseRequestLocalization(localizationOptions);

        }



    }
}
