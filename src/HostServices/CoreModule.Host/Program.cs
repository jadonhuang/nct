using Nct.AspNetCore;
using Nct.Module.Core.AppSettings;



var builder = WebApplication.CreateBuilder(args);
builder.Host.UseCastleWindsor();
builder.Configuration.AddDbConfiguration();
builder.Services.AddNct<HostModule>();
var app = builder.Build();
app.UseNct();
app.Run();

