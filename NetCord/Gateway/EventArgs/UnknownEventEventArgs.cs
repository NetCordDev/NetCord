using System.Text.Json;

namespace NetCord.Gateway;

public class UnknownEventEventArgs(string name, JsonElement data)
{

    /// <summary>
    /// Event name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Event data.
    /// </summary>
    public JsonElement Data { get; } = data;
}
