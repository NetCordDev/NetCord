using NetCord.Gateway;

namespace NetCord.Services.Commands;

public interface ICommandContext : IGatewayClientContext
{
    public Message Message { get; }
}
