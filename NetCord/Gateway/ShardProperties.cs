using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Gateway;

[JsonConverter(typeof(ShardPropertiesConverter))]
public class ShardProperties
{
    public ShardProperties(int shardId, int shardsCount)
    {
        ShardId = shardId;
        ShardsCount = shardsCount;
    }

    public int ShardId { get; }

    public int ShardsCount { get; }

    private class ShardPropertiesConverter : JsonConverter<ShardProperties>
    {
        public override ShardProperties? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, ShardProperties value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.ShardId);
            writer.WriteNumberValue(value.ShardsCount);
            writer.WriteEndArray();
        }
    }
}