using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class GuildTemplate : IJsonModel<JsonModels.JsonGuildTemplate>
{
    JsonModels.JsonGuildTemplate IJsonModel<JsonModels.JsonGuildTemplate>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildTemplate _jsonModel;

    private readonly RestClient _client;

    public GuildTemplate(JsonModels.JsonGuildTemplate jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        Creator = new(_jsonModel.Creator, client);
        SerializedSourceGuild = new(jsonModel.SerializedSourceGuild, client);
        _client = client;
    }

    public string Code => _jsonModel.Code;

    public string Name => _jsonModel.Name;

    public string Description => _jsonModel.Description;

    public int UsageCount => _jsonModel.UsageCount;

    public ulong CreatorId => _jsonModel.CreatorId;

    public User Creator { get; }

    public DateTimeOffset CreatedAt => _jsonModel.CreatedAt;

    public DateTimeOffset UpdatedAt => _jsonModel.UpdatedAt;

    public ulong SourceGuildId => _jsonModel.SourceGuildId;

    public Guild SerializedSourceGuild { get; }

    public bool? IsDirty => _jsonModel.IsDirty;

    #region GuildTemplate
    public Task<RestGuild> CreateGuildAsync(GuildFromGuildTemplateProperties guildProperties, RequestProperties? properties = null) => _client.CreateGuildFromGuildTemplateAsync(Code, guildProperties, properties);
    public Task<GuildTemplate> SyncAsync(RequestProperties? properties = null) => _client.SyncGuildTemplateAsync(SourceGuildId, Code, properties);
    public Task<GuildTemplate> ModifyAsync(Action<GuildTemplateOptions> action, RequestProperties? properties = null) => _client.ModifyGuildTemplateAsync(SourceGuildId, Code, action, properties);
    public Task<GuildTemplate> DeleteAsync(RequestProperties? properties = null) => _client.DeleteGuildTemplateAsync(SourceGuildId, Code, properties);
    #endregion
}
