using System.ComponentModel.DataAnnotations;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

public class ShardedGatewayClientOptions : IDiscordOptions
{
    [Required]
    public string? Token { get; set; }

    public string? PublicKey { get; set; }

    public ShardedGatewayClientConfiguration? Configuration { get; set; }
}
