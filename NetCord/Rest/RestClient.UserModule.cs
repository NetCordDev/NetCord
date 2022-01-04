using System.Text.Json.Serialization;

namespace NetCord;

public partial class RestClient
{
    public class UserModule
    {
        private readonly RestClient _client;

        internal UserModule(RestClient client)
        {
            _client = client;
        }

        public async Task<User> GetAsync(DiscordId userId, RequestOptions? options = null)
            => new((await _client.SendRequestAsync(HttpMethod.Get, $"/users/{userId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonUser>(), _client);

        public async Task<SelfUser> ModifyAsync(Action<SelfUserProperties> action, RequestOptions? options = null)
        {
            SelfUserProperties properties = new();
            action.Invoke(properties);
            var result = (await _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(properties), $"/users/@me", options).ConfigureAwait(false))!;
            return new(result.ToObject<JsonModels.JsonUser>(), _client);
        }
    }
}

public class SelfUserProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("avatar")]
    public Image? Avatar { get; set; }
}