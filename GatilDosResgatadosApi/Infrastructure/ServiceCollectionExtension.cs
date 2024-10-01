using GatilDosResgatadosApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GatilDosResgatadosApi.Infrastructure.Options;
using GatilDosResgatadosApi.Infrastructure.Data;
using GatilDosResgatadosApi.Core.Abstractions;
using GatilDosResgatadosApi.Core.Services;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace GatilDosResgatadosApi.Infrastructure;

public static class ServiceCollectionExtension
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(x => x.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddErrorDescriber<IdentityPortugueseMessages>()
            .AddDefaultTokenProviders();

        var secret = configuration.GetSection("TokenConfiguration").Get<JwtTokenOptions>()!.Secret;
        services.AddAuthenticationJwtBearer(s => s.SigningKey = secret);

        services.AddAuthentication(o => o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme);
        return services;
    }

    public static IServiceCollection ConfigureEmailSender(this IServiceCollection services, IConfiguration _1)
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
        services.AddDbContext<ApplicationDbContext>(c =>
            c.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}
