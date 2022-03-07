using System.Text.Json.Serialization;

namespace NetCord;

public partial class RestClient
{
    public async Task<User> GetUserAsync(DiscordId userId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/users/{userId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonUser>(), this);

    public async Task<SelfUser> ModifyCurrentUserAsync(Action<SelfUserProperties> action, RequestProperties? options = null)
    {
        SelfUserProperties properties = new();
        action.Invoke(properties);
        var result = (await SendRequestAsync(HttpMethod.Patch, new JsonContent(properties), $"/users/@me", options).ConfigureAwait(false))!;
        return new(result.ToObject<JsonModels.JsonUser>(), this);
    }
}

public class SelfUserProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("avatar")]
    public ImageProperties? Avatar { get; set; }
}