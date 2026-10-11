using System.Text.Json;

using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public abstract class WebhookEventArgs(JsonWebhookEventArgs jsonModel) : IWebhookEventArgs
{
    public int Version { get; } = jsonModel.Version;

    public ulong ApplicationId { get; } = jsonModel.ApplicationId;

    public string Type { get; } = jsonModel.Event!.Type;

    public DateTimeOffset Timestamp { get; } = jsonModel.Event.Timestamp;

    public static WebhookEventArgs Create(JsonWebhookEventArgs jsonModel, RestClient client)
    {
        return jsonModel.Event!.Type switch
        {
            "APPLICATION_AUTHORIZED" => new ApplicationAuthorizedWebhookEventArgs(jsonModel, client),
            "APPLICATION_DEAUTHORIZED" => new ApplicationDeauthorizedWebhookEventArgs(jsonModel, client),
            "ENTITLEMENT_CREATE" => new EntitlementCreateWebhookEventArgs(jsonModel, client),
            _ => new UnknownEventWebhookEventArgs(jsonModel),
        };
    }
}

public class ApplicationAuthorizedWebhookEventArgs : WebhookEventArgs
{
    public ApplicationAuthorizedWebhookEventArgs(JsonWebhookEventArgs jsonModel, RestClient client) : base(jsonModel)
    {
        var eventData = jsonModel.Event!.Data.ToObject(Serialization.Default.JsonApplicationAuthorizedWebhookEventData)!;

        IntegrationType = eventData.IntegrationType;

        User = new(eventData.User, client);

        Scopes = eventData.Scopes;

        var guild = eventData.Guild;
        if (guild is not null)
            Guild = new(guild, client);
    }

    public ApplicationIntegrationType? IntegrationType { get; }

    public User User { get; }

    public IReadOnlyList<string> Scopes { get; }

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
    public JsonElement Data { get; } = jsonModel.Event!.Data;
}
