namespace NetCord.Interactions;

public abstract class MenuInteractionModule<TContext> where TContext : IMenuInteractionContext
{
    public TContext Context { get; internal set; }
}

public abstract class MenuInteractionModule : MenuInteractionModule<MenuInteractionContext>
{
}