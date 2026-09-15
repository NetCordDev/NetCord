using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a category within a guild.
/// </summary>
public partial class CategoryGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : Channel(jsonModel, client), IGuildChannel
{
    /// <summary>
    /// The ID of the guild this category belongs to.
    /// </summary>
    public ulong GuildId { get; } = guildId;

    /// <summary>
    /// The sorting position of the category channel.
    /// </summary>
    public int? Position => jsonModel.Position;

    /// <summary>
    /// A dictionary of permission overwrites for roles and users in this category channel.
    /// </summary>
    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));

    /// <summary>
    /// The name of the category channel.
    /// </summary>
    public string Name => jsonModel.Name!;
}
