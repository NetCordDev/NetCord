﻿using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class Emoji : IJsonModel<JsonEmoji>
{
    private protected Emoji(JsonEmoji jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public static Emoji CreateFromJson(JsonEmoji jsonModel, ulong guildId, RestClient client)
    {
        if (jsonModel.Id.HasValue)
            return new GuildEmoji(jsonModel, guildId, client);
        else
            return new Emoji(jsonModel);
    }

    JsonEmoji IJsonModel<JsonEmoji>.JsonModel => _jsonModel;

    private protected readonly JsonEmoji _jsonModel;

    public string Name => _jsonModel.Name!;

    public bool Animated => _jsonModel.Animated;

    public override string ToString() => Name;
}
