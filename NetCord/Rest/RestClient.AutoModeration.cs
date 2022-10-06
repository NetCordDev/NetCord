using NetCord.Rest.RateLimits;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<Snowflake, AutoModerationRule>> GetAutoModerationRulesAsync(Snowflake guildId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/auto-moderation/rules", new Route(RouteParameter.GetAutoModerationRulesForGuild, guildId), properties).ConfigureAwait(false)).ToObject<JsonModels.JsonAutoModerationRule[]>(JsonModels.JsonAutoModerationRule.JsonAutoModerationRuleArraySerializerContext.WithOptions.JsonAutoModerationRuleArray).ToDictionary(r => r.Id, r => new AutoModerationRule(r));

    public async Task<AutoModerationRule> GetAutoModerationRuleAsync(Snowflake guildId, Snowflake autoModerationRuleId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/auto-moderation/rules/{autoModerationRuleId}", new Route(RouteParameter.GetAutoModerationRule, guildId), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonAutoModerationRule.JsonAutoModerationRuleSerializerContext.WithOptions.JsonAutoModerationRule));

    public async Task<AutoModerationRule> CreateAutoModerationRuleAsync(Snowflake guildId, AutoModerationRuleProperties autoModerationRuleProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/auto-moderation/rules", new Route(RouteParameter.CreateAutoModerationRule, guildId), new JsonContent<AutoModerationRuleProperties>(autoModerationRuleProperties, AutoModerationRuleProperties.AutoModerationRulePropertiesSerializerContext.WithOptions.AutoModerationRuleProperties), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonAutoModerationRule.JsonAutoModerationRuleSerializerContext.WithOptions.JsonAutoModerationRule));

    public async Task<AutoModerationRule> ModifyAutoModerationRuleAsync(Snowflake guildId, Snowflake autoModerationRuleId, Action<AutoModerationRuleOptions> action, RequestProperties? properties = null)
    {
        AutoModerationRuleOptions autoModerationRuleOptions = new();
        action(autoModerationRuleOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/auto-moderation/rules/{autoModerationRuleId}", new Route(RouteParameter.ModifyAutoModerationRule, guildId), new JsonContent<AutoModerationRuleOptions>(autoModerationRuleOptions, AutoModerationRuleOptions.AutoModerationRuleOptionsSerializerContext.WithOptions.AutoModerationRuleOptions), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonAutoModerationRule.JsonAutoModerationRuleSerializerContext.WithOptions.JsonAutoModerationRule));
    }

    public Task DeleteAutoModerationRuleAsync(Snowflake guildId, Snowflake autoModerationRuleId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/auto-moderation/rules/{autoModerationRuleId}", new Route(RouteParameter.DeleteAutoModerationRule, guildId), properties);
}
