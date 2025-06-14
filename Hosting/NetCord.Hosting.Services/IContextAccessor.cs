namespace NetCord.Hosting.Services;

public interface IContextAccessor<TContext>
{
    public TContext? Context { get; }

    internal void SetContext(TContext? context);
}

internal class ContextAccessor<TContext> : IContextAccessor<TContext>
{
    private static readonly AsyncLocal<TContext?> _context = new();

    public TContext? Context => _context.Value;

    void IContextAccessor<TContext>.SetContext(TContext? context)
    {
        _context.Value = context;
    }
}
