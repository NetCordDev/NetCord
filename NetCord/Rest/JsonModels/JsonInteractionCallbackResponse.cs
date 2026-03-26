using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonInteractionCallbackResponse
{
    [JsonPropertyName("interaction")]
    public JsonInteractionCallbackResponseInteraction Interaction { get; set; }

    [JsonPropertyName("resource")]
    public JsonInteractionCallbackResponseResource Resource { get; set; }
}

public class JsonInteractionCallbackResponseInteraction : JsonEntity
{
    [JsonPropertyName("type")]
    public InteractionType Type { get; set; }

    [JsonPropertyName("activity_instance_id")]
    public string? ActivityInstanceId { get; set; }

    [JsonPropertyName("response_message_id")]
    public ulong? ResponseMessageId { get; set; }

    [JsonPropertyName("response_message_loading")]
    public bool? ResponseMessageLoading { get; set; }

    [JsonPropertyName("response_message_ephemeral")]
    public bool? ResponseMessageEphemeral { get; set; }
}

public class JsonInteractionCallbackResponseResource
{
    [JsonPropertyName("type")]
    public InteractionCallbackType Type { get; set; }

    [JsonPropertyName("activity_instance")]
    public JsonActivityInstance? ActivityInstance { get; set; }

    [JsonPropertyName("message")]
    public JsonMessage? Message { get; set; }
}

public class JsonActivityInstance
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}
