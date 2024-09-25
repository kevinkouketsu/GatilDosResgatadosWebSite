namespace GatilDosResgatadosApi.Infrastructure.Options;

public class JwtTokenOptions
{
    public string Secret { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
}
