using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.JsonConverters;

using JsonGuard;

namespace NetCord.JsonModels;

[JsonConverter(typeof(Converter))]
[JsonGuard]
public partial class JsonInteraction : JsonEntity
{
    [JsonGuard]
    public partial class Converter : JsonConverter<JsonInteraction>
    {
        public override JsonInteraction? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var readerCopy = reader;

            if (!JsonConverterHelper.TrySkipToProperty(ref readerCopy, "type"u8))
                throw new JsonException("Missing property 'type'.");

            var type = (InteractionType)readerCopy.GetInt32();

            return type switch
            {
                InteractionType.Ping => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonPingInteraction),
                InteractionType.ApplicationCommand => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonApplicationCommandInteraction),
                InteractionType.MessageComponent => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonMessageComponentInteraction),
                InteractionType.Autocomplete => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonAutocompleteInteraction),
                InteractionType.ModalSubmit => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonModalSubmitInteraction),
                _ => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonUnknownInteraction),
            };
        }

        public override void Write(Utf8JsonWriter writer, JsonInteraction value, JsonSerializerOptions options) => throw new NotSupportedException();
    }

    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonPropertyName("type")]
    public InteractionType Type { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("guild")]
    public JsonInteractionGuildReference? GuildReference { get; set; }

    [JsonPropertyName("channel")]
    public JsonChannel? Channel { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? GuildUser { get; set; }

    [JsonPropertyName("user")]
    public JsonUser? User { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("message")]
    public JsonMessage? Message { get; set; }

    [JsonPropertyName("app_permissions")]
    public Permissions AppPermissions { get; set; }

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("guild_locale")]
    public string? GuildLocale { get; set; }

    [JsonPropertyName("entitlements")]
    public JsonEntitlement[] Entitlements { get; set; }

    [JsonPropertyName("authorizing_integration_owners")]
    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> AuthorizingIntegrationOwners { get; set; }

    [JsonPropertyName("context")]
    public InteractionContextType? Context { get; set; }

    [JsonPropertyName("attachment_size_limit")]
    public long AttachmentSizeLimit { get; set; }
}

[JsonGuard]
public partial class JsonPingInteraction : JsonInteraction;

[JsonGuard]
public partial class JsonApplicationCommandInteraction : JsonInteraction
{
    [JsonPropertyName("data")]
    public JsonApplicationCommandInteractionData Data { get; set; }
}

[JsonGuard]
public partial class JsonMessageComponentInteraction : JsonInteraction
{
    [JsonPropertyName("data")]
    public JsonMessageComponentInteractionData Data { get; set; }
}

[JsonGuard]
public partial class JsonAutocompleteInteraction : JsonInteraction
{
    [JsonPropertyName("data")]
    public JsonApplicationCommandInteractionData Data { get; set; }
}

[JsonGuard]
public partial class JsonModalSubmitInteraction : JsonInteraction
{
    [JsonPropertyName("data")]
    public JsonModalSubmitInteractionData Data { get; set; }
}

[JsonGuard]
public partial class JsonUnknownInteraction : JsonInteraction
{
    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }
}
