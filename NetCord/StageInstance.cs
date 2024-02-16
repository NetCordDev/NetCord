using NetCord.Rest;

namespace NetCord;

public partial class StageInstance(JsonModels.JsonStageInstance jsonModel, RestClient client) : ClientEntity(client), IJsonModel<JsonModels.JsonStageInstance>
{
    JsonModels.JsonStageInstance IJsonModel<JsonModels.JsonStageInstance>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;
    public ulong GuildId => jsonModel.GuildId;
    public ulong ChannelId => jsonModel.ChannelId;
    public string Topic => jsonModel.Topic;
    public StageInstancePrivacyLevel PrivacyLevel => jsonModel.PrivacyLevel;
    public bool DiscoverableDisabled => jsonModel.DiscoverableDisabled;
}
