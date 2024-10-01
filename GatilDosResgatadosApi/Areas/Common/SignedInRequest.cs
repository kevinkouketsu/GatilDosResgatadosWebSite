using FastEndpoints;
using FluentValidation;

namespace GatilDosResgatadosApi.Areas.Common;

public record SignedInRequest
{
    [FromClaim("UserID")] 
    public string? UserId { get; set; }
}

public class Validator : Validator<SignedInRequest>
{
    public Validator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}


