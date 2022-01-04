namespace NetCord;

public abstract class ClientEntity : Entity
{
    protected RestClient _client;

    protected ClientEntity(RestClient client)
    {
        _client = client;
    }
}