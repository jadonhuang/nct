using Microsoft.Extensions.Configuration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOcelot();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("default",
       builder => builder
           .SetIsOriginAllowed(_ => true)
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials()
          );

});

var app = builder.Build();

app.UseCors("default");
app.UseRouting().UseEndpoints(r =>
       {
            r.MapDefaultControllerRoute();
       });
app.MapGet("/", () => "gateway is running");
app.UseOcelot().Wait();

app.Run();

