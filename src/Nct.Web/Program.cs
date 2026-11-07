using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Nct.AspNetCore;
using Nct.Module.Core.AppSettings;
using Nct.Web;
using NLog.Web;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseCastleWindsor();
builder.Configuration.AddDbConfiguration();
builder.Logging.ClearProviders()
    .SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

builder.Services.AddNct<WebModule>();
var app = builder.Build();
app.UseNct();
app.Run();

