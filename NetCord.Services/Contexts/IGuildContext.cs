using NetCord.Gateway;

namespace NetCord.Services;

public interface IGuildContext
{
    public Guild? Guild { get; }
}
