using NetCord.Rest;

namespace NetCord;

public class SelfUser : User
{
    public SelfUser(JsonModels.JsonUser jsonModel, RestClient client) : base(jsonModel, client)
    {
    }

    #region User
    public Task<SelfUser> ModifyAsync(Action<SelfUserProperties> action, RequestProperties? properties = null) => _client.ModifyCurrentUserAsync(action, properties);
    #endregion
}
