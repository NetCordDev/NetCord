namespace NetCord.Rest;

/// <summary>
/// Message commands are application commands that appear on the context menu (right click or tap) of messages.
/// They're a great way to surface quick actions for your app that target messages.
/// </summary>
/// <param name="name"><inheritdoc cref="ApplicationCommandProperties.Name" path="/summary" /></param>
public partial class MessageCommandProperties(string name) : ApplicationCommandProperties(ApplicationCommandType.Message, name)
{
}
