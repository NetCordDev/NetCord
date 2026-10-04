using NetCord.JsonModels;

namespace NetCord;

public class InteractionGuildReference(JsonInteractionGuildReference jsonModel) : Entity(jsonModel)
{
    public IReadOnlyList<string> Features { get; } = jsonModel.Features;

    public string Locale { get; } = jsonModel.Locale;
}
