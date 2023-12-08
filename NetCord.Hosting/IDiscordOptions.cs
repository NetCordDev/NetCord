namespace NetCord.Hosting;

public interface IDiscordOptions
{
    public string? Token { get; set; }

    public string? PublicKey { get; set; }
}
