using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Gateway;

[JsonConverter(typeof(PartySizePropertiesConverter))]
[GenerateMethodsForProperties]
public partial class PartySizeProperties(long currentSize, long maxSize)
{
    public long CurrentSize { get; set; } = currentSize;

    public long MaxSize { get; set; } = maxSize;

    public class PartySizePropertiesConverter : JsonConverter<PartySizeProperties>
    {
        public override PartySizeProperties? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, PartySizeProperties value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.CurrentSize);
            writer.WriteNumberValue(value.MaxSize);
            writer.WriteEndArray();
        }
    }
}
