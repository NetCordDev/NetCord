namespace NetCord.Services.ApplicationCommands;

public class CreateCommandsResult
{
    internal CreateCommandsResult(IEnumerable<ApplicationCommandProperties> global, IReadOnlyDictionary<DiscordId, (IEnumerable<ApplicationCommandProperties>, IEnumerable<GuildApplicationCommandPermissionsProperties>)>? guild)
    {
        Global = global;
        Guild = guild;
    }

    public IEnumerable<ApplicationCommandProperties> Global { get; }

    public IReadOnlyDictionary<DiscordId, (IEnumerable<ApplicationCommandProperties>, IEnumerable<GuildApplicationCommandPermissionsProperties>)>? Guild { get; }
}