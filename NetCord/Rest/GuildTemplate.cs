using NetCord.JsonModels;

namespace NetCord.Rest;

public class GuildTemplate : IJsonModel<JsonGuildTemplate>
{
    JsonGuildTemplate IJsonModel<JsonGuildTemplate>.JsonModel => _jsonModel;
    private readonly JsonGuildTemplate _jsonModel;

    private readonly RestClient _client;

    public GuildTemplate(JsonGuildTemplate jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        Creator = new(_jsonModel.Creator, client);
        SerializedSourceGuild = new(_jsonModel.SerializedSourceGuild, _jsonModel.SourceGuildId, client);
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

    public GuildTemplateSourceGuild SerializedSourceGuild { get; }

    public bool? IsDirty => _jsonModel.IsDirty;

    #region GuildTemplate
    public Task<RestGuild> CreateGuildAsync(GuildFromGuildTemplateProperties guildProperties, RequestProperties? properties = null) => _client.CreateGuildFromGuildTemplateAsync(Code, guildProperties, properties);
    public Task<GuildTemplate> SyncAsync(RequestProperties? properties = null) => _client.SyncGuildTemplateAsync(SourceGuildId, Code, properties);
    public Task<GuildTemplate> ModifyAsync(Action<GuildTemplateOptions> action, RequestProperties? properties = null) => _client.ModifyGuildTemplateAsync(SourceGuildId, Code, action, properties);
    public Task<GuildTemplate> DeleteAsync(RequestProperties? properties = null) => _client.DeleteGuildTemplateAsync(SourceGuildId, Code, properties);
    #endregion
}
