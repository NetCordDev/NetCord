using System.Text.Json;

namespace NetCord;

public interface IJsonSerializable
{
    public void WriteTo(Utf8JsonWriter writer);
}

internal interface IJsonSerializable<in T>
{
    public void WriteTo(Utf8JsonWriter writer, T data);
}
