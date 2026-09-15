using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a custom guild sticker.
/// </summary>
public partial class GuildSticker(JsonModels.JsonSticker jsonModel, RestClient client) : Sticker(jsonModel)
{
    /// <summary>
    /// Whether the sticker is available for use. Can be <see langword="false"/> if server boosts are lost.
    /// </summary>
    public bool? Available => jsonModel.Available;

    /// <summary>
    /// The ID corresponding to the sticker's parent guild.
    /// </summary>
    public ulong GuildId => jsonModel.GuildId.GetValueOrDefault();

    /// <summary>
    /// The user that uploaded the sticker.
    /// </summary>
    public User? Creator { get; } = jsonModel.Creator is { } creator ? new(creator, client) : null;
}
