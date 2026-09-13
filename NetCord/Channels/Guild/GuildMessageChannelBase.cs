using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

internal abstract partial class GuildMessageChannelBase(JsonChannel jsonModel, ulong guildId, RestClient client) : TextChannelBase(jsonModel, client), IGuildMessageChannel
{
    public ulong GuildId => guildId;
}