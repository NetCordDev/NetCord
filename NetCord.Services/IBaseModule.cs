namespace NetCord.Services;

#nullable disable

internal interface IBaseModule<TContext>
{
    /// <summary>
    /// The context associated with the invoked command or interaction.
    /// </summary>
    public TContext Context { get; }

    internal void SetContext(TContext context);
}
