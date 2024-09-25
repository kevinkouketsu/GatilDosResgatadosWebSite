namespace GatilDosResgatadosApi.Core.Abstractions;

public interface IEmailGateway
{
    Task SendRegisterConfirmation(string to, string callbackUrl);
}
