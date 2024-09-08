using NetCord.JsonModels;

namespace NetCord;

public class InteractionGuildReference(JsonInteractionGuildReference jsonModel) : Entity, IJsonModel<JsonInteractionGuildReference>
{
    JsonInteractionGuildReference IJsonModel<JsonInteractionGuildReference>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;

    public string[] Features => jsonModel.Features;

    public string Locale => jsonModel.Locale;
}
