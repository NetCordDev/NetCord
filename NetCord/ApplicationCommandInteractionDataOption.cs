using System.Text.Json;

namespace NetCord;

public class ApplicationCommandInteractionDataOption
{
    private readonly JsonModels.JsonApplicationCommandInteractionDataOption _jsonEntity;

    public string Name => _jsonEntity.Name;

    public ApplicationCommandOptionType Type => _jsonEntity.Type;

    public string? Value { get; }

    public IEnumerable<ApplicationCommandInteractionDataOption>? Options { get; }

    public bool Focused => _jsonEntity.Focused;

    internal ApplicationCommandInteractionDataOption(JsonModels.JsonApplicationCommandInteractionDataOption jsonEntity)
    {
        _jsonEntity = jsonEntity;
        if (jsonEntity.Value.HasValue)
        {
            var value = jsonEntity.Value.GetValueOrDefault();
            Value = value.ValueKind == JsonValueKind.String ? value.GetString() : value.GetRawText();
        }
        Options = jsonEntity.Options.SelectOrEmpty(o => new ApplicationCommandInteractionDataOption(o));
    }
}