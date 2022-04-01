namespace NetCord;

public class SelfUser : User
{
    internal SelfUser(JsonModels.JsonUser jsonEntity, RestClient client) : base(jsonEntity, client)
    {
    }

    public Task<SelfUser> ModifyAsync(Action<SelfUserProperties> action, RequestProperties? options = null)
        => _client.ModifyCurrentUserAsync(action, options);
}