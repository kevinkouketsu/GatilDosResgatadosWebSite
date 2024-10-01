using FastEndpoints;
using FluentValidation;
using GatilDosResgatadosApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GatilDosResgatadosApi.Areas.Users.Features.ConfirmUser;

public record ConfirmUserRequest([FromQuery] string UserId, [FromQuery] string Token);

public class ConfirmUserRequestValidator : Validator<ConfirmUserRequest>
{
    public ConfirmUserRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("O código do usuário é obrigatório");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("O token é obrigatório");
    }
}

public class ConfirmUser(UserManager<ApplicationUser> userManager, ILogger<ConfirmUser> logger) : Endpoint<ConfirmUserRequest, Results<Ok, Ok<string>, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Get("/api/user/confirm");
        AllowAnonymous();
    }

    public async override Task<Results<Ok, Ok<string>, NotFound, ProblemHttpResult>> ExecuteAsync(ConfirmUserRequest request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            logger.LogWarning("Could not find user {UserId} to confirm email", request.UserId);

            return TypedResults.NotFound();
        }

        if (user.EmailConfirmed)
        {
            return TypedResults.Ok("O e-mail já estava confirmado");
        }

        var result = await userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
            var error = string.Join(";", result.Errors.Select(x => x.Description));

            logger.LogError("Could not confirm account {Email}. Error: {Error}", user.Email, error);
            return TypedResults.Problem("Não foi possível confirmar a conta.");
        }

        return TypedResults.Ok();
    }
}
