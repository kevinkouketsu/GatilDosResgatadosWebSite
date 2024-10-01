using FastEndpoints;
using FluentValidation;
using GatilDosResgatadosApi.Core.Abstractions;
using GatilDosResgatadosApi.Infrastructure.Identity;
using GatilDosResgatadosApi.Infrastructure.Options;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace GatilDosResgatadosApi.Areas.Users.Features;

public record CreateUserRequest
{
    public string Name { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
    public string Email { get; set; } = default!;
}

public class CreateUserRequestValidator : Validator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O campo Nome é obrigatório.")
            .MaximumLength(20).WithMessage("O campo Nome tem um máximo de 20 caracteres.");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("O campo Sobrenome é obrigatório.")
            .MaximumLength(40).WithMessage("O campo Sobrenome tem um máximo de 40 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O campo e-mail é obrigatório")
            .EmailAddress().WithMessage("Digite um e-mail válido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("O campo senha é obrigatória")
            .MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres")
            .Matches(@"[A-Z]+").WithMessage("Sua senha deve conter no mínimo um caracter maiúsculo.")
            .Matches(@"[a-z]+").WithMessage("Sua senha deve conter no mínimo um caracter minúsculo.")
            .Matches(@"[0-9]+").WithMessage("Sua senha deve conter no mínimo um número.")
            .Matches(@"[\!\?\*\.]+").WithMessage("Sua senha deve conter no mínimo um caractere dos seguintes: (!? *.).");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("O campo confirmação de senha é obrigatório.")
            .Equal(x => x.Password).WithMessage("As senhas não são iguais.");
    }
}

public class CreateUser(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IEmailGateway emailGateway
) : Endpoint<CreateUserRequest>
{
    public override void Configure()
    {
        Post("/api/user");
        AllowAnonymous();
    }

    public async override Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var user = new ApplicationUser
        {
            UserName = req.Email,
            Email = req.Email,
            EmailConfirmed = false,
            Name = req.Name,
            Surname = req.Surname
        };

        var result = await signInManager.UserManager.CreateAsync(user, req.Password);
        if (result.Succeeded)
        {
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = QueryHelpers.AddQueryString(BaseURL, new Dictionary<string, string?>() { { "id", user.Id }, { "token", code } });

            await emailGateway.SendRegisterConfirmation(user.Email, url);
        }
        else
        {
            foreach (var error in result.Errors)
            {
                AddError(error.Description);
            }

            await SendAsync(new ProblemDetails(ValidationFailures));
        }
    }
}
