namespace NetCord.Services.Interactions;

#nullable disable

public class InteractionModule<TContext> where TContext : InteractionContext
{
    public TContext Context { get; internal set; }
}