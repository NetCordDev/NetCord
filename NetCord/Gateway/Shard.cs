using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Gateway;

[JsonConverter(typeof(JsonShardConverter))]
public class Shard
{
    private Shard(int shardId, int shardsCount)
    {
        ShardId = shardId;
        ShardsCount = shardsCount;
    }

    public int ShardId { get; }

    public int ShardsCount { get; }

    private class JsonShardConverter : JsonConverter<Shard>
    {
        public override Shard? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var a = reader.ToObject<int[]>();
            return new(a[0], a[1]);
        }

        public override void Write(Utf8JsonWriter writer, Shard value, JsonSerializerOptions options) => throw new NotImplementedException();
    }
}
