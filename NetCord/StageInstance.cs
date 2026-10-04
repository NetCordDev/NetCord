using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class StageInstance(JsonStageInstance jsonModel, RestClient client) : ClientEntity(client)
{
    public override ulong Id { get; } = jsonModel.ChannelId;

    public ulong GuildId { get; } = jsonModel.GuildId;

    public ulong ChannelId { get; } = jsonModel.ChannelId;

    public string Topic { get; } = jsonModel.Topic;

    public StageInstancePrivacyLevel PrivacyLevel { get; } = jsonModel.PrivacyLevel;

    public bool DiscoverableDisabled { get; } = jsonModel.DiscoverableDisabled;

    public ulong? GuildScheduledEventId { get; } = jsonModel.GuildScheduledEventId;
}
