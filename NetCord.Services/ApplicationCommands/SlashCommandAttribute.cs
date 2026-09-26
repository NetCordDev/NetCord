namespace NetCord.Services.ApplicationCommands;

/// <inheritdoc cref="Rest.SlashCommandProperties" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class SlashCommandAttribute(string name, string description) : ApplicationCommandAttribute(name)
{
    /// <inheritdoc cref="Rest.SlashCommandProperties.Description" />
    public string Description { get; } = description;
}
