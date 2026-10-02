using NetCord.JsonModels;

namespace NetCord;

public class InteractionGuildReference(JsonInteractionGuildReference jsonModel) : Entity
{
    public override ulong Id { get; } = jsonModel.Id;

    public IReadOnlyList<string> Features { get; } = jsonModel.Features;

    public string Locale { get; } = jsonModel.Locale;
}
