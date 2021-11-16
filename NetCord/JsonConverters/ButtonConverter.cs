using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters
{
    internal class ButtonConverter : JsonConverter<Button>
    {
        public override Button Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, Button button, JsonSerializerOptions options)
        {
            if (button is ActionButton actionButton)
                JsonSerializer.Serialize(writer, actionButton);
            else if (button is LinkButton linkButton)
                JsonSerializer.Serialize(writer, linkButton);
        }
    }
}
