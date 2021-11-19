namespace NetCord.Interactions;

public abstract class ButtonInteractionModule<TContext> where TContext : IButtonInteractionContext
{
    public TContext Context { get; internal set; }
}

public abstract class ButtonInteractionModule : ButtonInteractionModule<ButtonInteractionContext>
{
}