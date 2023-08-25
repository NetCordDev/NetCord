namespace NetCord.Services.Commands;

#nullable disable

public abstract class BaseCommandModule<TContext> : IBaseModule<TContext> where TContext : ICommandContext
{
    public TContext Context => _context;

    private TContext _context;

    void IBaseModule<TContext>.SetContext(TContext context)
    {
        _context = context;
    }
}
