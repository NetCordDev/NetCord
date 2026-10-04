using System.Text.Json;

using NetCord.JsonModels;

namespace NetCord;

public class AuditLogChange(JsonAuditLogChange jsonModel)
{
    /// <summary>
    /// New value of the key.
    /// </summary>
    public JsonElement? NewValue { get; } = jsonModel.NewValue;

    /// <summary>
    /// Old value of the key.
    /// </summary>
    public JsonElement? OldValue { get; } = jsonModel.OldValue;

    /// <summary>
    /// Name of the changed entity, with a few exceptions.
    /// </summary>
    public string Key { get; } = jsonModel.Key;

    /// <summary>
    /// Whether there is a new value of the key.
    /// </summary>
    public bool HasNewValue => NewValue.HasValue;

    /// <summary>
    /// Whether there is an old value of the key.
    /// </summary>
    public bool HasOldValue => OldValue.HasValue;
}
