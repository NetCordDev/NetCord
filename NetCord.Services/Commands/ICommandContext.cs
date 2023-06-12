using NetCord.Gateway;

namespace NetCord.Services.Commands;

public interface ICommandContext
{
    public Message Message { get; }
}
