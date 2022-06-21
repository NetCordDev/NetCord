using NetCord.Rest;

namespace NetCord.Services;

public interface IRestMessageContext
{
    public RestMessage Message { get; }
}
