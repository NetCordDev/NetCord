namespace NetCord;

public class Entitlement : Entity, IJsonModel<JsonModels.JsonEntitlement>
{
    JsonModels.JsonEntitlement IJsonModel<JsonModels.JsonEntitlement>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEntitlement _jsonModel;

    public Entitlement(JsonModels.JsonEntitlement jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public override ulong Id => _jsonModel.Id;

    public ulong SkuId => _jsonModel.SkuId;

    public ulong? UserId => _jsonModel.UserId;

    public ulong? GuildId => _jsonModel.GuildId;

    public ulong ApplicationId => _jsonModel.ApplicationId;

    public EntitlementType Type => _jsonModel.Type;

    public bool Consumed => _jsonModel.Consumed;

    public DateTimeOffset? StartsAt => _jsonModel.StartsAt;

    public DateTimeOffset? EndsAt => _jsonModel.EndsAt;
}
