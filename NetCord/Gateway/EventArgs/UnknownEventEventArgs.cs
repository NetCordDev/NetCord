using System.Text.Json;

namespace NetCord.Gateway;

public class UnknownEventEventArgs
{
    public UnknownEventEventArgs(string name, JsonElement data)
    {
        Name = name;
        Data = data;
    }

    /// <summary>
    /// Event name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Event data.
    /// </summary>
    public JsonElement Data { get; }
}
