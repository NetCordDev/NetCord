using NetCord.Rest;

namespace NetCord;

public partial class TextGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : TextChannel(jsonModel, client), IGuildChannel
{
    public ulong GuildId { get; } = guildId;
    public int? Position => _jsonModel.Position;
    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    public string Name => _jsonModel.Name!;
    public string? Topic => _jsonModel.Topic;
    public bool Nsfw => _jsonModel.Nsfw.GetValueOrDefault();
    public int Slowmode => _jsonModel.Slowmode.GetValueOrDefault();
    public ulong? ParentId => _jsonModel.ParentId;
    public int? DefaultAutoArchiveDuration => _jsonModel.DefaultAutoArchiveDuration;
    public int DefaultThreadSlowmode => _jsonModel.DefaultThreadSlowmode.GetValueOrDefault();
}
