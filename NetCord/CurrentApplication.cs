using NetCord.Rest;

namespace NetCord;

public partial class CurrentApplication(JsonModels.JsonApplication jsonModel, RestClient client) : Application(jsonModel, client)
{
}
