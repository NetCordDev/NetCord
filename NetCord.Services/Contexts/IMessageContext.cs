using NetCord.Gateway;

namespace NetCord.Services;

public interface IMessageContext
{
    public Message Message { get; }
}