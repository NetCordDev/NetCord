namespace NetCord;

public class SelfUser : User
{
    public Task<SelfUser> ModifyAsync(Action<SelfUserProperties> action, RequestOptions? options = null)
        => _client.User.ModifyAsync(action, options);

    internal SelfUser(JsonModels.JsonUser jsonEntity, RestClient client) : base(jsonEntity, client)
    {
    }
}