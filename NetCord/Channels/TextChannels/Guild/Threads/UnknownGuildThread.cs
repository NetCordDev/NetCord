﻿using NetCord.Rest;

namespace NetCord;

public class UnknownGuildThread : GuildThread, IUnknownGuildThread
{
    public UnknownGuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
    }

    public ChannelType Type => _jsonModel.Type;
}