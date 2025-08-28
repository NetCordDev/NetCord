using NetCord.JsonModels;

namespace NetCord;

public class MentionableMenuDefaultValue(JsonEntityMenuDefaultValue jsonModel) : Entity
{
    public override ulong Id { get; } = jsonModel.Id;

    public MentionableMenuDefaultValueType Type { get; } = (MentionableMenuDefaultValueType)jsonModel.Type;
}
