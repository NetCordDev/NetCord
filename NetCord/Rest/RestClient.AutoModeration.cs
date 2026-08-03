using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    /// <summary>
    /// Retrieves a read-only list of configured automod rules for a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to retrieve auto-moderation rules for.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyList<AutoModerationRule>> GetAutoModerationRulesAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/auto-moderation/rules", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonAutoModerationRuleArray).ConfigureAwait(false)).Select(r => new AutoModerationRule(r, this)).ToArray();

    /// <summary>
    /// Retrieves information about a specific auto-mod's rule configuration, for a specified guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to retrieve auto-moderation rules for.</param>
    /// <param name="autoModerationRuleId">The ID of the rule to retrieve information on.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(AutoModerationRule)], nameof(AutoModerationRule.GuildId), nameof(AutoModerationRule.Id))]
    public async Task<AutoModerationRule> GetAutoModerationRuleAsync(ulong guildId, ulong autoModerationRuleId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/auto-moderation/rules/{autoModerationRuleId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonAutoModerationRule).ConfigureAwait(false), this);

    /// <summary>
    /// Creates a new auto-moderation rule for a specified guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to create the rule for..</param>
    /// <param name="autoModerationRuleProperties">The properties of the auto-moderation rule to be created, including triggers, actions, and conditions.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<AutoModerationRule> CreateAutoModerationRuleAsync(ulong guildId, AutoModerationRuleProperties autoModerationRuleProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<AutoModerationRuleProperties>(autoModerationRuleProperties, Serialization.Default.AutoModerationRuleProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/auto-moderation/rules", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonAutoModerationRule).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Modifies a pre-existing auto-moderation rule for a specified guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to modify the rule for..</param>
    /// <param name="autoModerationRuleId">The ID of the rule to modify.</param>
    /// <param name="action">An action, representing the modifications to make to the rule.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(AutoModerationRule)], nameof(AutoModerationRule.GuildId), nameof(AutoModerationRule.Id))]
    public async Task<AutoModerationRule> ModifyAutoModerationRuleAsync(ulong guildId, ulong autoModerationRuleId, Action<AutoModerationRuleOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        AutoModerationRuleOptions autoModerationRuleOptions = new();
        action(autoModerationRuleOptions);
        using (HttpContent content = new JsonContent<AutoModerationRuleOptions>(autoModerationRuleOptions, Serialization.Default.AutoModerationRuleOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/auto-moderation/rules/{autoModerationRuleId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonAutoModerationRule).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Deletes an auto-moderation rule for a specified guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to modify the rule for..</param>
    /// <param name="autoModerationRuleId">The ID of the rule to modify.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(AutoModerationRule)], nameof(AutoModerationRule.GuildId), nameof(AutoModerationRule.Id))]
    public Task DeleteAutoModerationRuleAsync(ulong guildId, ulong autoModerationRuleId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/auto-moderation/rules/{autoModerationRuleId}", null, new(guildId), properties, cancellationToken: cancellationToken);
}
