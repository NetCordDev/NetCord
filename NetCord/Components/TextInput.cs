using NetCord.JsonModels;

namespace NetCord;

public class TextInput : IComponent
{
    private readonly JsonComponent _jsonEntity;

    public ComponentType ComponentType => ComponentType.TextInput;
    public string CustomId => _jsonEntity.CustomId!;
    public string? Placeholder => _jsonEntity.Placeholder;
    public string? Label => _jsonEntity.Label!;
    public int? MinLength => _jsonEntity.MinLength;
    public int? MaxLength => _jsonEntity.MaxLength;
    public bool? Required => _jsonEntity.Required;
    public string Value => _jsonEntity.Value;

    internal TextInput(JsonComponent jsonEntity)
    {
        _jsonEntity = jsonEntity.Components[0];
    }
}
