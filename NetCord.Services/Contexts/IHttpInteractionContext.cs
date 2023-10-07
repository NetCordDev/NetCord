using NetCord.Rest;

namespace NetCord.Services;

public interface IHttpInteractionContext
{
    public InteractionCallback? Callback { get; set; }
}
