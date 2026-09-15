using System.Text.Json;
using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents the base event arguments received via a Discord webhook.
/// </summary>
public abstract class WebhookEventArgs(JsonWebhookEventArgs jsonModel) : IWebhookEventArgs
{
    JsonWebhookEventArgs IJsonModel<JsonWebhookEventArgs>.JsonModel => jsonModel;
    
    // Pole chronione zostaje, ponieważ klasa pochodna UnknownEventWebhookEventArgs się do niego odwołuje
    private protected readonly JsonWebhookEventArgs _jsonModel = jsonModel;

    public int Version => jsonModel.Version;

    public ulong ApplicationId => jsonModel.ApplicationId;

    public string Type => jsonModel.Event!.Type;

    public DateTimeOffset Timestamp => jsonModel.Event!.Timestamp;

    public static WebhookEventArgs CreateFromJson(JsonWebhookEventArgs jsonModel, RestClient client)
    {
        return jsonModel.Event!.Type switch
        {
            "APPLICATION_AUTHORIZED"   => new ApplicationAuthorizedWebhookEventArgs(jsonModel, client),
            "APPLICATION_DEAUTHORIZED" => new ApplicationDeauthorizedWebhookEventArgs(jsonModel, client),
            "ENTITLEMENT_CREATE"       => new EntitlementCreateWebhookEventArgs(jsonModel, client),
            _                          => new UnknownEventWebhookEventArgs(jsonModel),
        };
    }
}

/// <summary>
/// Represents the event arguments when an application is authorized by a user or guild.
/// </summary>
public class ApplicationAuthorizedWebhookEventArgs : WebhookEventArgs
{
    private readonly JsonApplicationAuthorizedWebhookEventData _eventData;

    public ApplicationAuthorizedWebhookEventArgs(JsonWebhookEventArgs jsonModel, RestClient client) : base(jsonModel)
    {
        var eventData = _eventData = jsonModel.Event!.Data.ToObject(Serialization.Default.JsonApplicationAuthorizedWebhookEventData)!;

        User = new User(eventData.User, client);

        // Pattern matching eliminujący double reference do guild
        if (eventData.Guild is { } guild)
            Guild = new RestGuild(guild, client);
    }

    public ApplicationIntegrationType? IntegrationType => _eventData.IntegrationType;

    public User User { get; }

    public IReadOnlyList<string> Scopes => _eventData.Scopes;

    public RestGuild? Guild { get; }
}

/// <summary>
/// Represents the event arguments when an application is deauthorized by a user.
/// </summary>
public class ApplicationDeauthorizedWebhookEventArgs : WebhookEventArgs
{
    private readonly JsonApplicationDeauthorizedWebhookEventData _eventData;

    public ApplicationDeauthorizedWebhookEventArgs(JsonWebhookEventArgs jsonModel, RestClient client) : base(jsonModel)
    {
        var eventData = _eventData = jsonModel.Event!.Data.ToObject(Serialization.Default.JsonApplicationDeauthorizedWebhookEventData)!;

        User = new User(eventData.User, client);
    }

    public User User { get; }
}

/// <summary>
/// Represents the event arguments when a new entitlement (subscription/purchase) is created.
/// </summary>
public class EntitlementCreateWebhookEventArgs(JsonWebhookEventArgs jsonModel, RestClient client) : WebhookEventArgs(jsonModel)
{
    public Entitlement Entitlement { get; } = new(jsonModel.Event!.Data.ToObject(Serialization.Default.JsonEntitlement)!, client);
}

/// <summary>
/// Represents webhook event arguments for an unrecognized or unhandled event type.
/// </summary>
public class UnknownEventWebhookEventArgs(JsonWebhookEventArgs jsonModel) : WebhookEventArgs(jsonModel)
{
    public JsonElement Data => _jsonModel.Event!.Data;
}
