namespace NetCord.Services;

#nullable disable

internal interface IBaseModule<TContext>
{
    public TContext Context { get; }

    internal void SetContext(TContext context);
}
