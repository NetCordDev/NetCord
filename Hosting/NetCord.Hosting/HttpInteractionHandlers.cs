namespace NetCord.Hosting;

public interface IHttpInteractionHandler
{
    /// <summary>
    /// Handles HTTP <see cref="Interaction"/>s.
    /// </summary>
    /// <param name="interaction">The <see cref="Interaction"/> received.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public ValueTask HandleAsync(Interaction interaction);
}

internal class DelegateHttpInteractionHandler(IServiceProvider services, Delegate handler) : IHttpInteractionHandler
{
    private readonly Func<Interaction, IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<Interaction, IServiceProvider, ValueTask>>(handler, [typeof(Interaction)]);

    public ValueTask HandleAsync(Interaction interaction) => _handler(interaction, services);
}
