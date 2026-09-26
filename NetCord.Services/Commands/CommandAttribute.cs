namespace NetCord.Services.Commands;

/// <summary>
/// Commands are text-based commands that can be invoked by users in a chat by sending a message, typically starting with a prefix.
/// </summary>
/// <param name="aliases"><inheritdoc cref="Aliases" path="/summary" /></param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class CommandAttribute(params string[] aliases) : Attribute
{
    /// <summary>
    /// Aliases of the command.
    /// </summary>
    public string[] Aliases { get; } = aliases;

    /// <summary>
    /// Priority of the command.
    /// Commands are matched in order of descending priority, and the first command that matches the input is executed.
    /// Higher values indicate higher priority.
    /// </summary>
    public int Priority { get; init; }
}
