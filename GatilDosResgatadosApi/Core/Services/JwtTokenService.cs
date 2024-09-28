using FastEndpoints;
using FastEndpoints.Security;
using GatilDosResgatadosApi.Infrastructure.Identity;
using GatilDosResgatadosApi.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GatilDosResgatadosApi.Core.Services;

public class JwtTokenService : RefreshTokenService<TokenRequest, TokenResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public JwtTokenService(UserManager<ApplicationUser> userManager, IOptions<JwtTokenOptions> config)
    {
        _userManager = userManager;

        Setup(o =>
        {
            var tokenOption = config.Value!;
            o.TokenSigningKey = tokenOption.Secret;
            o.AccessTokenValidity = TimeSpan.FromMinutes(5);
            o.RefreshTokenValidity = TimeSpan.FromHours(4);

            o.Endpoint("/api/refresh-token", ep =>
            {
                ep.Summary(s => s.Summary = "this is the refresh token endpoint");
            });
        });
    }

    public override async Task PersistTokenAsync(TokenResponse response)
    {
        var user = await _userManager.FindByIdAsync(response.UserId);
        if (user is null) 
        {
            throw new InvalidOperationException("Invalid state, user should be always found");
        }

        user.RefreshToken = response.RefreshToken;
        user.RefreshTokenExpiryTime = response.RefreshExpiry;

        if (!(await _userManager.UpdateAsync(user)).Succeeded)
        {
            throw new InvalidOperationException("Failed to update user information.");
        }
    }

    public async override Task RefreshRequestValidationAsync(TokenRequest req)
    {
        var user = await _userManager.FindByIdAsync(req.UserId);
        if (user is null)
        {
            throw new InvalidOperationException("Invalid state, user should be always found");
        }

        if (user.RefreshToken != req.RefreshToken)
            AddError("The refresh token is not valid");
    }

    public override Task SetRenewalPrivilegesAsync(TokenRequest request, UserPrivileges privileges)
    {
        throw new NotImplementedException();
    }
}
