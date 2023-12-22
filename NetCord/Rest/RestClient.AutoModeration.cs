namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<ulong, AutoModerationRule>> GetAutoModerationRulesAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/auto-moderation/rules", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonAutoModerationRuleArray).ConfigureAwait(false)).ToDictionary(r => r.Id, r => new AutoModerationRule(r, this));

    public async Task<AutoModerationRule> GetAutoModerationRuleAsync(ulong guildId, ulong autoModerationRuleId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/auto-moderation/rules/{autoModerationRuleId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonAutoModerationRule).ConfigureAwait(false), this);

    public async Task<AutoModerationRule> CreateAutoModerationRuleAsync(ulong guildId, AutoModerationRuleProperties autoModerationRuleProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<AutoModerationRuleProperties>(autoModerationRuleProperties, Serialization.Default.AutoModerationRuleProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/auto-moderation/rules", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonAutoModerationRule).ConfigureAwait(false), this);
    }

    public async Task<AutoModerationRule> ModifyAutoModerationRuleAsync(ulong guildId, ulong autoModerationRuleId, Action<AutoModerationRuleOptions> action, RequestProperties? properties = null)
    {
        AutoModerationRuleOptions autoModerationRuleOptions = new();
        action(autoModerationRuleOptions);
        using (HttpContent content = new JsonContent<AutoModerationRuleOptions>(autoModerationRuleOptions, Serialization.Default.AutoModerationRuleOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/auto-moderation/rules/{autoModerationRuleId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonAutoModerationRule).ConfigureAwait(false), this);
    }

    public Task DeleteAutoModerationRuleAsync(ulong guildId, ulong autoModerationRuleId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/auto-moderation/rules/{autoModerationRuleId}", null, new(guildId), properties);
}
