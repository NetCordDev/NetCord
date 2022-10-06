using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Gateway;

[JsonConverter(typeof(JsonShardConverter))]
public partial class Shard
{
    private Shard(int shardId, int shardsCount)
    {
        ShardId = shardId;
        ShardsCount = shardsCount;
    }

    public int ShardId { get; }

    public int ShardsCount { get; }

    internal partial class JsonShardConverter : JsonConverter<Shard>
    {
        public override Shard? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var a = reader.ToObject(Int32ArraySerializerContext.WithOptions.Int32Array);
            return new(a[0], a[1]);
        }

        public override void Write(Utf8JsonWriter writer, Shard value, JsonSerializerOptions options) => throw new NotImplementedException();

        [JsonSerializable(typeof(int[]))]
        internal partial class Int32ArraySerializerContext : JsonSerializerContext
        {
            public static Int32ArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
        }
    }
}
