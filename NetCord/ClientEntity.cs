using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public abstract class ClientEntity(ulong id, RestClient client) : Entity(id)
{
    public ClientEntity(JsonEntity jsonModel, RestClient client) : this(jsonModel.Id, client)
    {
    }

    private protected readonly RestClient _client = client;
}
