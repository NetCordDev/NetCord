namespace NetCord.Services.ApplicationCommands;

/// <inheritdoc cref="Rest.MessageCommandProperties" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class MessageCommandAttribute(string name) : ApplicationCommandAttribute(name)
{
}
