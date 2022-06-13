using NetCord.JsonModels;

namespace NetCord;

public class AuditLogChange : IJsonModel<JsonAuditLogChange>
{
    JsonAuditLogChange IJsonModel<JsonAuditLogChange>.JsonModel => _jsonModel;
    private protected readonly JsonAuditLogChange _jsonModel;

    public AuditLogChange(JsonAuditLogChange jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public string Key => _jsonModel.Key;

    public bool HasNewValue => _jsonModel.NewValue.HasValue;

    public bool HasOldValue => _jsonModel.OldValue.HasValue;

    public AuditLogChange<TValue> GetWithValues<TValue>() => new(_jsonModel);
}

public class AuditLogChange<TValue> : AuditLogChange
{
    public AuditLogChange(JsonAuditLogChange jsonModel) : base(jsonModel)
    {
    }

    public TValue? NewValue => _jsonModel.NewValue!.Value.ToObject<TValue>();

    public TValue? OldValue => _jsonModel.OldValue!.Value.ToObject<TValue>();
}