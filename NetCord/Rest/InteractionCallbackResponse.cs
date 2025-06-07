using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class InteractionCallbackResponse(JsonInteractionCallbackResponse jsonModel, RestClient client) : IJsonModel<JsonInteractionCallbackResponse>
{
    JsonInteractionCallbackResponse IJsonModel<JsonInteractionCallbackResponse>.JsonModel => jsonModel;

    public InteractionCallbackResponseInteraction Interaction { get; } = new(jsonModel.Interaction);

    public InteractionCallbackResponseResource Resource { get; } = new(jsonModel.Resource, client);
}

public class InteractionCallbackResponseInteraction(JsonInteractionCallbackResponseInteraction jsonModel) : Entity, IJsonModel<JsonInteractionCallbackResponseInteraction>
{
    JsonInteractionCallbackResponseInteraction IJsonModel<JsonInteractionCallbackResponseInteraction>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;
    public InteractionType Type => jsonModel.Type;
    public string? ActivityInstanceId => jsonModel.ActivityInstanceId;
    public ulong? ResponseMessageId => jsonModel.ResponseMessageId;
    public bool? ResponseMessageLoading => jsonModel.ResponseMessageLoading;
    public bool? ResponseMessageEphemeral => jsonModel.ResponseMessageEphemeral;
}

public class InteractionCallbackResponseResource : IJsonModel<JsonInteractionCallbackResponseResource>
{
    public InteractionCallbackResponseResource(JsonInteractionCallbackResponseResource jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        if (jsonModel.ActivityInstance is { } activityInstance)
            ActivityInstance = new ActivityInstance(activityInstance);

        if (jsonModel.Message is { } message)
            Message = new RestMessage(message, client);
    }

    JsonInteractionCallbackResponseResource IJsonModel<JsonInteractionCallbackResponseResource>.JsonModel => _jsonModel;
    private readonly JsonInteractionCallbackResponseResource _jsonModel;
    
    public InteractionCallbackType Type => _jsonModel.Type;
    public ActivityInstance? ActivityInstance { get; }
    public RestMessage? Message { get; }
}

public class ActivityInstance(JsonActivityInstance jsonModel) : IJsonModel<JsonActivityInstance>
{
    JsonActivityInstance IJsonModel<JsonActivityInstance>.JsonModel => jsonModel;
 
    public string Id => jsonModel.Id;
}
