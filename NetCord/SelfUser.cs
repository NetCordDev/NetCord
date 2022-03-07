namespace NetCord;

public class SelfUser : User
{
    public Task<SelfUser> ModifyAsync(Action<SelfUserProperties> action, RequestProperties? options = null)
        => _client.ModifyCurrentUserAsync(action, options);

    internal SelfUser(JsonModels.JsonUser jsonEntity, RestClient client) : base(jsonEntity, client)
    {
    }
}