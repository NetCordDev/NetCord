namespace NetCord.Services.ApplicationCommands;

/// <inheritdoc cref="Rest.UserCommandProperties" />
public class UserCommandAttribute(string name) : ApplicationCommandAttribute(name)
{
}
