﻿using NetCord.Rest;

namespace NetCord;

public class Integration : Entity, IJsonModel<JsonModels.JsonIntegration>
{
    public Integration(JsonModels.JsonIntegration jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;

        var user = _jsonModel.User;
        if (user is not null)
            User = new(user, client);

        Account = new(_jsonModel.Account);

        var application = _jsonModel.Application;
        if (application is not null)
            Application = new(application, client);
    }

    JsonModels.JsonIntegration IJsonModel<JsonModels.JsonIntegration>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonIntegration _jsonModel;

    public override ulong Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public IntegrationType Type => _jsonModel.Type;

    public bool Enabled => _jsonModel.Enabled;

    public bool? Syncing => _jsonModel.Syncing;

    public ulong? RoleId => _jsonModel.RoleId;

    public bool? EnableEmoticons => _jsonModel.EnableEmoticons;

    public IntegrationExpireBehavior? ExpireBehavior => _jsonModel.ExpireBehavior;

    public int? ExpireGracePeriod => _jsonModel.ExpireGracePeriod;

    public User? User { get; }

    public Account Account { get; }

    public DateTimeOffset? SyncedAt => _jsonModel.SyncedAt;

    public int? SubscriberCount => _jsonModel.SubscriberCount;

    public bool? Revoked => _jsonModel.Revoked;

    public IntegrationApplication? Application { get; }
}
