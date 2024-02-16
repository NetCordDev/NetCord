using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Gateway;

[JsonConverter(typeof(JsonShardConverter))]
public readonly struct Shard(int id, int count)
{
    public int Id { get; } = id;

    public int Count { get; } = count;

    public class JsonShardConverter : JsonConverter<Shard>
    {
        public override Shard Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.Read();
            var id = reader.GetInt32();
            reader.Read();
            var count = reader.GetInt32();
            reader.Read();
            return new(id, count);
        }

        public override void Write(Utf8JsonWriter writer, Shard value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.Id);
            writer.WriteNumberValue(value.Count);
            writer.WriteEndArray();
        }
    }
}
