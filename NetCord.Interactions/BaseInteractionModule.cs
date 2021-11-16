namespace NetCord.Interactions;

public abstract class BaseButtonInteractionModule<TContext> where TContext : IButtonInteractionContext
{
    public TContext Context { get; internal set; }
}

public abstract class BaseMenuInteractionModule<TContext> where TContext : IMenuInteractionContext
{
    public TContext Context { get; internal set; }
}