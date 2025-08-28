using System.Text.Json;

namespace NetCord;

public interface IJsonSerializable<TSelf> where TSelf : IJsonSerializable<TSelf>
{
    public void WriteTo(Utf8JsonWriter writer);
}

internal interface IJsonSerializable<TSelf, in TData> where TSelf : IJsonSerializable<TSelf, TData>
{
    public void WriteTo(Utf8JsonWriter writer, TData data);
}
