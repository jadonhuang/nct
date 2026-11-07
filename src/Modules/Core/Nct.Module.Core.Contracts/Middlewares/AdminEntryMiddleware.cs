using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;


namespace Nct.Module.Core
{
    public class AdminEntryMiddleware
    {



        private readonly RequestDelegate _next;

        private readonly IConfiguration _configuration;

        public AdminEntryMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;

        }

        public async Task Invoke(HttpContext context)
        {

            var requestPath = context.Request.Path;

            if (requestPath.StartsWithSegments("/api"))
            {
                await _next.Invoke(context);
                return;
            }

            var adminEntry = _configuration[Constants.Config.AdminEntry];
            if (adminEntry.IsEmpty())
            {
                await _next.Invoke(context);
                return;
            }
            adminEntry = "/" + adminEntry;
            var adminDir = Constants.AdminFileDir;

            var indexPage = $"/{adminDir}/index.html";

            if (requestPath.ToString().IgnoreCaseEqual(indexPage))
            {
                context.Response.StatusCode = 404;
                return;

            }

            if (requestPath.StartsWithSegments(adminEntry) && Path.GetExtension(requestPath).IsEmpty())
            {
                var indexFile = Path.Combine(PathHelper.WebPath(adminDir), "index.html");
                if (!File.Exists(indexFile))
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                var html = File.ReadAllText(indexFile);

                var titleEndTag = "</title>";

                var index = html.IndexOf(titleEndTag);

                var part = html[..(index + titleEndTag.Length)];

                var part2 = html[(index + titleEndTag.Length)..];

                var loginBg = _configuration["LoginBg"];
                if (loginBg.IsNotEmpty())
                {

                    loginBg = $"window.loginBg='{loginBg}';";

                }

                var appName = _configuration["AppName"];
                if (appName.IsNotEmpty())
                {
                    appName = $"window.appName='{appName}';";

                }


                var resHtml = $"{part}{Environment.NewLine}<script>window.basePath ='{adminEntry}';window.baseUrl = '';{appName}{loginBg}</script>{Environment.NewLine}{part2}";

                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(resHtml);
                return;

            }

            await _next.Invoke(context);

        }

    }
}