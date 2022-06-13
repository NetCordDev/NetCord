using NetCord.JsonModels;

namespace NetCord;

public class Emoji : IJsonModel<JsonEmoji>
{
    JsonEmoji IJsonModel<JsonEmoji>.JsonModel => _jsonModel;

    private protected readonly JsonEmoji _jsonModel;

    public string Name => _jsonModel.Name!;

    public bool Animated => _jsonModel.Animated;


    public Emoji(JsonEmoji jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public override string ToString() => Name;

    internal static Emoji CreateFromJson(JsonEmoji jsonModel, Snowflake guildId, RestClient client)
    {
        if (jsonModel.Id.HasValue)
            return new GuildEmoji(jsonModel, guildId, client);
        else
            return new Emoji(jsonModel);
    }
}