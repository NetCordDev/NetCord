using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

internal abstract partial class TextChannelBase(JsonChannel jsonModel, RestClient client) : Channel(jsonModel, client), ITextChannel
{
    /// <inheritdoc cref="ITextChannel.LastMessageId" path="/summary" />
    public ulong? LastMessageId => throw new NotImplementedException();
}