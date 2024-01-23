using NetCord.Rest;

namespace NetCord;

public partial class CurrentApplication : Application
{
    private readonly RestClient _client;

    public CurrentApplication(JsonModels.JsonApplication jsonModel, RestClient client) : base(jsonModel, client)
    {
        _client = client;
    }
}
