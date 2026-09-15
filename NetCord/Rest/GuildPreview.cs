using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a partial preview of a guild, available even if the user is not a member.
/// </summary>
public class GuildPreview(JsonModels.JsonGuild jsonModel, RestClient client) : ClientEntity(client), IJsonModel<JsonModels.JsonGuild>
{
    JsonModels.JsonGuild IJsonModel<JsonModels.JsonGuild>.JsonModel => jsonModel;

    /// <summary>
    /// The unique identifier of the guild.
    /// </summary>
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// The name of the guild.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// The guild's icon hash, if set.
    /// </summary>
    public string? IconHash => jsonModel.IconHash;

    /// <summary>
    /// The guild's invite splash hash, if set.
    /// </summary>
    public string? SplashHash => jsonModel.SplashHash;

    /// <summary>
    /// The guild's discovery splash hash, if set.
    /// </summary>
    public string? DiscoverySplashHash => jsonModel.DiscoverySplashHash;

    /// <summary>
    /// A dictionary containing the custom emojis available in the guild, indexed by their unique identifiers.
    /// </summary>
    public IReadOnlyDictionary<ulong, GuildEmoji> Emojis { get; } = jsonModel.Emojis.ToDictionary(
        e => e.Id.GetValueOrDefault(), 
        e => new GuildEmoji(e, jsonModel.Id, client));

    /// <summary>
    /// A list of enabled features and capabilities for the guild.
    /// </summary>
    public IReadOnlyList<string> Features => jsonModel.Features;

    /// <summary>
    /// The approximate total number of users in the guild.
    /// </summary>
    public int ApproximateUserCount => jsonModel.ApproximateUserCount.GetValueOrDefault();

    /// <summary>
    /// The approximate number of online or active presences in the guild.
    /// </summary>
    public int ApproximatePresenceCount => jsonModel.ApproximatePresenceCount.GetValueOrDefault();

    /// <summary>
    /// The description provided for the guild, if applicable.
    /// </summary>
    public string? Description => jsonModel.Description;

    /// <summary>
    /// A dictionary containing the custom stickers available in the guild, indexed by their unique identifiers.
    /// </summary>
    public IReadOnlyDictionary<ulong, GuildSticker> Stickers { get; } = jsonModel.Stickers.ToDictionary(
        s => s.Id, 
        s => new GuildSticker(s, client));
}
