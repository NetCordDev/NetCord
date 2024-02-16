namespace NetCord.Rest;

public class RestAuditLogEntryData(JsonModels.JsonAuditLog jsonModel, RestClient client)
{

    /// <summary>
    /// List of application commands referenced in the audit log.
    /// </summary>
    public IReadOnlyDictionary<ulong, ApplicationCommand> ApplicationCommands { get; } = jsonModel.ApplicationCommands.ToDictionary(c => c.Id, c => new ApplicationCommand(c, client));

    /// <summary>
    /// List of auto moderation rules referenced in the audit log.
    /// </summary>
    public IReadOnlyDictionary<ulong, AutoModerationRule> AutoModerationRules { get; } = jsonModel.AutoModerationRules.ToDictionary(c => c.Id, r => new AutoModerationRule(r, client));

    /// <summary>
    /// List of guild scheduled events referenced in the audit log.
    /// </summary>
    public IReadOnlyDictionary<ulong, GuildScheduledEvent> GuildScheduledEvents { get; } = jsonModel.GuildScheduledEvents.ToDictionary(c => c.Id, e => new GuildScheduledEvent(e, client));

    /// <summary>
    /// List of integration objects.
    /// </summary>
    public IReadOnlyDictionary<ulong, Integration> Integrations { get; } = jsonModel.Integrations.ToDictionary(c => c.Id, e => new Integration(e, client));

    /// <summary>
    /// List of threads referenced in the audit log
    /// </summary>
    public IReadOnlyDictionary<ulong, GuildThread> Threads { get; } = jsonModel.Threads.ToDictionary(c => c.Id, t => GuildThread.CreateFromJson(t, client));

    /// <summary>
    /// List of users referenced in the audit log.
    /// </summary>
    public IReadOnlyDictionary<ulong, User> Users { get; } = jsonModel.Users.ToDictionary(c => c.Id, u => new User(u, client));

    /// <summary>
    /// List of webhooks referenced in the audit log.
    /// </summary>
    public IReadOnlyDictionary<ulong, Webhook> Webhooks { get; } = jsonModel.Webhooks.ToDictionary(c => c.Id, w => Webhook.CreateFromJson(w, client));
}
