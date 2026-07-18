using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <inheritdoc cref="IUnknownTextChannel"/>
internal partial class UnknownTextChannel(JsonChannel jsonModel, RestClient client) : TextChannel(jsonModel, client), IUnknownTextChannel
{
    public ChannelType Type => _jsonModel.Type;
}
