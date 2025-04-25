using NetCord.Gateway;

namespace NetCord.Services;

public interface IGuildContext : IContext
{
    public Guild? Guild { get; }

    internal protected ulong? GuildId { get; }
}
