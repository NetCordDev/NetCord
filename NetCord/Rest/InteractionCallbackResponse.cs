using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class InteractionCallbackResponse(JsonInteractionCallbackResponse jsonModel, RestClient client)
{
    public InteractionCallbackResponseInteraction Interaction { get; } = new(jsonModel.Interaction);

    public InteractionCallbackResponseResource? Resource { get; } = jsonModel.Resource is { } resource ? new(resource, client) : null;
}

public class InteractionCallbackResponseInteraction(JsonInteractionCallbackResponseInteraction jsonModel) : Entity
{
    public override ulong Id { get; } = jsonModel.Id;

    public InteractionType Type { get; } = jsonModel.Type;

    public string? ActivityInstanceId { get; } = jsonModel.ActivityInstanceId;

    public ulong? ResponseMessageId { get; } = jsonModel.ResponseMessageId;

    public bool? ResponseMessageLoading { get; } = jsonModel.ResponseMessageLoading;

    public bool? ResponseMessageEphemeral { get; } = jsonModel.ResponseMessageEphemeral;
}

public class InteractionCallbackResponseResource(JsonInteractionCallbackResponseResource jsonModel, RestClient client)
{
    public InteractionCallbackType Type { get; } = jsonModel.Type;

    public ActivityInstance? ActivityInstance { get; } = jsonModel.ActivityInstance is { } activityInstance ? new(activityInstance) : null;

    public RestMessage? Message { get; } = jsonModel.Message is { } message ? new(message, client) : null;
}

public class ActivityInstance(JsonActivityInstance jsonModel)
{
    public string Id { get; } = jsonModel.Id;
}
