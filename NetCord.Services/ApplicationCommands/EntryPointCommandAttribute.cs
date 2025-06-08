namespace NetCord.Services.ApplicationCommands;

/// <inheritdoc cref="Rest.EntryPointCommandProperties" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class EntryPointCommandAttribute(string name, string description) : ApplicationCommandAttribute(name)
{
    /// <inheritdoc cref="Rest.EntryPointCommandProperties.Description" />
    public string Description { get; } = description;
}
