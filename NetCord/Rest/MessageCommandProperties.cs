namespace NetCord.Rest;

/// <summary>
/// 
/// </summary>
/// <param name="name">Name of the command (1-32 characters).</param>
public partial class MessageCommandProperties(string name) : ApplicationCommandProperties(ApplicationCommandType.Message, name)
{
}
