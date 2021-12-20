namespace NetCord.Interactions;

public abstract class ButtonInteractionModule<TContext> where TContext : IButtonInteractionContext
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public TContext Context { get; internal set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

public abstract class ButtonInteractionModule : ButtonInteractionModule<ButtonInteractionContext>
{
}