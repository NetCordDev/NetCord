using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents the response to an interaction callback.
/// </summary>
public class InteractionCallbackResponse(JsonInteractionCallbackResponse jsonModel, RestClient client) : IJsonModel<JsonInteractionCallbackResponse>
{
    JsonInteractionCallbackResponse IJsonModel<JsonInteractionCallbackResponse>.JsonModel => jsonModel;

    /// <summary>
    /// Interaction object associated with the callback.
    /// </summary>
    public InteractionCallbackResponseInteraction Interaction { get; } = new(jsonModel.Interaction);

    /// <summary>
    /// Resource created by the interaction callback.
    /// </summary>
    public InteractionCallbackResponseResource Resource { get; } = new(jsonModel.Resource, client);
}

/// <summary>
/// Represents interaction metadata within an interaction callback response.
/// </summary>
public class InteractionCallbackResponseInteraction(JsonInteractionCallbackResponseInteraction jsonModel) : Entity, IJsonModel<JsonInteractionCallbackResponseInteraction>
{
    JsonInteractionCallbackResponseInteraction IJsonModel<JsonInteractionCallbackResponseInteraction>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// Type of the interaction.
    /// </summary>
    public InteractionType Type => jsonModel.Type;

    /// <summary>
    /// ID of the activity instance.
    /// </summary>
    public string? ActivityInstanceId => jsonModel.ActivityInstanceId;

    /// <summary>
    /// ID of the response message.
    /// </summary>
    public ulong? ResponseMessageId => jsonModel.ResponseMessageId;

    /// <summary>
    /// Whether the response message is currently loading.
    /// </summary>
    public bool? ResponseMessageLoading => jsonModel.ResponseMessageLoading;

    /// <summary>
    /// Whether the response message is ephemeral.
    /// </summary>
    public bool? ResponseMessageEphemeral => jsonModel.ResponseMessageEphemeral;
}

/// <summary>
/// Represents resource details returned in an interaction callback response.
/// </summary>
public class InteractionCallbackResponseResource(JsonInteractionCallbackResponseResource jsonModel, RestClient client) : IJsonModel<JsonInteractionCallbackResponseResource>
{
    JsonInteractionCallbackResponseResource IJsonModel<JsonInteractionCallbackResponseResource>.JsonModel => jsonModel;

    /// <summary>
    /// Interaction callback type of the resource.
    /// </summary>
    public InteractionCallbackType Type => jsonModel.Type;

    /// <summary>
    /// Activity instance resource, if applicable.
    /// </summary>
    public ActivityInstance? ActivityInstance { get; } = jsonModel.ActivityInstance is { } activityInstance ? new(activityInstance) : null;

    /// <summary>
    /// Message resource created or modified by the callback.
    /// </summary>
    public RestMessage? Message { get; } = jsonModel.Message is { } message ? new(message, client) : null;
}

/// <summary>
/// Represents an activity instance within Discord.
/// </summary>
public class ActivityInstance(JsonActivityInstance jsonModel) : IJsonModel<JsonActivityInstance>
{
    JsonActivityInstance IJsonModel<JsonActivityInstance>.JsonModel => jsonModel;

    /// <summary>
    /// Unique identifier for the activity instance.
    /// </summary>
    public string Id => jsonModel.Id;
}
