using System.Globalization;

namespace NetCord.Rest;

public class ApplicationRoleConnectionMetadata(JsonModels.JsonApplicationRoleConnectionMetadata jsonModel) : IJsonModel<JsonModels.JsonApplicationRoleConnectionMetadata>
{
    JsonModels.JsonApplicationRoleConnectionMetadata IJsonModel<JsonModels.JsonApplicationRoleConnectionMetadata>.JsonModel => jsonModel;

    public ApplicationRoleConnectionMetadataType Type => jsonModel.Type;
    public string Key => jsonModel.Key;
    public string Name => jsonModel.Name;
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations => jsonModel.NameLocalizations;
    public string Description => jsonModel.Description;
    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations => jsonModel.DescriptionLocalizations;
}
