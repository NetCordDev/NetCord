using NetCord.Rest;

namespace NetCord;

public partial class GroupDMChannel(JsonModels.JsonChannel jsonModel, RestClient client) : DMChannel(jsonModel, client), INamedChannel
{
    public string Name => _jsonModel.Name!;
    public string? IconHash => _jsonModel.IconHash;
    public ulong OwnerId => _jsonModel.OwnerId.GetValueOrDefault();
    public ulong? ApplicationId => _jsonModel.ApplicationId;
    public bool Managed => _jsonModel.Managed.GetValueOrDefault();
}
