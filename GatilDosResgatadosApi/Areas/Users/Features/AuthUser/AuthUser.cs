using FastEndpoints;
using FastEndpoints.Security;
using GatilDosResgatadosApi.Core.Services;
using GatilDosResgatadosApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace GatilDosResgatadosApi.Areas.Users.Features.AuthUser;

public record AuthUserRequest(string Email, string Password);

public class AuthUser(UserManager<ApplicationUser> userManager) : Endpoint<AuthUserRequest, TokenResponse>
{
    public override void Configure()
    {
        Post("/api/user/auth");
        AllowAnonymous();
    }

    public async override Task HandleAsync(AuthUserRequest req, CancellationToken ct)
    {
        var user = await userManager.FindByNameAsync(req.Email);
        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        bool isEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);
        if (!isEmailConfirmed)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var result = await userManager.CheckPasswordAsync(user, req.Password);
        if (!result)
        {
            await SendResultAsync(TypedResults.StatusCode(401));
            return;
        }

        TokenResponse token = await CreateTokenWith<JwtTokenService>(user.Id, p =>
        {
            p.Claims.Add(new("UserID", user.Id));

            // TODO permissions are not handled yet
        });

        await SendOkAsync(token, ct);
    }
}
