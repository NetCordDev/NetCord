using NetCord.Rest;
using NetCord.Rest.JsonModels;

namespace NetCord;

public partial class GuildTemplate(JsonGuildTemplate jsonModel, RestClient client)
{
    public string Code { get; } = jsonModel.Code;

    public string Name { get; } = jsonModel.Name;

    public string? Description { get; } = jsonModel.Description;

    public int UsageCount { get; } = jsonModel.UsageCount;

    public ulong CreatorId { get; } = jsonModel.CreatorId;

    public User Creator { get; } = new(jsonModel.Creator, client);

    public DateTimeOffset CreatedAt { get; } = jsonModel.CreatedAt;

    public DateTimeOffset UpdatedAt { get; } = jsonModel.UpdatedAt;

    public ulong SourceGuildId { get; } = jsonModel.SourceGuildId;

    public GuildTemplateSerializerSourceGuild SerializedSourceGuild { get; } = new(jsonModel.SerializedSourceGuild, client);

    public bool? IsDirty { get; } = jsonModel.IsDirty;
}
