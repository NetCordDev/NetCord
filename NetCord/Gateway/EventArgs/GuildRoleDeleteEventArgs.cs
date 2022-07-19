﻿using NetCord.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class GuildRoleDeleteEventArgs : IJsonModel<JsonModels.EventArgs.JsonGuildRoleDeleteEventArgs>
{
    JsonModels.EventArgs.JsonGuildRoleDeleteEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildRoleDeleteEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonGuildRoleDeleteEventArgs _jsonModel;

    public GuildRoleDeleteEventArgs(JsonGuildRoleDeleteEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public Snowflake GuildId => _jsonModel.GuildId;

    public Snowflake RoleId => _jsonModel.RoleId;
}