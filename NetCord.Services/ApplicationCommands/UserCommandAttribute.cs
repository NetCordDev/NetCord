namespace NetCord.Services.ApplicationCommands;

/// <inheritdoc cref="Rest.UserCommandProperties" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class UserCommandAttribute(string name) : ApplicationCommandAttribute(name)
{
}
