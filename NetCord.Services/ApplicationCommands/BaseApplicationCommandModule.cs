namespace NetCord.Services.ApplicationCommands;

#nullable disable

public class BaseApplicationCommandModule<TContext> : IBaseModule<TContext> where TContext : IApplicationCommandContext
{
    public TContext Context => _context;

    private TContext _context;

    void IBaseModule<TContext>.SetContext(TContext context)
    {
        _context = context;
    }
}
