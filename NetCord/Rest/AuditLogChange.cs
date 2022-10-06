using System.Text.Json.Serialization.Metadata;

using NetCord.JsonModels;

namespace NetCord.Rest;

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

    public AuditLogChange<TValue> GetWithValues<TValue>(JsonTypeInfo<TValue> jsonTypeInfo) => new(_jsonModel, jsonTypeInfo);
}

public class AuditLogChange<TValue> : AuditLogChange
{
    private readonly JsonTypeInfo<TValue> _jsonTypeInfo;

    public AuditLogChange(JsonAuditLogChange jsonModel, JsonTypeInfo<TValue> jsonTypeInfo) : base(jsonModel)
    {
        _jsonTypeInfo = jsonTypeInfo;
    }

    public TValue? NewValue => _jsonModel.NewValue!.Value.ToObject(_jsonTypeInfo);

    public TValue? OldValue => _jsonModel.OldValue!.Value.ToObject(_jsonTypeInfo);
}
