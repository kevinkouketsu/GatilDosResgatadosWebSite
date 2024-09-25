using GatilDosResgatadosApi.Core.Abstractions;

namespace GatilDosResgatadosApi.Core.Services;

public class NoopEmailGateway : IEmailGateway
{
    public Task SendRegisterConfirmation(string to, string callbackUrl)
    {
        return Task.CompletedTask;
    }
}
