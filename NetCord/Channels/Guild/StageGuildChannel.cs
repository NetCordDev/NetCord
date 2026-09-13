using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a stage channel within a guild.
/// </summary>
internal partial class StageGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : VoiceGuildChannel(jsonModel, guildId, client)
{
}
