using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a base Discord emoji.
/// </summary>
public class Emoji(JsonEmoji jsonModel) : IJsonModel<JsonEmoji>
{
    JsonEmoji IJsonModel<JsonEmoji>.JsonModel => jsonModel;

    /// <summary>
    /// The emoji's name.
    /// </summary>
    public string Name => jsonModel.Name!;

    /// <summary>
    /// Whether the emoji is animated.
    /// </summary>
    public bool Animated => jsonModel.Animated;

    public override string ToString() => Name;

    public static Emoji CreateFromJson(JsonEmoji jsonModel, ulong guildId, RestClient client)
    {
        if (jsonModel.Id.HasValue)
            return new GuildEmoji(jsonModel, guildId, client);
        else
            return new Emoji(jsonModel);
    }
}
