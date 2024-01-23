using NetCord.Rest;

namespace NetCord;

public partial class TextGuildChannel : TextChannel, IGuildChannel
{
    public TextGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : base(jsonModel, client)
    {
        GuildId = guildId;
        PermissionOverwrites = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    public ulong GuildId { get; }
    public int? Position => _jsonModel.Position;
    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; }
    public string Name => _jsonModel.Name!;
    public string? Topic => _jsonModel.Topic;
    public bool Nsfw => _jsonModel.Nsfw.GetValueOrDefault();
    public int Slowmode => _jsonModel.Slowmode.GetValueOrDefault();
    public ulong? ParentId => _jsonModel.ParentId;
    public int? DefaultAutoArchiveDuration => _jsonModel.DefaultAutoArchiveDuration;
    public int DefaultThreadSlowmode => _jsonModel.DefaultThreadSlowmode.GetValueOrDefault();
}
