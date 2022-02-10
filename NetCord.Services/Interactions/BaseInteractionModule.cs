namespace NetCord.Services.Interactions;

#nullable disable

public class BaseInteractionModule<TContext> where TContext : InteractionContext
{
    public TContext Context { get; internal set; }
}