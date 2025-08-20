using System.Text.Json;

namespace NetCord.Rest;

/// <summary>
/// Message commands are application commands that appear on the context menu (right click or tap) of messages.
/// They are a great way to surface quick actions for your app that target messages.
/// </summary>
/// <param name="name"><inheritdoc cref="ApplicationCommandProperties.Name" path="/summary" /></param>
[GenerateMethodsForProperties]
public partial class MessageCommandProperties(string name) : ApplicationCommandProperties(ApplicationCommandType.Message, name)
{
    public override void WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.MessageCommandProperties);
    }
}
