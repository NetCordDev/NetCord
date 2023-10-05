using NetCord.Rest;

namespace NetCord;

public class CurrentUser : User
{
    public CurrentUser(JsonModels.JsonUser jsonModel, RestClient client) : base(jsonModel, client)
    {
    }

    #region User
    public Task<CurrentUser> ModifyAsync(Action<CurrentUserOptions> action, RequestProperties? properties = null) => _client.ModifyCurrentUserAsync(action, properties);
    #endregion
}
