using FastEndpoints;
using Serilog.Events;
using Serilog;
using GatilDosResgatadosApi.Infrastructure;
using FastEndpoints.Swagger;
using FastEndpoints.Security;

var builder = WebApplication.CreateBuilder();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day).CreateLogger();

builder.Services
    .ConfigureAuthentication()
    .ConfigureOptions(builder.Configuration)
    .ConfigureEmailSender(builder.Configuration)
    .AddDatabase(builder.Configuration)
    .AddAuthorization()
    .AddFastEndpoints()
    .SwaggerDocument();

var app = builder.Build();
app.UseFastEndpoints()
   .UseSwaggerGen()
   .UseAuthentication()
   .UseAuthorization();

app.Run();
