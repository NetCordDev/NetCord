using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a bot's current <see cref="Application"/>.
/// </summary>
public partial class CurrentApplication(JsonModels.JsonApplication jsonModel, RestClient client) : Application(jsonModel, client)
{
}
