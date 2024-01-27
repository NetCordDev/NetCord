using NetCord.Rest;

namespace NetCord;

public partial class CurrentApplication : Application
{
    public CurrentApplication(JsonModels.JsonApplication jsonModel, RestClient client) : base(jsonModel, client)
    {
    }
}
