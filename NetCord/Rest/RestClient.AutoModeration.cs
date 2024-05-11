using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyDictionary<ulong, AutoModerationRule>> GetAutoModerationRulesAsync(ulong guildId, RestRequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/auto-moderation/rules", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonAutoModerationRuleArray).ConfigureAwait(false)).ToDictionary(r => r.Id, r => new AutoModerationRule(r, this));

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(AutoModerationRule)], nameof(AutoModerationRule.GuildId), nameof(AutoModerationRule.Id))]
    public async Task<AutoModerationRule> GetAutoModerationRuleAsync(ulong guildId, ulong autoModerationRuleId, RestRequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/auto-moderation/rules/{autoModerationRuleId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonAutoModerationRule).ConfigureAwait(false), this);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<AutoModerationRule> CreateAutoModerationRuleAsync(ulong guildId, AutoModerationRuleProperties autoModerationRuleProperties, RestRequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<AutoModerationRuleProperties>(autoModerationRuleProperties, Serialization.Default.AutoModerationRuleProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/auto-moderation/rules", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonAutoModerationRule).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(AutoModerationRule)], nameof(AutoModerationRule.GuildId), nameof(AutoModerationRule.Id))]
    public async Task<AutoModerationRule> ModifyAutoModerationRuleAsync(ulong guildId, ulong autoModerationRuleId, Action<AutoModerationRuleOptions> action, RestRequestProperties? properties = null)
    {
        AutoModerationRuleOptions autoModerationRuleOptions = new();
        action(autoModerationRuleOptions);
        using (HttpContent content = new JsonContent<AutoModerationRuleOptions>(autoModerationRuleOptions, Serialization.Default.AutoModerationRuleOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/auto-moderation/rules/{autoModerationRuleId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonAutoModerationRule).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(AutoModerationRule)], nameof(AutoModerationRule.GuildId), nameof(AutoModerationRule.Id))]
    public Task DeleteAutoModerationRuleAsync(ulong guildId, ulong autoModerationRuleId, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/auto-moderation/rules/{autoModerationRuleId}", null, new(guildId), properties);
}
