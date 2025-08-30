using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public interface IComponentInteractionsBuilder
{
    public ComponentInteractionBuilder AddComponentInteraction(string customId, Delegate handler);

    public void Build();
}

public interface IComponentInteractionsBuilder<TContext> : IComponentInteractionsBuilder where TContext : IComponentInteractionContext;

internal class ComponentInteractionsBuilder<TContext>(ComponentInteractionService<TContext> service) : IComponentInteractionsBuilder, IComponentInteractionsBuilder<TContext> where TContext : IComponentInteractionContext
{
    private List<ComponentInteractionBuilder> _builders = [];

    public ComponentInteractionBuilder AddComponentInteraction(string customId, Delegate handler)
    {
        ComponentInteractionBuilder result = new(customId, handler);
        _builders.Add(result);
        return result;
    }

    public void Build()
    {
        foreach (var builder in _builders)
            service.AddComponentInteraction(builder);

        _builders = [];
    }
}
