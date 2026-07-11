using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a guild thread of a unresolved type.
/// </summary>
internal partial class UnknownGuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : GuildThread(jsonModel, client), IUnknownGuildThread
{
    public ChannelType Type => _jsonModel.Type;
}
