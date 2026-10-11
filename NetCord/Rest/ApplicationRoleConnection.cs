using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class ApplicationRoleConnection(JsonApplicationRoleConnection jsonModel)
{
    public string? PlatformName { get; } = jsonModel.PlatformName;

    public IReadOnlyDictionary<string, string> Metadata { get; } = jsonModel.Metadata;
}
