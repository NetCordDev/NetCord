using NetCord.Rest;

namespace NetCord;

public class CurrentApplication : Application
{
    private readonly RestClient _client;

    public CurrentApplication(JsonModels.JsonApplication jsonModel, RestClient client) : base(jsonModel, client)
    {
        _client = client;
    }

    #region Application
    public Task<CurrentApplication> ModifyAsync(Action<CurrentApplicationOptions> action, RequestProperties? properties = null) => _client.ModifyCurrentApplicationAsync(action, properties);
    #endregion
}
