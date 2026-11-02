using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class ApplicationRoleConnectionMetadata(JsonApplicationRoleConnectionMetadata jsonModel)
{
    public ApplicationRoleConnectionMetadataType Type { get; } = jsonModel.Type;

    public string Key { get; } = jsonModel.Key;

    public string Name { get; } = jsonModel.Name;

    public IReadOnlyDictionary<string, string>? NameLocalizations { get; } = jsonModel.NameLocalizations;

    public string Description { get; } = jsonModel.Description;

    public IReadOnlyDictionary<string, string>? DescriptionLocalizations { get; } = jsonModel.DescriptionLocalizations;
}
