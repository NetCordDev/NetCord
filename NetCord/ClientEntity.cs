namespace NetCord;

public abstract class ClientEntity : Entity
{
    protected BotClient _client;

    protected ClientEntity(BotClient client)
    {
        _client = client;
    }
}