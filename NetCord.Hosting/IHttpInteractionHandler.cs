namespace NetCord.Hosting;

public interface IHttpInteractionHandler
{
    public ValueTask HandleAsync(Interaction interaction);
}
