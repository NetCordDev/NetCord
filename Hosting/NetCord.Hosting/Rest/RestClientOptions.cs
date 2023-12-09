using NetCord.Rest;

namespace NetCord.Hosting.Rest;

public class RestClientOptions : IDiscordOptions
{
    public string? Token { get; set; }

    public string? PublicKey { get; set; }

    public RestClientConfiguration? Configuration { get; set; }
}
