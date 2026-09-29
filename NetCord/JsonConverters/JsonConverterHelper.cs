using System.Text.Json;

namespace NetCord.JsonConverters;

internal static class JsonConverterHelper
{
    public static bool TrySkipToProperty(ref Utf8JsonReader reader, ReadOnlySpan<byte> propertyName)
    {
        while (true)
        {
            if (!reader.Read())
                return false;

            switch (reader.TokenType)
            {
                case JsonTokenType.PropertyName:
                    if (reader.ValueTextEquals(propertyName))
                        return reader.Read();

                    reader.Skip();
                    break;
                case JsonTokenType.EndObject:
                    return false;
            }
        }
    }
}
