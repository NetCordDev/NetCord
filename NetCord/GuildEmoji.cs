using NetCord.JsonModels;

namespace NetCord;

public class GuildEmoji : Emoji
{
    private protected readonly RestClient _client;

    internal GuildEmoji(JsonEmoji jsonEntity, Snowflake guildId, RestClient client) : base(jsonEntity)
    {
        _client = client;
        if (jsonEntity.Creator != null)
            Creator = new(jsonEntity.Creator, client);
        if (jsonEntity.AllowedRoles != null)
            AllowedRoles = jsonEntity.AllowedRoles.ToDictionary(r => r.Id, r => new GuildRole(r, client));
        GuildId = guildId;
    }

    public Snowflake Id => _jsonEntity.Id.GetValueOrDefault();

    public IReadOnlyDictionary<Snowflake, GuildRole>? AllowedRoles { get; }

    public User? Creator { get; }

    public bool? RequireColons => _jsonEntity.RequireColons;

    public bool? Managed => _jsonEntity.Managed;

    public bool? Available => _jsonEntity.Available;

    public Snowflake GuildId { get; }

    public override string ToString() => Animated ? $"<a:{Name}:{Id}>" : $"<:{Name}:{Id}>";

    public Task<GuildEmoji> ModifyAsync(Action<GuildEmojiOptions> action, RequestProperties? options = null) => _client.ModifyGuildEmojiAsync(GuildId, Id, action, options);

    public Task DeleteAsync(RequestProperties? options = null) => _client.DeleteGuildEmojiAsync(GuildId, Id, options);
}
