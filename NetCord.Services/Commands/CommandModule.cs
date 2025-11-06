using NetCord.Rest;

namespace NetCord.Services.Commands;

/// <summary>
/// Represents a module for commands.
/// </summary>
/// <typeparam name="TContext">The context the invoked commands use.</typeparam>
public abstract class CommandModule<TContext> : BaseCommandModule<TContext> where TContext : ICommandContext
{
    /// <inheritdoc cref="RestMessage.ReplyAsync" />
    public Task<RestMessage> ReplyAsync(ReplyMessageProperties replyMessage, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => Context.Message.ReplyAsync(replyMessage, properties, cancellationToken);

    /// <inheritdoc cref="RestMessage.SendAsync" />
    public Task<RestMessage> SendAsync(MessageProperties message, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => Context.Message.SendAsync(message, properties, cancellationToken);
}
