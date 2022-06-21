namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<Snowflake, AutoModerationRule>> GetAutoModerationRulesForGuildAsync(Snowflake guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/auto-moderation/rules", options).ConfigureAwait(false)).ToObject<JsonModels.JsonAutoModerationRule[]>().ToDictionary(r => r.Id, r => new AutoModerationRule(r));

    public async Task<AutoModerationRule> GetAutoModerationRuleAsync(Snowflake guildId, Snowflake autoModerationRuleId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/auto-moderation/rules/{autoModerationRuleId}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonAutoModerationRule>());

    public async Task<AutoModerationRule> CreateAutoModerationRuleAsync(Snowflake guildId, AutoModerationRuleProperties autoModerationRuleProperties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(autoModerationRuleProperties), $"/guilds/{guildId}/auto-moderation/rules", options).ConfigureAwait(false)).ToObject<JsonModels.JsonAutoModerationRule>());

    public async Task<AutoModerationRule> ModifyAutoModerationRuleAsync(Snowflake guildId, Snowflake autoModerationRuleId, Action<AutoModerationRuleOptions> action, RequestProperties? options = null)
    {
        AutoModerationRuleOptions autoModerationRuleOptions = new();
        action(autoModerationRuleOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(autoModerationRuleOptions), $"/guilds/{guildId}/auto-moderation/rules/{autoModerationRuleId}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonAutoModerationRule>());
    }

    public Task DeleteAutoModerationRuleAsync(Snowflake guildId, Snowflake autoModerationRuleId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/auto-moderation/rules/{autoModerationRuleId}", options);
}