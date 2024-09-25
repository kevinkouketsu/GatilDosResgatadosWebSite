using GatilDosResgatadosApi.Infrastructure.Identity.Data;
using GatilDosResgatadosApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GatilDosResgatadosApi.Infrastructure.Options;
using System.Text;
using GatilDosResgatadosApi.Infrastructure.Data;
using FastEndpoints.Security;
using GatilDosResgatadosApi.Core.Abstractions;
using GatilDosResgatadosApi.Core.Services;

namespace GatilDosResgatadosApi.Infrastructure;

public static class ServiceCollectionExtension
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection ConfigureEmailSender(this IServiceCollection services, IConfiguration _configuration)
    {
        services.AddTransient<IEmailGateway, NoopEmailGateway>();

        return services;
    }

    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtTokenOptions>(options => configuration.GetSection("TokenConfiguration").Bind(options));
        services.Configure<GeneralOptions>(options => configuration.GetSection("General").Bind(options));

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationIdentityDbContext>(c =>
            c.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddDbContext<ApplicationDbContext>(c =>
            c.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}
