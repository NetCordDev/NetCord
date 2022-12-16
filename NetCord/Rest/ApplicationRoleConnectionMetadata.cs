using System.Globalization;

namespace NetCord.Rest;

public class ApplicationRoleConnectionMetadata : IJsonModel<JsonModels.JsonApplicationRoleConnectionMetadata>
{
    JsonModels.JsonApplicationRoleConnectionMetadata IJsonModel<JsonModels.JsonApplicationRoleConnectionMetadata>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplicationRoleConnectionMetadata _jsonModel;

    public ApplicationRoleConnectionMetadata(JsonModels.JsonApplicationRoleConnectionMetadata jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public ApplicationRoleConnectionMetadataType Type => _jsonModel.Type;
    public string Key => _jsonModel.Key;
    public string Name => _jsonModel.Name;
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations => _jsonModel.NameLocalizations;
    public string Description => _jsonModel.Description;
    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations => _jsonModel.DescriptionLocalizations;
}
