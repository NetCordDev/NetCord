namespace NetCord.Services.Interactions;

#nullable disable

public class BaseInteractionModule<TContext> where TContext : IInteractionContext
{
    public TContext Context { get; internal set; }
}
