namespace NetCord.Rest;

public class RestAuditLogEntryData
{
    public RestAuditLogEntryData(JsonModels.JsonAuditLog jsonModel, RestClient client)
    {
        ApplicationCommands = jsonModel.ApplicationCommands.ToDictionary(c => c.Id, c => new ApplicationCommand(c, client));
        AutoModerationRules = jsonModel.AutoModerationRules.ToDictionary(c => c.Id, r => new AutoModerationRule(r, client));
        GuildScheduledEvents = jsonModel.GuildScheduledEvents.ToDictionary(c => c.Id, e => new GuildScheduledEvent(e, client));
        Integrations = jsonModel.Integrations.ToDictionary(c => c.Id, e => new Integration(e, client));
        Threads = jsonModel.Threads.ToDictionary(c => c.Id, t => GuildThread.CreateFromJson(t, client));
        Users = jsonModel.Users.ToDictionary(c => c.Id, u => new User(u, client));
        Webhooks = jsonModel.Webhooks.ToDictionary(c => c.Id, w => Webhook.CreateFromJson(w, client));
    }

    /// <summary>
    /// List of application commands referenced in the audit log.
    /// </summary>
    public IReadOnlyDictionary<ulong, ApplicationCommand> ApplicationCommands { get; }

    /// <summary>
    /// List of auto moderation rules referenced in the audit log.
    /// </summary>
    public IReadOnlyDictionary<ulong, AutoModerationRule> AutoModerationRules { get; }

    /// <summary>
    /// List of guild scheduled events referenced in the audit log.
    /// </summary>
    public IReadOnlyDictionary<ulong, GuildScheduledEvent> GuildScheduledEvents { get; }

    /// <summary>
    /// List of integration objects.
    /// </summary>
    public IReadOnlyDictionary<ulong, Integration> Integrations { get; }

    /// <summary>
    /// List of threads referenced in the audit log
    /// </summary>
    public IReadOnlyDictionary<ulong, GuildThread> Threads { get; }

    /// <summary>
    /// List of users referenced in the audit log.
    /// </summary>
    public IReadOnlyDictionary<ulong, User> Users { get; }

    /// <summary>
    /// List of webhooks referenced in the audit log.
    /// </summary>
    public IReadOnlyDictionary<ulong, Webhook> Webhooks { get; }
}
