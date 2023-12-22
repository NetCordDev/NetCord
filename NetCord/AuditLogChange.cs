using System.Diagnostics.CodeAnalysis;
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

    /// <summary>
    /// Gets the change with values associated.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="jsonTypeInfo"></param>
    /// <returns></returns>
    public AuditLogChange<TValue> WithValues<TValue>(JsonTypeInfo<TValue> jsonTypeInfo) => new(_jsonModel, jsonTypeInfo);

    /// <summary>
    /// Gets the change with values associated.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    public AuditLogChange<TValue> WithValues<TValue>() => new(_jsonModel);
}

public class AuditLogChange<TValue> : AuditLogChange
{
    public AuditLogChange(JsonAuditLogChange jsonModel, JsonTypeInfo<TValue> jsonTypeInfo) : base(jsonModel)
    {
        NewValue = jsonModel.NewValue!.Value.ToObject(jsonTypeInfo);
        OldValue = jsonModel.OldValue!.Value.ToObject(jsonTypeInfo);
    }

    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    public AuditLogChange(JsonAuditLogChange jsonModel) : base(jsonModel)
    {
        NewValue = jsonModel.NewValue!.Value.ToObject<TValue>();
        OldValue = jsonModel.OldValue!.Value.ToObject<TValue>();
    }

    /// <summary>
    /// New value of the key.
    /// </summary>
    public TValue? NewValue { get; }

    /// <summary>
    /// Old value of the key.
    /// </summary>
    public TValue? OldValue { get; }
}
