using NetCord.Rest;

namespace NetCord;

public partial class CurrentUser(JsonModels.JsonUser jsonModel, RestClient client) : User(jsonModel, client)
{
}
