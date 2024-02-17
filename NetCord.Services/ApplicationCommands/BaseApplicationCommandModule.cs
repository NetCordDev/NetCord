namespace NetCord.Services.ApplicationCommands;

#nullable disable

public class BaseApplicationCommandModule<TContext> : IBaseModule<TContext> where TContext : IApplicationCommandContext
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
