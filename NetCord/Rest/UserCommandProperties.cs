namespace NetCord.Rest;

/// <summary>
/// 
/// </summary>
/// <param name="name">Name of the command (1-32 characters).</param>
public partial class UserCommandProperties(string name) : ApplicationCommandProperties(ApplicationCommandType.User, name)
{
}
