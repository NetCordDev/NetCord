using System.Text.Json;

using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public abstract class WebhookEventArgs(JsonWebhookEventArgs jsonModel) : IWebhookEventArgs
{
    JsonWebhookEventArgs IJsonModel<JsonWebhookEventArgs>.JsonModel => _jsonModel;
    private protected readonly JsonWebhookEventArgs _jsonModel = jsonModel;

    public int Version => _jsonModel.Version;

    public ulong ApplicationId => _jsonModel.ApplicationId;

    public DateTimeOffset Timestamp => _jsonModel.Event!.Timestamp;
}

public class ApplicationAuthorizedWebhookEventArgs : WebhookEventArgs
{
    private readonly JsonApplicationAuthorizedWebhookEventData _eventData;

    public ApplicationAuthorizedWebhookEventArgs(JsonWebhookEventArgs jsonModel, RestClient client) : base(jsonModel)
    {
        var eventData = _eventData = jsonModel.Event!.Data.ToObject(Serialization.Default.JsonApplicationAuthorizedWebhookEventData)!;

        User = new(eventData.User, client);

        var guild = eventData.Guild;
        if (guild is not null)
            Guild = new(guild, client);
    }

    public ApplicationIntegrationType? IntegrationType => _eventData.IntegrationType;

    public User User { get; }

    public IReadOnlyList<string> Scopes => _eventData.Scopes;

    public RestGuild? Guild { get; }
}

public class ApplicationDeauthorizedWebhookEventArgs : WebhookEventArgs
{
    public ApplicationDeauthorizedWebhookEventArgs(JsonWebhookEventArgs jsonModel, RestClient client) : base(jsonModel)
    {
        var eventData = jsonModel.Event!.Data.ToObject(Serialization.Default.JsonApplicationDeauthorizedWebhookEventData)!;

        User = new(eventData.User, client);
    }

    public User User { get; }
}

public class EntitlementCreateWebhookEventArgs(JsonWebhookEventArgs jsonModel, RestClient client) : WebhookEventArgs(jsonModel)
{
    public Entitlement Entitlement { get; } = new(jsonModel.Event!.Data.ToObject(Serialization.Default.JsonEntitlement)!, client);
}

public class UnknownEventWebhookEventArgs(JsonWebhookEventArgs jsonModel) : WebhookEventArgs(jsonModel)
{
    public string Type => _jsonModel.Event!.Type;

    public JsonElement Data => _jsonModel.Event!.Data;
}
