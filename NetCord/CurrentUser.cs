using NetCord.Rest;

namespace NetCord;

public partial class CurrentUser : User
{
    public CurrentUser(JsonModels.JsonUser jsonModel, RestClient client) : base(jsonModel, client)
    {
    }
}
