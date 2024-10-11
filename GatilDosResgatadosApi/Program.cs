using FastEndpoints;
using Serilog.Events;
using Serilog;
using GatilDosResgatadosApi.Infrastructure;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day).CreateLogger();

builder.Services
    .ConfigureOptions(builder.Configuration)
    .ConfigureEmailSender(builder.Configuration)
    .AddDatabase(builder.Configuration)
    .ConfigurePayment(builder.Configuration)
    .ConfigureAuthentication(builder.Configuration)
    .AddAuthorization()
    .Configure<FormOptions>(options => options.MultipartBodyLengthLimit = 10 * 1024 * 1024)
    .AddFastEndpoints()
    .SwaggerDocument(x =>
    {
        x.AutoTagPathSegmentIndex = 2;
    });

var app = builder.Build();
app
   .UseSwaggerGen()
   .UseAuthentication()
   .UseAuthorization()
   .UseFastEndpoints();

app.Run();
