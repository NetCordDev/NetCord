namespace NetCord.Interactions;

public abstract class MenuInteractionModule<TContext> where TContext : IMenuInteractionContext
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public TContext Context { get; internal set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

public abstract class MenuInteractionModule : MenuInteractionModule<MenuInteractionContext>
{
}