using NetCord.Rest;

namespace NetCord.Services;

public interface IRestMessageContext : IContext
{
    public RestMessage Message { get; }
}
