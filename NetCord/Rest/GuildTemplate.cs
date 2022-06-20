using NetCord.JsonModels;

namespace NetCord;

public class GuildTemplate : IJsonModel<JsonGuildTemplate>
{
    JsonGuildTemplate IJsonModel<JsonGuildTemplate>.JsonModel => _jsonModel;
    private readonly JsonGuildTemplate _jsonModel;

    public GuildTemplate(JsonGuildTemplate jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        Creator = new(_jsonModel.Creator, client);
        SerializedSourceGuild = new(_jsonModel.SerializedSourceGuild, client);
    }

    public string Code => _jsonModel.Code;

    public string Name => _jsonModel.Name;

    public string Description => _jsonModel.Description;

    public int UsageCount => _jsonModel.UsageCount;

    public Snowflake CreatorId => _jsonModel.CreatorId;

    public User Creator { get; }

    public DateTimeOffset CreatedAt => _jsonModel.CreatedAt;

    public DateTimeOffset UpdatedAt => _jsonModel.UpdatedAt;

    public Snowflake SourceGuildId => _jsonModel.SourceGuildId;

    public GuildTemplateSourceGuild SerializedSourceGuild { get; }

    public bool? IsDirty => _jsonModel.IsDirty;
}