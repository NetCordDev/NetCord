using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Services.Commands;

/// <summary>
/// Context for handling text-based commands.
/// </summary>
public interface ICommandContext : IRestMessageContext
{
    /// <summary>
    /// The message that triggered the command.
    /// </summary>
    public new Message Message { get; }

    RestMessage IRestMessageContext.Message => Message;
}
