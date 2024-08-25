using NetCord.Gateway;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public class ComponentInteractionServiceOptions<TInteraction, TContext> where TInteraction : Interaction where TContext : IComponentInteractionContext
{
    public ComponentInteractionServiceConfiguration<TContext> Configuration { get; set; } = ComponentInteractionServiceConfiguration<TContext>.Default;

    public bool UseScopes { get; set; } = true;

    public Func<TInteraction, GatewayClient?, IServiceProvider, TContext>? CreateContext { get; set; }

    public IComponentInteractionResultHandler<TContext> ResultHandler { get; set; } = new ComponentInteractionResultHandler<TContext>();
}
