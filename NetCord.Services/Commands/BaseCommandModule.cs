namespace NetCord.Services.Commands;

#nullable disable

public abstract class BaseCommandModule<TContext> : IBaseModule<TContext> where TContext : ICommandContext
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
