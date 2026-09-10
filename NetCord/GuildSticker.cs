using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a custom guild sticker.
/// </summary>
public partial class GuildSticker : Sticker
{
    private readonly RestClient _client;

    /// <summary>
    /// Whether the sticker is available for use. Can be <see langword="false"/> if server boosts are lost.
    /// </summary>
    public bool? Available => _jsonModel.Available;

    /// <summary>
    /// The ID corresponding to the sticker's parent guild.
    /// </summary>
    public ulong GuildId => _jsonModel.GuildId;

    /// <summary>
    /// The user that uploaded the sticker.
    /// </summary>
    public User? Creator { get; }

    public GuildSticker(JsonModels.JsonSticker jsonModel, RestClient client) : base(jsonModel)
    {
        _client = client;

        var creator = jsonModel.Creator;
        if (creator is not null)
            Creator = new(creator, client);
    }
}
