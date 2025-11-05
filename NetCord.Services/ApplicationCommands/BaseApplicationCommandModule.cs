namespace NetCord.Services.ApplicationCommands;

#nullable disable

/// <summary>
/// Represents a base module for application commands.
/// </summary>
/// <typeparam name="TContext">The context the invoked application commands use.</typeparam>
public abstract class BaseApplicationCommandModule<TContext> : IBaseModule<TContext> where TContext : IApplicationCommandContext
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
