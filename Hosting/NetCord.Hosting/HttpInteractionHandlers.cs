namespace NetCord.Hosting;

public interface IHttpInteractionHandler
{
    public ValueTask HandleAsync(Interaction interaction);
}

internal class DelegateHttpInteractionHandler(IServiceProvider services, Delegate handler) : IHttpInteractionHandler
{
    private readonly Func<Interaction, IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<Interaction, IServiceProvider, ValueTask>>(handler, [typeof(Interaction)]);

    public ValueTask HandleAsync(Interaction interaction) => _handler(interaction, services);
}
