using NetCord.Rest;

namespace NetCord;

public partial class StageInstance : ClientEntity, IJsonModel<JsonModels.JsonStageInstance>
{
    JsonModels.JsonStageInstance IJsonModel<JsonModels.JsonStageInstance>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonStageInstance _jsonModel;

    public override ulong Id => _jsonModel.Id;
    public ulong GuildId => _jsonModel.GuildId;
    public ulong ChannelId => _jsonModel.ChannelId;
    public string Topic => _jsonModel.Topic;
    public StageInstancePrivacyLevel PrivacyLevel => _jsonModel.PrivacyLevel;
    public bool DiscoverableDisabled => _jsonModel.DiscoverableDisabled;

    public StageInstance(JsonModels.JsonStageInstance jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
    }
}
