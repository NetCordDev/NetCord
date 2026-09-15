using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;

using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents a change entry in an audit log.
/// </summary>
public class AuditLogChange(JsonAuditLogChange jsonModel) : IJsonModel<JsonAuditLogChange>
{
    JsonAuditLogChange IJsonModel<JsonAuditLogChange>.JsonModel => jsonModel;

    /// <summary>
    /// Name of the changed entity, with a few exceptions.
    /// </summary>
    public string Key => jsonModel.Key;

    /// <summary>
    /// Whether there is a new value of the key.
    /// </summary>
    public bool HasNewValue => jsonModel.NewValue.HasValue;

    /// <summary>
    /// Whether there is an old value of the key.
    /// </summary>
    public bool HasOldValue => jsonModel.OldValue.HasValue;

    /// <summary>
    /// Gets the change with values associated using the specified JSON type information.
    /// </summary>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="jsonTypeInfo">The JSON type info used for deserialization.</param>
    /// <returns>A new <see cref="AuditLogChange{TValue}"/> instance with strongly-typed values.</returns>
    public AuditLogChange<TValue> WithValues<TValue>(JsonTypeInfo<TValue> jsonTypeInfo) => new(jsonModel, jsonTypeInfo);

    /// <summary>
    /// Gets the change with values associated.
    /// </summary>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <returns>A new <see cref="AuditLogChange{TValue}"/> instance with strongly-typed values.</returns>
    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    public AuditLogChange<TValue> WithValues<TValue>() => new(jsonModel);
}

/// <summary>
/// Represents a strongly-typed change entry in an audit log.
/// </summary>
/// <typeparam name="TValue">The type of the values associated with the change.</typeparam>
public class AuditLogChange<TValue>(JsonAuditLogChange jsonModel, JsonTypeInfo<TValue> jsonTypeInfo) : AuditLogChange(jsonModel)
{
    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    public AuditLogChange(JsonAuditLogChange jsonModel) : this(jsonModel, null!)
    {
        NewValue = jsonModel.NewValue is { } newValue ? newValue.ToObject<TValue>() : default;
        OldValue = jsonModel.OldValue is { } oldValue ? oldValue.ToObject<TValue>() : default;
    }

    /// <summary>
    /// New value of the key.
    /// </summary>
    public TValue? NewValue { get; } = jsonModel.NewValue is { } newValue ? newValue.ToObject(jsonTypeInfo) : default;

    /// <summary>
    /// Old value of the key.
    /// </summary>
    public TValue? OldValue { get; } = jsonModel.OldValue is { } oldValue ? oldValue.ToObject(jsonTypeInfo) : default;
}
