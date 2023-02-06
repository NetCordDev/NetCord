using System.Text.Json.Serialization.Metadata;

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

    /// <summary>
    /// Name of the changed entity, with a few exceptions.
    /// </summary>
    public string Key => _jsonModel.Key;

    /// <summary>
    /// Whether there is a new value of the key.
    /// </summary>
    public bool HasNewValue => _jsonModel.NewValue.HasValue;

    /// <summary>
    /// Whether there is an old value of the key.
    /// </summary>
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

    /// <summary>
    /// New value of the key.
    /// </summary>
    public TValue? NewValue => _jsonModel.NewValue!.Value.ToObject(_jsonTypeInfo);

    /// <summary>
    /// Old value of the key.
    /// </summary>
    public TValue? OldValue => _jsonModel.OldValue!.Value.ToObject(_jsonTypeInfo);
}
