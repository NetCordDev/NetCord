namespace NetCord.Services.Interactions;

#nullable disable

public class BaseInteractionModule<TContext> : IBaseModule<TContext> where TContext : IInteractionContext
{
    public TContext Context => _context;

    private TContext _context;

    void IBaseModule<TContext>.SetContext(TContext context)
    {
        _context = context;
    }
}
