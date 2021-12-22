using System.Text.Json.Serialization;

namespace NetCord
{
    internal class InteractionCallback
    {
        [JsonPropertyName("type")]
        public InteractionCallbackType Type { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("data")]
        public InteractionMessage? Message { get; }

        public InteractionCallback(InteractionCallbackType type)
        {
            Type = type;
        }

        public InteractionCallback(InteractionCallbackType type, InteractionMessage? message)
        {
            Type = type;
            Message = message;
        }

        internal MultipartFormDataContent Build()
        {
            MultipartFormDataContent content = new();
            content.Add(new JsonContent(this), "payload_json");
            if (Message != null)
            {
                var attachments = Message.Attachments;
                if (attachments != null)
                {
                    var count = attachments.Count;
                    for (var i = 0; i < count; i++)
                    {
                        MessageAttachment attachment = attachments[i];
                        content.Add(new StreamContent(attachment.Stream), $"files[{i}]", attachment.FileName);
                    }
                }
            }
            return content;
        }
    }
}
