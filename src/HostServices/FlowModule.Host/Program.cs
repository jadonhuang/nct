using Nct.AspNetCore;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseCastleWindsor();
builder.Services.AddNct<HostModule>();
var app = builder.Build();
app.UseNct();
app.Run();

