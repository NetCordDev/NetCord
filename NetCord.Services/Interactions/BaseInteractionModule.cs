namespace NetCord.Services.Interactions;

#nullable disable

public class BaseInteractionModule<TContext> : IBaseModule<TContext> where TContext : IInteractionContext
{
    public TContext Context => _context;

#pragma warning disable IDE0032 // Use auto property
    private TContext _context;
#pragma warning restore IDE0032 // Use auto property

    void IBaseModule<TContext>.SetContext(TContext context)
    {
        _context = context;
    }
}
