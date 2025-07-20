using System.Text.Json;

namespace NetCord.Rest;

/// <summary>
/// User commands are application commands that appear on the context menu (right click or tap) of users.
/// They are a great way to surface quick actions for your app that target users.
/// </summary>
/// <param name="name"><inheritdoc cref="ApplicationCommandProperties.Name" path="/summary" /></param>
public partial class UserCommandProperties(string name) : ApplicationCommandProperties(ApplicationCommandType.User, name)
{
    public override void WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.UserCommandProperties);
    }
}
