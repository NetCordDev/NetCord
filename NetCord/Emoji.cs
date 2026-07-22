using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a standard Discord emoji.
/// </summary>
public class Emoji : IJsonModel<JsonEmoji>
{
    JsonEmoji IJsonModel<JsonEmoji>.JsonModel => _jsonModel;

    private protected readonly JsonEmoji _jsonModel;

    /// <summary>
    /// The emoji's name.
    /// </summary>
    public string Name => _jsonModel.Name!;

    /// <summary>
    /// Whether the emoji is animated.
    /// </summary>
    public bool Animated => _jsonModel.Animated;

    private protected Emoji(JsonEmoji jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public override string ToString() => Name;

    public static Emoji CreateFromJson(JsonEmoji jsonModel, ulong guildId, RestClient client)
    {
        if (jsonModel.Id.HasValue)
            return new GuildEmoji(jsonModel, guildId, client);
        else
            return new Emoji(jsonModel);
    }
}
