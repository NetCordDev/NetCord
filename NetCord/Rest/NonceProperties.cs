using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(NoncePropertiesConverter))]
public partial class NonceProperties
{
    private readonly string? _s;
    private readonly long _l;

    /// <summary>
    /// If <see langword="true"/>, the nonce will be checked for uniqueness in the past few minutes. If another message was created by the same author with the same nonce, that message will be returned and no new message will be created.
    /// </summary>
    public bool Unique { get; set; } = true;

    public NonceProperties(string s)
    {
        _s = s;
    }

    public NonceProperties(long l)
    {
        _l = l;
    }

    public static implicit operator NonceProperties(long l) => new(l);

    public static implicit operator NonceProperties(string s) => new(s);

    public class NoncePropertiesConverter : JsonConverter<NonceProperties>
    {
        private static readonly JsonEncodedText _enforceNonce = JsonEncodedText.Encode("enforce_nonce");

        public override NonceProperties? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, NonceProperties value, JsonSerializerOptions options)
        {
            var s = value._s;
            if (s is not null)
                writer.WriteStringValue(s);
            else
                writer.WriteNumberValue(value._l);

            writer.WriteBoolean(_enforceNonce, value.Unique);
        }
    }
}
