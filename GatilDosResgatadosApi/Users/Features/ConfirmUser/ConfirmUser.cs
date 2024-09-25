using FastEndpoints;

namespace GatilDosResgatadosApi.Users.Features.ConfirmUser;

public class ConfirmUser : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/api/confirm-user");
        AllowAnonymous();
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        return base.HandleAsync(ct);
    }
}
