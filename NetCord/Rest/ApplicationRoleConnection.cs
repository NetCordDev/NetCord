namespace NetCord.Rest;

public class ApplicationRoleConnection(JsonModels.JsonApplicationRoleConnection jsonModel)
{
    public string? PlatformName { get; } = jsonModel.PlatformName;

    public IReadOnlyDictionary<string, string> Metadata { get; } = jsonModel.Metadata;
}
