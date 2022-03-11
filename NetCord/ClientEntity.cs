namespace NetCord;

public abstract class ClientEntity : Entity
{
    private protected readonly RestClient _client;

    private protected ClientEntity(RestClient client)
    {
        _client = client;
    }
}