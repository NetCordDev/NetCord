using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyDictionary<ulong, GuildEmoji>> GetGuildEmojisAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/emojis", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEmojiArray).ConfigureAwait(false)).ToDictionary(e => e.Id.GetValueOrDefault(), e => new GuildEmoji(e, guildId, this));

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildEmoji)], nameof(GuildEmoji.GuildId), nameof(GuildEmoji.Id))]
    public async Task<GuildEmoji> GetGuildEmojiAsync(ulong guildId, ulong emojiId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/emojis/{emojiId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEmoji).ConfigureAwait(false), guildId, this);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildEmoji> CreateGuildEmojiAsync(ulong guildId, GuildEmojiProperties guildEmojiProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildEmojiProperties>(guildEmojiProperties, Serialization.Default.GuildEmojiProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/emojis", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEmoji).ConfigureAwait(false), guildId, this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildEmoji)], nameof(GuildEmoji.GuildId), nameof(GuildEmoji.Id))]
    public async Task<GuildEmoji> ModifyGuildEmojiAsync(ulong guildId, ulong emojiId, Action<GuildEmojiOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        GuildEmojiOptions guildEmojiOptions = new();
        action(guildEmojiOptions);
        using (HttpContent content = new JsonContent<GuildEmojiOptions>(guildEmojiOptions, Serialization.Default.GuildEmojiOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/emojis/{emojiId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEmoji).ConfigureAwait(false), guildId, this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildEmoji)], nameof(GuildEmoji.GuildId), nameof(GuildEmoji.Id))]
    public Task DeleteGuildEmojiAsync(ulong guildId, ulong emojiId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/emojis/{emojiId}", null, new(guildId), properties, cancellationToken: cancellationToken);

    [GenerateAlias([typeof(Application)], nameof(Application.Id))]
    public async Task<IReadOnlyDictionary<ulong, ApplicationEmoji>> GetApplicationEmojisAsync(ulong applicationId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/emojis", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEmojiArray).ConfigureAwait(false)).ToDictionary(e => e.Id.GetValueOrDefault(), e => new ApplicationEmoji(e, applicationId, this));

    [GenerateAlias([typeof(Application)], nameof(Application.Id))]
    [GenerateAlias([typeof(ApplicationEmoji)], nameof(ApplicationEmoji.ApplicationId), nameof(ApplicationEmoji.Id))]
    public async Task<ApplicationEmoji> GetApplicationEmojiAsync(ulong applicationId, ulong emojiId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/emojis/{emojiId}", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEmoji).ConfigureAwait(false), applicationId, this);

    [GenerateAlias([typeof(Application)], nameof(Application.Id))]
    public async Task<ApplicationEmoji> CreateApplicationEmojiAsync(ulong applicationId, ApplicationEmojiProperties applicationEmojiProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<ApplicationEmojiProperties>(applicationEmojiProperties, Serialization.Default.ApplicationEmojiProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/applications/{applicationId}/emojis", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEmoji).ConfigureAwait(false), applicationId, this);
    }

    [GenerateAlias([typeof(Application)], nameof(Application.Id))]
    [GenerateAlias([typeof(ApplicationEmoji)], nameof(ApplicationEmoji.ApplicationId), nameof(ApplicationEmoji.Id))]
    public async Task<ApplicationEmoji> ModifyApplicationEmojiAsync(ulong applicationId, ulong emojiId, Action<ApplicationEmojiOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        ApplicationEmojiOptions applicationEmojiOptions = new();
        action(applicationEmojiOptions);
        using (HttpContent content = new JsonContent<ApplicationEmojiOptions>(applicationEmojiOptions, Serialization.Default.ApplicationEmojiOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/applications/{applicationId}/emojis/{emojiId}", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEmoji).ConfigureAwait(false), applicationId, this);
    }

    [GenerateAlias([typeof(Application)], nameof(Application.Id))]
    [GenerateAlias([typeof(ApplicationEmoji)], nameof(ApplicationEmoji.ApplicationId), nameof(ApplicationEmoji.Id))]
    public Task DeleteApplicationEmojiAsync(ulong applicationId, ulong emojiId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/emojis/{emojiId}", null, null, properties, cancellationToken: cancellationToken);
}
