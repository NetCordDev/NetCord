using NetCord.Rest;
using NetCord.Rest.JsonModels;

namespace NetCord;

public partial class GuildTemplate(JsonGuildTemplate jsonModel, RestClient client) : IJsonModel<JsonGuildTemplate>
{
    JsonGuildTemplate IJsonModel<JsonGuildTemplate>.JsonModel => jsonModel;

    private readonly RestClient _client = client;

    public string Code => jsonModel.Code;

    public string Name => jsonModel.Name;

    public string Description => jsonModel.Description;

    public int UsageCount => jsonModel.UsageCount;

    public ulong CreatorId => jsonModel.CreatorId;

    public User Creator { get; } = new(jsonModel.Creator, client);

    public DateTimeOffset CreatedAt => jsonModel.CreatedAt;

    public DateTimeOffset UpdatedAt => jsonModel.UpdatedAt;

    public ulong SourceGuildId => jsonModel.SourceGuildId;

    public GuildTemplatePreview Preview { get; } = new(jsonModel.Preview, client);

    public bool? IsDirty => jsonModel.IsDirty;
}
