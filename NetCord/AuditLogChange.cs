using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using NetCord.JsonModels;

namespace NetCord;

public class AuditLogChange
{
    public AuditLogChange(JsonAuditLogChange jsonModel) : this(jsonModel.NewValue, jsonModel.OldValue, jsonModel.Key)
    {
    }

    protected AuditLogChange(JsonElement? newValue, JsonElement? oldValue, string key)
    {
        _newValue = newValue;
        _oldValue = oldValue;
        Key = key;
    }

    protected readonly JsonElement? _newValue;
    protected readonly JsonElement? _oldValue;

    /// <summary>
    /// Name of the changed entity, with a few exceptions.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Whether there is a new value of the key.
    /// </summary>
    public bool HasNewValue => _newValue.HasValue;

    /// <summary>
    /// Whether there is an old value of the key.
    /// </summary>
    public bool HasOldValue => _oldValue.HasValue;

    /// <summary>
    /// Gets the change with values associated.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="jsonTypeInfo"></param>
    /// <returns></returns>
    public AuditLogChange<TValue> WithValues<TValue>(JsonTypeInfo<TValue> jsonTypeInfo) => new(_newValue, _oldValue, Key, jsonTypeInfo);

    /// <summary>
    /// Gets the change with values associated.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    public AuditLogChange<TValue> WithValues<TValue>() => new(_newValue, _oldValue, Key);
}

public class AuditLogChange<TValue> : AuditLogChange
{
    public AuditLogChange(JsonAuditLogChange jsonModel, JsonTypeInfo<TValue> jsonTypeInfo) : this(jsonModel.NewValue, jsonModel.OldValue, jsonModel.Key, jsonTypeInfo)
    {
    }

    internal AuditLogChange(JsonElement? newValue, JsonElement? oldValue, string key, JsonTypeInfo<TValue> jsonTypeInfo) : base(newValue, oldValue, key)
    {
        if (newValue.HasValue)
            NewValue = newValue.GetValueOrDefault().ToObject(jsonTypeInfo);

        if (oldValue.HasValue)
            OldValue = oldValue.GetValueOrDefault().ToObject(jsonTypeInfo);
    }

    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    public AuditLogChange(JsonAuditLogChange jsonModel) : this(jsonModel.NewValue, jsonModel.OldValue, jsonModel.Key)
    {
    }

    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    internal AuditLogChange(JsonElement? newValue, JsonElement? oldValue, string key) : base(newValue, oldValue, key)
    {
        if (newValue.HasValue)
            NewValue = newValue.GetValueOrDefault().ToObject<TValue>();

        if (oldValue.HasValue)
            OldValue = oldValue.GetValueOrDefault().ToObject<TValue>();
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
