namespace GatilDosResgatadosApi.Infrastructure.Options;

public class GeneralOptions
{
    public string BaseUrl { get; set; } = default!;

    public string GetUrlTo(string path) => path switch
    {
        _ when path.StartsWith("/") => BaseUrl + path,
        _ => BaseUrl + "/" + path
    };
}
