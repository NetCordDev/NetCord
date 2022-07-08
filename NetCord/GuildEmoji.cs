using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class GuildEmoji : Emoji
{
    private protected readonly RestClient _client;

    public GuildEmoji(JsonEmoji jsonModel, Snowflake guildId, RestClient client) : base(jsonModel)
    {
        _client = client;
        if (jsonModel.Creator != null)
            Creator = new(jsonModel.Creator, client);
        if (jsonModel.AllowedRoles != null)
            AllowedRoles = jsonModel.AllowedRoles.ToDictionary(r => r.Id, r => new GuildRole(r, client));
        GuildId = guildId;
    }

    public Snowflake Id => _jsonModel.Id.GetValueOrDefault();

    public IReadOnlyDictionary<Snowflake, GuildRole>? AllowedRoles { get; }

    public User? Creator { get; }

    public bool? RequireColons => _jsonModel.RequireColons;

    public bool? Managed => _jsonModel.Managed;

    public bool? Available => _jsonModel.Available;

    public Snowflake GuildId { get; }

    public override string ToString() => Animated ? $"<a:{Name}:{Id}>" : $"<:{Name}:{Id}>";

    public Task<GuildEmoji> ModifyAsync(Action<GuildEmojiOptions> action, RequestProperties? properties = null) => _client.ModifyGuildEmojiAsync(GuildId, Id, action, properties);

    public Task DeleteAsync(RequestProperties? properties = null) => _client.DeleteGuildEmojiAsync(GuildId, Id, properties);
}
