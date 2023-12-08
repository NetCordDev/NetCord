using System.ComponentModel.DataAnnotations;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

public class GatewayClientOptions : IDiscordOptions
{
    [Required]
    public string? Token { get; set; }

    public string? PublicKey { get; set; }

    public GatewayClientConfiguration? Configuration { get; set; }
}
