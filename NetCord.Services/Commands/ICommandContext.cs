using NetCord.Gateway;

namespace NetCord.Services.Commands;

public interface ICommandContext : IContext
{
    public Message Message { get; }
}
