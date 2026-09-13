using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a text channel.
/// </summary>
internal abstract partial class TextChannel(JsonChannel jsonModel, RestClient client) : TextChannelBase(jsonModel, client), IPinnableChannel
{
    /// <inheritdoc cref="IPinnableChannel.LastPin" path="/summary" />
    public DateTimeOffset? LastPin => _jsonModel.LastPin;
}
