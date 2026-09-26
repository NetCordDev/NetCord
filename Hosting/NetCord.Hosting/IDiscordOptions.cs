namespace NetCord.Hosting;

public interface IDiscordOptions
{
    /// <summary>
    /// The token used to authenticate with Discord.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// The public key used to validate HTTP interactions.
    /// </summary>
    public string? PublicKey { get; set; }
}
